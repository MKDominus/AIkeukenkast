<script lang="ts">
	import { onMount } from 'svelte';
	import 'leaflet/dist/leaflet.css';

	let mapContainer: HTMLDivElement | null = null;

	onMount(() => {
		let map: import('leaflet').Map | null = null;

		(async () => {
			if (!mapContainer) {
				return;
			}

			const L = await import('leaflet');

			const netherlandsBounds = L.latLngBounds(
				L.latLng(50.72, 3.25),
				L.latLng(53.55, 6.95)
			);

			const initialZoom = 7.2;

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
		})();

		return () => {
			map?.remove();
		};
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
