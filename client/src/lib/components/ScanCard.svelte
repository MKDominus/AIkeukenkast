<script lang="ts">
	import type { Scan, ScanProduct } from '$lib/services/scanService';

	type Props = {
		scan: Scan;
	};

	let { scan }: Props = $props();
	let showAllProducts = $state(false);
	let selectedProduct = $state<ScanProduct | null>(null);

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
				{@const sustainabilityScore = Math.max(
					0,
					Math.min(100, detectedProduct.product?.sustainabilityScore ?? 0)
				)}
				<article class="product-details-card">
					<h4 class="product-name">{detectedProduct.product?.name ?? `Onbekend product ${index + 1}`}</h4>
					<div class="product-meta-row">
						<p><strong>{detectedProduct.product?.brand ?? '-'}</strong></p>
						<p><strong>{detectedProduct.product?.category ?? '-'}</strong></p>
					</div>
					<div class="sustainability-row">
						<p><strong>Duurzaamheidsscore:</strong> {sustainabilityScore}%</p>
						<div class="sustainability-bar" aria-hidden="true">
							<div
								class={`sustainability-fill ${sustainabilityScore >= 50 ? 'sustainability-fill-safe' : 'sustainability-fill-unsafe'}`}
								style={`width: ${sustainabilityScore}%`}
							></div>
						</div>
						<p>
							{#if detectedProduct.product?.safetyWarnings?.trim()}
								<span class="warnings-yes">Waarschuwingen aanwezig</span>
							{:else}
								<span class="warnings-no">Geen waarschuwingen</span>
							{/if}
						</p>
					</div>

					<div class="ingredients-section">
						<p><strong>Ingrediënten:</strong> {detectedProduct.product?.ingredients?.length ?? 0}</p>
						<button
							type="button"
							class="view-ingredients-link"
							onclick={() => (selectedProduct = detectedProduct)}
						>
							view ingredients
						</button>
					</div>
				</article>
			{/each}
		</section>
	{/if}
</article>

{#if selectedProduct}
	{@const detailSustainabilityScore = Math.max(
		0,
		Math.min(100, selectedProduct.product?.sustainabilityScore ?? 0)
	)}
	<button
		type="button"
		class="ingredients-modal-backdrop"
		onclick={() => (selectedProduct = null)}
		aria-label="Sluit ingrediënten details"
	></button>
	<div class="ingredients-modal" role="dialog" aria-modal="true" aria-label="Ingrediënten details">
		<button
			type="button"
			class="close-modal-button"
			onclick={() => (selectedProduct = null)}
			aria-label="Sluiten"
		>
			×
		</button>

		<h3 class="modal-product-name">{selectedProduct.product?.name ?? 'Onbekend product'}</h3>
		<p class="modal-manufacturer"><strong>Fabrikant:</strong> {selectedProduct.product?.brand ?? '-'}</p>

		<div class="modal-meta-row">
			<p><strong>Categorie:</strong> {selectedProduct.product?.category ?? '-'}</p>
			<p>
				<strong>Duurzaamheidsscore:</strong>
				<span class={detailSustainabilityScore > 50
					? 'warnings-no'
					: detailSustainabilityScore < 50
						? 'warnings-yes'
						: ''}
				>
					{detailSustainabilityScore}%
				</span>
			</p>
		</div>

		<p class="modal-warnings">
			<strong>Veiligheidswaarschuwingen:</strong>
			{selectedProduct.product?.safetyWarnings?.trim() ?? 'geen veiligheids waarshuwingen'}
		</p>

		<h4 class="modal-ingredients-header">Ingredients</h4>
		{#if selectedProduct.product?.ingredients?.length}
			<ul class="modal-ingredients-list">
				{#each selectedProduct.product.ingredients as ingredient}
					<li>
						<p class="modal-ingredient-name">{ingredient.name}</p>
						<p>{ingredient.description ?? 'geen beschrijving'}</p>
						<p>Gevaarlijk: {ingredient.isHazardous ? 'ja' : 'nee'}</p>
						<p>Concentratie: {ingredient.concentration}</p>
					</li>
				{/each}
			</ul>
		{:else}
			<p class="empty-ingredients">Geen ingrediënten beschikbaar.</p>
		{/if}
	</div>
{/if}

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
		font-size: 1.1rem;
		font-weight: 700;
		color: #6b7280;
		line-height: 1.1;
	}

	.municipality-name {
		margin: 8px 0 0;
		font-size: 1.3rem;
		font-weight: 700;
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
		font-size: 1.2rem;
		font-weight: 700;
		color: #111827;
	}

	.product-meta-row {
		display: flex;
		gap: 16px;
		align-items: baseline;
		flex-wrap: wrap;
	}

	.sustainability-row {
		display: flex;
		align-items: center;
		gap: 10px;
		margin-top: 6px;
	}

	.sustainability-row p {
		margin: 0;
		white-space: nowrap;
		line-height: 1.2;
	}

	.sustainability-bar {
		width: 6%;
		height: 8px;
		background: #e5e7eb;
		border-radius: 999px;
		overflow: hidden;
		min-width: 48px;
		align-self: center;
		margin-top: 7px;
	}

	.sustainability-fill {
		height: 100%;
		border-radius: 999px;
	}

	.sustainability-fill-safe {
		background: #16a34a;
	}

	.sustainability-fill-unsafe {
		background: #dc2626;
	}

	.warnings-yes{
		color: #dc2626;
	}

	.warnings-no{
		color: #16a34a;
	}

	.product-details-card p {
		margin: 6px 0 0;
		color: #4b5563;
	}

	.ingredients-section {
		margin-top: 10px;
		display: flex;
		gap: 10px;
		align-items: center;
		flex-wrap: wrap;
	}

	.view-ingredients-link {
		background: none;
		border: none;
		padding: 0;
		margin-top: 6px;
		font-size: 0.95rem;
		font-weight: 500;
		color: #2563eb;
		text-decoration: underline;
		cursor: pointer;
	}

	.view-ingredients-link:hover {
		color: #1d4ed8;
	}

	.empty-ingredients {
		margin: 4px 0 0;
		color: #6b7280;
	}

	.ingredients-modal-backdrop {
		position: fixed;
		inset: 0;
		border: none;
		padding: 0;
		background: rgba(17, 24, 39, 0.45);
		z-index: 40;
	}

	.ingredients-modal {
		position: fixed;
		top: 50%;
		left: 50%;
		transform: translate(-50%, -50%);
		z-index: 41;
		width: min(640px, calc(100vw - 32px));
		max-height: calc(100vh - 48px);
		overflow-y: auto;
		background: white;
		border: 1px solid #d1d5db;
		border-radius: 14px;
		box-shadow: 0 24px 50px rgba(0, 0, 0, 0.2);
		padding: 18px;
	}

	.close-modal-button {
		position: absolute;
		top: 10px;
		right: 12px;
		border: none;
		background: transparent;
		font-size: 1.5rem;
		line-height: 1;
		color: #6b7280;
		cursor: pointer;
	}

	.modal-product-name {
		margin: 0;
		font-weight: 700;
		font-size: 1.35rem;
		color: #111827;
	}

	.modal-manufacturer {
		margin: 6px 0 0;
		color: #4b5563;
	}

	.modal-meta-row {
		display: flex;
		gap: 16px;
		flex-wrap: wrap;
		margin-top: 10px;
	}

	.modal-meta-row p {
		margin: 0;
		color: #4b5563;
	}

	.modal-warnings {
		margin-top: 10px;
		padding: 10px;
		border: 1px solid #e5e7eb;
		border-radius: 10px;
		background: #f9fafb;
		color: #374151;
	}

	.modal-ingredients-header {
		margin: 14px 0 0;
		font-size: 1.05rem;
		color: #111827;
	}

	.modal-ingredients-list {
		margin: 8px 0 0;
		padding-left: 18px;
		color: #374151;
	}

	.modal-ingredients-list li {
		margin-top: 8px;
		padding: 8px;
		border: 1px solid #e5e7eb;
		border-radius: 8px;
		background: #fcfcfd;
		list-style: none;
	}

	.modal-ingredients-list p {
		margin: 4px 0 0;
		color: #4b5563;
	}

	.modal-ingredient-name {
		margin: 0;
		font-weight: 600;
		color: #111827;
	}
</style>
