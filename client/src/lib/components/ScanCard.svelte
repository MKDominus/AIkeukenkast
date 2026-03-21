<script lang="ts">
	import type { Scan } from '$lib/services/scanService';

	type Props = {
		scan: Scan;
	};

	let { scan }: Props = $props();
</script>

<article class="scan-card">
	<h3>Scan #{scan.id}</h3>
	<p><strong>Date:</strong> {new Date(scan.scanDate).toLocaleString()}</p>
	<p><strong>User:</strong> {scan.user?.name ?? 'Unknown user'}</p>
	<p><strong>Municipality:</strong> {scan.municipality?.name ?? 'Unknown municipality'}</p>
	<p><strong>Image URL:</strong> {scan.imageUrl}</p>
	<p><strong>Detected products:</strong> {scan.detectedProducts.length}</p>

	{#if scan.detectedProducts.length > 0}
		<ul>
			{#each scan.detectedProducts as detectedProduct}
				<li>
					{detectedProduct.product?.name ?? `Product #${detectedProduct.productId}`}
					({detectedProduct.count}x, confidence {Math.round(detectedProduct.confidence * 100)}%)
				</li>
			{/each}
		</ul>
	{/if}
</article>
