<script lang="ts">
	import type { Scan } from '$lib/services/scanService';

	type Props = {
		scan: Scan;
	};

	let { scan }: Props = $props();
	let showAllProducts = $state(false);

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
	<div class="scan-card-header">
		<div class="scan-main-info">
			<h3 class="municipality-label">municipality</h3>
			<p class="municipality-name">{scan.municipality?.name ?? 'Onbekende gemeente'}</p>
			<p class="scan-date">gescanned op {new Date(scan.scanDate).toLocaleString()}</p>
			<p class="detected-products">{scan.detectedProducts.length} product(en) gedetecteerd</p>
			<p class="sustainability-summary">
				<span class="sustainability-safe">{sustainableProductsDetected} duurzaam</span>
				<span class="sustainability-unsafe">{nonSustainableProductsDetected} niet duurzaam</span>
			</p>
		</div>

		<button
			type="button"
			class="show-products-toggle"
			onclick={() => (showAllProducts = !showAllProducts)}
			aria-expanded={showAllProducts}
		>
			<span>klik om alle producten te zien</span>
			<span class="toggle-arrow" aria-hidden="true">{showAllProducts ? '▲' : '▼'}</span>
		</button>
	</div>

	{#if showAllProducts}
		<section class="products-details">
			{#each scan.detectedProducts as detectedProduct, index}
				<article class="product-details-card">
					<h4 class="product-name">{detectedProduct.product?.name ?? `Onbekend product ${index + 1}`}</h4>
					<p><strong>Merk:</strong> {detectedProduct.product?.brand ?? '-'}</p>
					<p><strong>Categorie:</strong> {detectedProduct.product?.category ?? '-'}</p>
					<p><strong>Aantal gedetecteerd:</strong> {detectedProduct.count}</p>
					<p><strong>Betrouwbaarheid:</strong> {(detectedProduct.confidence).toFixed(1)}%</p>
					<p><strong>Duurzaamheidsscore:</strong> {detectedProduct.product?.sustainabilityScore ?? '-'}</p>
					<p>
						<strong>Duurzaam:</strong>
						{detectedProduct.product?.isSustainable == null
							? '-'
							: detectedProduct.product.isSustainable
								? 'Ja'
								: 'Nee'}
					</p>
					<p><strong>Veiligheidswaarschuwingen:</strong> {detectedProduct.product?.safetyWarnings ?? '-'}</p>
					<p><strong>Afbeelding URL:</strong> {detectedProduct.product?.imageUrl ?? '-'}</p>

					<div class="ingredients-section">
						<p><strong>Ingrediënten:</strong></p>
						{#if detectedProduct.product?.ingredients?.length}
							<ul class="ingredients-list">
								{#each detectedProduct.product.ingredients as ingredient}
									<li>
										<span>{ingredient.name}</span>
										<span> — {ingredient.description ?? 'geen beschrijving'}</span>
										<span>
											 — gevaarlijk: {ingredient.isHazardous ? 'ja' : 'nee'}, concentratie:
											{ingredient.concentration}
										</span>
									</li>
								{/each}
							</ul>
						{:else}
							<p class="empty-ingredients">Geen ingrediënten beschikbaar.</p>
						{/if}
					</div>
				</article>
			{/each}
		</section>
	{/if}
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

	.scan-card-header {
		display: flex;
		justify-content: space-between;
		gap: 18px;
	}

	.scan-main-info {
		flex: 1;
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

	.show-products-toggle {
		align-self: flex-start;
		display: inline-flex;
		flex-direction: column;
		align-items: center;
		justify-content: center;
		gap: 4px;
		padding: 8px 12px;
		color: #374151;
		font-size: 0.85rem;
		font-weight: 600;
		cursor: pointer;
		text-align: center;
		min-width: 155px;
	}

	.toggle-arrow {
		font-size: 0.9rem;
		line-height: 1;
	}

	.products-details {
		margin-top: 14px;
		display: grid;
		gap: 10px;
	}

	.product-details-card {
		border: 1px solid #e5e7eb;
		border-radius: 10px;
		padding: 12px;
		background: #fcfcfd;
	}

	.product-name {
		margin: 0 0 8px;
		font-size: 1rem;
		color: #111827;
	}

	.product-details-card p {
		margin: 6px 0 0;
		color: #4b5563;
	}

	.ingredients-section {
		margin-top: 10px;
	}

	.ingredients-list {
		margin: 6px 0 0;
		padding-left: 18px;
		color: #4b5563;
	}

	.ingredients-list li {
		margin-top: 4px;
	}

	.empty-ingredients {
		margin: 4px 0 0;
		color: #6b7280;
	}
</style>
