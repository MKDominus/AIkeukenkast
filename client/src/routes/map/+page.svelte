<script lang="ts">
	import Header from '$lib/components/Header.svelte';
	import NetherlandsMap from '$lib/components/NetherlandsMap.svelte';
	import cleaningPic from '$lib/assets/cleaning_pic.png';
	import type { MunicipalityScanCount } from '$lib/services/municipalityService';

	type Props = {
		data: {
			municipalityScanCounts: MunicipalityScanCount[];
		};
	};

	let { data }: Props = $props();

	let municipalityTotal = $derived(data.municipalityScanCounts.length);
	let scanTotal = $derived(
		data.municipalityScanCounts.reduce((sum, municipality) => sum + municipality.scanCount, 0)
	);
	let topMunicipality = $derived(
		data.municipalityScanCounts.length > 0
			? data.municipalityScanCounts.reduce((highest, current) =>
					current.scanCount > highest.scanCount ? current : highest
				)
			: null
	);
</script>

<div id="map-top"></div>
<Header></Header>

<main id="mapPage">
	<section id="mapBanner" aria-label="Map banner">
		<img src={cleaningPic} alt="Schoonmaakproducten banner" />
		<div class="bannerLabel">Scans per gemeente</div>
	</section>

	<section id="mapPanel" aria-label="Kaart panel">
		<h2>Scanintensiteit Per Gemeente</h2>
		<p>Klik op een marker om het aantal scans per gemeente te bekijken.</p>

		<div id="mapLayout">
			<aside id="mapSummary" aria-label="Kaart samenvatting">
				<h3>Samenvatting</h3>
				<div class="summaryStats">
					<div class="summaryItem">
						<span class="label">Gemeenten in kaart</span>
						<strong>{municipalityTotal}</strong>
					</div>
					<div class="summaryItem">
						<span class="label">Totaal aantal scans</span>
						<strong>{scanTotal}</strong>
					</div>
					{#if topMunicipality}
						<div class="summaryItem">
							<span class="label">Hoogste scanvolume</span>
							<strong>{topMunicipality.municipalityName} ({topMunicipality.scanCount})</strong>
						</div>
					{/if}
				</div>

				<div class="legendBox" aria-label="Marker legenda">
					<h4>Marker legenda</h4>
					<div class="legendRow">
						<span class="dot small"></span>
						<span>Lager aantal scans</span>
					</div>
					<div class="legendRow">
						<span class="dot medium"></span>
						<span>Middelgroot aantal scans</span>
					</div>
					<div class="legendRow">
						<span class="dot large"></span>
						<span>Hoger aantal scans</span>
					</div>
				</div>
			</aside>

			<div id="mapArea">
				<NetherlandsMap municipalityCounts={data.municipalityScanCounts} />
			</div>
		</div>
	</section>
</main>

<style>
	#mapPage {
		margin: 24px 0 30px;
		display: grid;
		gap: 22px;
	}

	#mapBanner {
		position: relative;
		display: flex;
		align-items: center;
		justify-content: flex-end;
		overflow: hidden;
		background-color: var(--color-primary);
		min-height: clamp(210px, 25vw, 290px);
		padding-top: 18px;
		padding-bottom: 18px;
	}

	#mapBanner img {
		display: block;
		width: clamp(340px, 64%, 820px);
		height: clamp(190px, 23vw, 280px);
		object-fit: cover;
		object-position: 28% center;
		margin-right: clamp(14px, 4vw, 62px);
		border-radius: 22px;
	}

	.bannerLabel {
		position: absolute;
		left: clamp(16px, 4vw, 54px);
		top: 50%;
		transform: translateY(-50%);
		padding: 0;
		max-width: min(44%, 420px);
		background: transparent;
		color: var(--color-bg);
		border: none;
		font-family: var(--font-header);
		font-weight: 700;
		font-size: clamp(1.55rem, 2.2vw, 2.15rem);
		line-height: 1.15;
		letter-spacing: 0.01em;
	}

	#mapPanel {
		padding: 16px;
		margin: 24px 30px 0;
		border: none;
		border-radius: 12px;
		background-color: var(--color-bg);
	}

	#mapPanel h2 {
		margin: 0;
		color: var(--color-primary-dark);
		font-family: var(--font-header);
	}

	#mapPanel p {
		margin: 6px 0 16px;
		color: var(--color-primary-dark);
		font-weight: 600;
	}

	#mapLayout {
		display: grid;
		grid-template-columns: minmax(230px, 310px) minmax(0, 1fr);
		gap: 12px;
		align-items: start;
	}

	#mapSummary {
		background: var(--color-bg);
		border: 2px solid var(--color-primary);
		border-top: 6px solid var(--color-secondary-dark);
		border-radius: 12px;
		padding: 14px;
	}

	#mapSummary h3,
	#mapSummary h4 {
		margin: 0 0 10px;
		color: var(--color-primary-dark);
		font-family: var(--font-header);
	}

	.summaryStats {
		display: grid;
		gap: 10px;
		margin-bottom: 14px;
	}

	.summaryItem {
		padding: 10px;
		border: 1px solid var(--color-primary);
		border-radius: 10px;
		background: var(--color-bg);
		display: grid;
		gap: 4px;
	}

	.summaryItem .label {
		font-weight: 600;
		color: var(--color-primary-dark);
		font-size: 0.95rem;
	}

	.legendBox {
		padding: 10px;
		border-radius: 10px;
		border: 1px solid var(--color-primary);
	}

	.legendRow {
		display: flex;
		align-items: center;
		gap: 8px;
		color: var(--color-primary-dark);
		font-weight: 600;
		margin-bottom: 7px;
	}

	.legendRow:last-child {
		margin-bottom: 0;
	}

	.dot {
		display: inline-block;
		border-radius: 999px;
		background: var(--color-primary);
		border: 2px solid var(--color-primary-dark);
	}

	.dot.small {
		width: 10px;
		height: 10px;
	}

	.dot.medium {
		width: 16px;
		height: 16px;
	}

	.dot.large {
		width: 22px;
		height: 22px;
	}

	#mapArea {
		display: flex;
		justify-content: flex-start;
	}

	:global(#mapPanel .mapSection) {
		margin: 0;
		width: min(100%, 900px);
	}

	@media (max-width: 700px) {
		#mapPage {
			margin: 18px 0 24px;
		}

		#mapPanel {
			margin: 18px 16px 0;
		}

		#mapLayout {
			grid-template-columns: 1fr;
		}

		#mapArea {
			justify-content: stretch;
		}

		:global(#mapPanel .mapSection) {
			width: 100%;
		}

		.bannerLabel {
			left: 10px;
			right: 10px;
			bottom: 10px;
			top: auto;
			transform: none;
			max-width: none;
			text-align: center;
			font-size: 1.3rem;
		}

		#mapBanner img {
			width: calc(100% - 28px);
			margin-right: 14px;
			height: 200px;
		}
	}
</style>
