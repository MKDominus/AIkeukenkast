<script lang="ts">
	import { cubicOut } from 'svelte/easing';
	import { slide } from 'svelte/transition';
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
			<div class="municipality-block">
				<svg class="municipality-icon" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="2001" height="2001" viewBox="0 0 2001 2001" aria-hidden="true">
					<path fill-rule="evenodd" fill="rgb(100%, 100%, 100%)" fill-opacity="0" d="M 0.921875 0.078125 L 2000.921875 0.078125 L 2000.921875 2000.078125 L 0.921875 2000.078125 L 0.921875 0.078125 "/>
					<path fill-rule="evenodd" fill="currentColor" fill-opacity="1" d="M 1000.921875 506.410156 C 1185.269531 506.410156 1334.710938 655.851562 1334.710938 840.199219 C 1334.710938 960.078125 1262.21875 1049.703125 1204.289062 1148.144531 L 1000.921875 1493.753906 L 797.546875 1148.144531 C 739.617188 1049.703125 667.128906 960.078125 667.128906 840.199219 C 667.128906 655.851562 816.570312 506.410156 1000.921875 506.410156 Z M 1000.921875 689.648438 C 1086.308594 689.648438 1155.53125 758.871094 1155.53125 844.261719 C 1155.53125 929.648438 1086.308594 998.871094 1000.921875 998.871094 C 915.53125 998.871094 846.308594 929.648438 846.308594 844.261719 C 846.308594 758.871094 915.53125 689.648438 1000.921875 689.648438 "/>
				</svg>
				<h3 class="municipality-label">municipality</h3>
				<p class="municipality-name">{scan.municipality?.name ?? 'Onbekende gemeente'}</p>
			</div>
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
		<section
			class="products-details"
			transition:slide={{ duration: 220, easing: cubicOut, axis: 'y' }}
		>
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
		background-color: var(--color-bg);
		border: 1px solid var(--color-border);
		box-shadow: 0 2px 8px rgba(65, 20, 71, 0.12);
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

	.municipality-block {
		position: relative;
		padding-left: 34px;
	}

	.municipality-icon {
		position: absolute;
		left: -16px;
		top: -2px;
		width: 52px;
		height: 52px; 
		color: var(--color-primary);
		pointer-events: none;
	}

	.municipality-label {
		margin: 0;
		font-size: 1.1rem;
		font-weight: 700;
		color: var(--color-text-muted);
		line-height: 1.1;
	}

	.municipality-name {
		margin: 8px 0 0;
		font-size: 1.3rem;
		font-weight: 700;
		color: var(--color-primary-dark);
	}

	.scan-date,
	.detected-products,
	.sustainability-summary {
		margin: 8px 0 0;
		color: var(--color-text-muted);
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
		background: var(--color-bg);
		border: 1px solid var(--color-secondary);
		color: var(--color-secondary-dark);
	}

	.sustainability-unsafe {
		background: var(--color-bg);
		border: 1px solid var(--color-primary);
		color: var(--color-primary-dark);
	}

	.show-products-toggle {
		align-self: flex-start;
		display: inline-flex;
		flex-direction: column;
		align-items: center;
		justify-content: center;
		gap: 4px;
		padding: 8px 12px;
		color: var(--color-primary);
		font-size: 0.85rem;
		font-weight: 600;
		cursor: pointer;
		text-align: center;
		min-width: 155px;
		border: 1px solid var(--color-primary);
		background: var(--color-bg);
		border-radius: 2rem;
	}

	.show-products-toggle:hover {
		background: var(--color-primary);
		color: var(--color-bg);
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
		border: 1px solid var(--color-border);
		border-radius: 10px;
		padding: 12px;
		background-color: #ffffff;
		background-image: none;
	}

	.product-name {
		margin: 0 0 8px;
		font-size: 1.2rem;
		font-weight: 700;
		color: var(--color-primary-dark);
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
		background: var(--color-border);
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
		background: var(--color-secondary);
	}

	.sustainability-fill-unsafe {
		background: var(--color-primary);
	}

	.warnings-yes{
		color: var(--color-primary-dark);
	}

	.warnings-no{
		color: var(--color-secondary-dark);
	}

	.product-details-card p {
		margin: 6px 0 0;
		color: var(--color-text-muted);
	}

	.ingredients-section {
		margin-top: 10px;
		display: flex;
		gap: 10px;
		align-items: center;
		flex-wrap: wrap;
	}

	.view-ingredients-link {
		background: var(--color-bg);
		border: 1px solid var(--color-primary);
		padding: 6px 10px;
		margin-top: 6px;
		font-size: 0.95rem;
		font-weight: 500;
		color: var(--color-primary);
		text-decoration: none;
		cursor: pointer;
		border-radius: 2rem;
	}

	.view-ingredients-link:hover {
		background: var(--color-primary);
		color: var(--color-bg);
	}

	.empty-ingredients {
		margin: 4px 0 0;
		color: var(--color-text-muted);
	}

	.ingredients-modal-backdrop {
		position: fixed;
		inset: 0;
		border: none;
		padding: 0;
		background: rgba(17, 24, 39, 0.28);
		backdrop-filter: blur(4px);
		-webkit-backdrop-filter: blur(4px);
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
		background: var(--color-bg);
		border: 1px solid var(--color-border);
		border-radius: 14px;
		box-shadow: 0 24px 50px rgba(65, 20, 71, 0.24);
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
		color: var(--color-text-muted);
		cursor: pointer;
	}

	.modal-product-name {
		margin: 0;
		font-weight: 700;
		font-size: 1.35rem;
		color: var(--color-primary-dark);
	}

	.modal-manufacturer {
		margin: 6px 0 0;
		color: var(--color-text-muted);
	}

	.modal-meta-row {
		display: flex;
		gap: 16px;
		flex-wrap: wrap;
		margin-top: 10px;
	}

	.modal-meta-row p {
		margin: 0;
		color: var(--color-text-muted);
	}

	.modal-warnings {
		margin-top: 10px;
		padding: 10px;
		border: 1px solid var(--color-primary);
		border-radius: 10px;
		background: var(--color-bg);
		color: var(--color-primary-dark);
	}

	.modal-ingredients-header {
		margin: 14px 0 0;
		font-size: 1.05rem;
		color: var(--color-primary-dark);
	}

	.modal-ingredients-list {
		margin: 8px 0 0;
		padding-left: 18px;
		color: var(--color-text);
	}

	.modal-ingredients-list li {
		margin-top: 8px;
		padding: 8px;
		border: 1px solid var(--color-primary);
		border-radius: 8px;
		background: var(--color-bg);
		list-style: none;
	}

	.modal-ingredients-list p {
		margin: 4px 0 0;
		color: var(--color-text-muted);
	}

	.modal-ingredient-name {
		margin: 0;
		font-weight: 600;
		color: var(--color-primary-dark);
	}

	@media (max-width: 900px) {
		.scan-card {
			padding: 14px;
		}

		.scan-card-header {
			flex-direction: column;
			gap: 12px;
		}

		.show-products-toggle {
			align-self: stretch;
			width: 100%;
			min-width: 0;
		}

		.sustainability-row {
			flex-wrap: wrap;
			align-items: center;
		}

		.sustainability-row p {
			white-space: normal;
		}

		.sustainability-bar {
			width: 84px;
			min-width: 84px;
			margin-top: 0;
		}

		.ingredients-modal {
			width: min(560px, calc(100vw - 24px));
			max-height: calc(100vh - 28px);
			padding: 16px;
		}
	}

	@media (max-width: 600px) {
		.municipality-block {
			padding-left: 28px;
		}

		.municipality-icon {
			left: -12px;
			top: -1px;
			width: 40px;
			height: 40px;
		}

		.municipality-name {
			font-size: 1.15rem;
		}

		.product-meta-row {
			flex-direction: column;
			gap: 4px;
			align-items: flex-start;
		}

		.ingredients-section {
			align-items: flex-start;
		}

		.view-ingredients-link {
			margin-top: 2px;
		}

		.ingredients-modal {
			padding: 14px;
		}

		.modal-meta-row {
			flex-direction: column;
			gap: 6px;
		}

		.modal-ingredients-list {
			padding-left: 0;
		}
	}
</style>
