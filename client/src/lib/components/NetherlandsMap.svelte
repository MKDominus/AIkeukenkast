<script lang="ts">
	import { onMount } from 'svelte';
	import 'leaflet/dist/leaflet.css';
	import type { MunicipalityScanCount } from '$lib/services/municipalityService';

	type GeocodedMunicipality = MunicipalityScanCount & {
		lat: number;
		lon: number;
	};

	type GeocodeCacheEntry = {
		lat: number;
		lon: number;
	};

	type GeocodeCache = Partial<Record<string, GeocodeCacheEntry>>;

	const GEOCODE_CACHE_KEY = 'municipality-geocode-cache-v1';
	const MIN_MARKER_RADIUS = 8;
	const MAX_MARKER_RADIUS = 26;

	let { municipalityCounts = [] }: { municipalityCounts?: MunicipalityScanCount[] } = $props();
	let mapContainer: HTMLDivElement | null = null;
	let map: import('leaflet').Map | null = null;
	let markerLayer: import('leaflet').LayerGroup | null = null;
	let leafletModule: typeof import('leaflet') | null = null;
	let renderRunId = 0;

	function normalizeMunicipalityName(name: string): string {
		return name.trim().toLowerCase();
	}

	function loadGeocodeCache(): GeocodeCache {
		if (typeof localStorage === 'undefined') {
			return {};
		}

		try {
			const rawCache = localStorage.getItem(GEOCODE_CACHE_KEY);
			if (!rawCache) {
				return {};
			}

			const parsed = JSON.parse(rawCache);
			if (parsed && typeof parsed === 'object') {
				return parsed as GeocodeCache;
			}
		} catch {
			return {};
		}

		return {};
	}

	function saveGeocodeCache(cache: GeocodeCache): void {
		if (typeof localStorage === 'undefined') {
			return;
		}

		try {
			localStorage.setItem(GEOCODE_CACHE_KEY, JSON.stringify(cache));
		} catch {
			// Ignore storage failures and continue with in-memory results.
		}
	}

	async function geocodeMunicipality(name: string): Promise<GeocodeCacheEntry | null> {
		const query = encodeURIComponent(`${name}, Netherlands`);
		const endpoint = `https://nominatim.openstreetmap.org/search?format=jsonv2&countrycodes=nl&limit=1&q=${query}`;

		const response = await fetch(endpoint, {
			headers: {
				Accept: 'application/json'
			}
		});

		if (!response.ok) {
			return null;
		}

		const results = (await response.json()) as Array<{ lat: string; lon: string }>;
		if (!Array.isArray(results) || results.length === 0) {
			return null;
		}

		const latitude = Number(results[0].lat);
		const longitude = Number(results[0].lon);

		if (!Number.isFinite(latitude) || !Number.isFinite(longitude)) {
			return null;
		}

		return { lat: latitude, lon: longitude };
	}

	function wait(milliseconds: number): Promise<void> {
		return new Promise((resolve) => {
			setTimeout(resolve, milliseconds);
		});
	}

	async function resolveMunicipalityCoordinates(
		counts: MunicipalityScanCount[]
	): Promise<GeocodedMunicipality[]> {
		const cache = loadGeocodeCache();
		const resolved: GeocodedMunicipality[] = [];
		const seenNames = new Set<string>();

		for (const municipality of counts) {
			const normalizedName = normalizeMunicipalityName(municipality.municipalityName);
			if (seenNames.has(normalizedName)) {
				continue;
			}

			seenNames.add(normalizedName);

			let coordinates = cache[normalizedName] ?? null;
			if (!coordinates) {
				coordinates = await geocodeMunicipality(municipality.municipalityName);
				if (coordinates) {
					cache[normalizedName] = coordinates;
				}
				// Keep requests polite to avoid geocoding rate-limits.
				await wait(275);
			}

			if (!coordinates) {
				continue;
			}

			resolved.push({
				...municipality,
				lat: coordinates.lat,
				lon: coordinates.lon
			});
		}

		saveGeocodeCache(cache);
		return resolved;
	}

	function getRadiusForCount(scanCount: number, distinctCounts: number[]): number {
		if (distinctCounts.length <= 1) {
			return (MIN_MARKER_RADIUS + MAX_MARKER_RADIUS) / 2;
		}

		const rank = distinctCounts.findIndex((count) => count === scanCount);
		if (rank === -1) {
			return MIN_MARKER_RADIUS;
		}

		const percentile = 1 - rank / (distinctCounts.length - 1);
		return MIN_MARKER_RADIUS + percentile * (MAX_MARKER_RADIUS - MIN_MARKER_RADIUS);
	}

	async function renderMunicipalityMarkers(): Promise<void> {
		if (!map || !leafletModule) {
			return;
		}

		const currentRunId = ++renderRunId;
		const geocodedMunicipalities = await resolveMunicipalityCoordinates(municipalityCounts);
		if (currentRunId !== renderRunId || !map || !leafletModule) {
			return;
		}

		if (markerLayer) {
			markerLayer.remove();
		}

		markerLayer = leafletModule.layerGroup().addTo(map);

		if (geocodedMunicipalities.length === 0) {
			return;
		}

		const distinctCounts = [...new Set(geocodedMunicipalities.map((item) => item.scanCount))].sort(
			(a, b) => b - a
		);

		for (const municipality of geocodedMunicipalities) {
			const radius = getRadiusForCount(municipality.scanCount, distinctCounts);
			leafletModule
				.circleMarker([municipality.lat, municipality.lon], {
					radius,
					color: 'var(--color-primary-dark)',
					fillColor: 'var(--color-primary)',
					fillOpacity: 0.78,
					weight: 2
				})
				.bindTooltip(`${municipality.municipalityName}: ${municipality.scanCount} scans`)
				.addTo(markerLayer);
		}
	}

	onMount(() => {
		(async () => {
			if (!mapContainer) {
				return;
			}

			const L = await import('leaflet');
			leafletModule = L;

			const netherlandsBounds = L.latLngBounds(
				L.latLng(50.72, 3.25),
				L.latLng(53.55, 6.95)
			);

			const initialZoom = 7.8;

			map = L.map(mapContainer, {
				center: [52.1326, 5.2913],
				zoom: initialZoom,
				minZoom: initialZoom,
				zoomSnap: 0.1,
				maxBounds: netherlandsBounds,
				maxBoundsViscosity: 1.0
			});

			L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
				attribution: '&copy; OpenStreetMap contributors'
			}).addTo(map);

			await renderMunicipalityMarkers();
		})();

		return () => {
			renderRunId += 1;
			markerLayer?.remove();
			map?.remove();
			markerLayer = null;
			map = null;
			leafletModule = null;
		};
	});

	$effect(() => {
		municipalityCounts;
		if (!map || !leafletModule) {
			return;
		}

		void renderMunicipalityMarkers();
	});
</script>

<section id="map-section" class="mapSection" aria-label="Kaart van Nederland">
	<div class="mapFrame" bind:this={mapContainer}></div>
</section>

<style>
	.mapSection {
		padding: 10px;
		margin: 30px auto;
		width: min(calc(100% - 60px), 924px);
		box-sizing: border-box;
		background-color: var(--color-bg);
		border: 2px solid var(--color-primary);
		border-top: 6px solid var(--color-secondary-dark);
		border-radius: 10px;
	}

	.mapFrame {
		display: block;
		width: 100%;
		aspect-ratio: 1 / 1;
		max-height: 72vh;
		border: 0;
		border-radius: 8px;
		overflow: hidden;
	}
</style>
