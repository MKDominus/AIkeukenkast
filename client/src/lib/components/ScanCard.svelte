<script lang="ts">
	import type { Scan } from '$lib/services/scanService';

	type Props = {
		scan: Scan;
	};

	let { scan }: Props = $props();

	let sustainableProductsDetected = $derived(
		scan.detectedProducts
			.filter((detectedProduct) => detectedProduct.product?.isSustainable === true)
			.reduce((sum, detectedProduct) => sum + detectedProduct.count, 0)
	);

	let nonSustainableProductsDetected = $derived(
		scan.detectedProducts
			.filter((detectedProduct) => detectedProduct.product?.isSustainable === false)
			.reduce((sum, detectedProduct) => sum + detectedProduct.count, 0)
	);
</script>

<article class="scan-card">
	<h3 class="municipality-label">municipality</h3>
	<p class="municipality-name">{scan.municipality?.name ?? 'Onbekende gemeente'}</p>
	<p class="scan-date">gescanned op {new Date(scan.scanDate).toLocaleString()}</p>
	<p class="detected-products">{scan.detectedProducts.length} product(en) gedetecteerd</p>
	<p class="sustainability-summary">
		<span class="sustainability-safe">{sustainableProductsDetected} duurzaam</span> <span class="sustainability-unsafe">{nonSustainableProductsDetected} niet duurzaam</span>
	</p>
</article>

<style>
	.scan-card {
		background-color: white;
		border: 1px solid #d1d5db;
		box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
		border-radius: 10px;
		padding: 18px;
		width: 100%;
		box-sizing: border-box;
		margin-bottom: 14px;
	}

	.municipality-label {
		margin: 0;
		font-size: 1.2rem;
		font-weight: 700;
		color: #6b7280;
		line-height: 1.1;
	}

	.municipality-name {
		margin: 8px 0 0;
		font-size: 1.3rem;
		font-weight: 500;
		color: #111827;
	}

	.scan-date,
	.detected-products,
	.sustainability-summary {
		margin: 8px 0 0;
		color: #6b7280;
	}

	.sustainability-summary {
		display: flex;
		gap: 8px;
		flex-wrap: wrap;
	}

	.sustainability-safe,
	.sustainability-unsafe {
		display: inline-flex;
		align-items: center;
		padding: 4px 10px;
		border-radius: 999px;
		font-size: 0.9rem;
		font-weight: 600;
		line-height: 1.2;
	}

	.sustainability-safe {
		background: #b3e1c5;
		color: #008555;
	}

	.sustainability-unsafe {
		background: #ffd3d3;
		color: #b42318;
	}
</style>
