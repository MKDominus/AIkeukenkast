<script lang="ts">
	import { cubicOut } from 'svelte/easing';
	import { slide } from 'svelte/transition';
	import type { Scan, ScanProduct } from '$lib/services/scanService';
	import locationIcon from '$lib/assets/dashboard_icons/location_icon.png';

	type Props = {
		scan: Scan;
	};

	let { scan }: Props = $props();
	let showAllProducts = $state(false);
	let selectedProduct = $state<ScanProduct | null>(null);

let safeProductsDetected = $derived(
scan.detectedProducts
.filter((detectedProduct) => detectedProduct.product?.riskLevel === 'Veilig')
.reduce((sum, detectedProduct) => sum + detectedProduct.count, 0)
);

let riskantProductsDetected = $derived(
scan.detectedProducts
.filter((detectedProduct) => detectedProduct.product?.riskLevel === 'Riskant')
.reduce((sum, detectedProduct) => sum + detectedProduct.count, 0)
);

let onveiligProductsDetected = $derived(
scan.detectedProducts
.filter((detectedProduct) => detectedProduct.product?.riskLevel === 'Onveilig')
.reduce((sum, detectedProduct) => sum + detectedProduct.count, 0)
);

function getRiskLabel(riskLevel: string | null | undefined) {
return riskLevel ?? 'Onbekend';
}

function getRiskClass(riskLevel: string | null | undefined) {
switch (riskLevel) {
case 'Veilig':
return 'warnings-no';
case 'Riskant':
return 'warnings-riskant';
case 'Onveilig':
return 'warnings-yes';
default:
return 'warnings-no';
}
}
	function formatTextList(values: string[] | undefined | null) {
		if (!values || values.length === 0) {
			return 'Geen';
		}

		return values.join(', ');
	}

	function formatWarningLabels(labels: { type: string; description: string }[] | undefined | null) {
		if (!labels || labels.length === 0) {
			return 'Geen waarschuwingen';
		}

		return labels.map((label) => `${label.type}: ${label.description}`).join('; ');
	}
</script>

<article class="scan-card">
	<div class="scan-card-header">
	<div class="scan-left">
		<img src={locationIcon} alt="Locatie icoon" class="municipality-icon" />

		<div class="scan-info">
			<div class="scan-meta">
				<span class="municipality-tag">GEMEENTE</span>
				<span class="scan-date">
					{new Date(scan.scanDate).toLocaleString()}
				</span>
			</div>

			<h3 class="municipality-name">
				{scan.municipality?.name ?? 'Onbekende gemeente'}
			</h3>

			<p class="detected-products">
				{scan.detectedProducts.length} product(en) gedetecteerd
			</p>
		</div>
	</div>

	<div class="scan-right">
		<div class="sustainability-summary">
			<span class="sustainability-safe">
				{safeProductsDetected} veilig
			</span>

			<span class="sustainability-riskant">
				{riskantProductsDetected} riskant
			</span>

			<span class="sustainability-unsafe">
				{onveiligProductsDetected} onveilig
			</span>
		</div>

		<button
			type="button"
			class="show-products-toggle"
			onclick={() => (showAllProducts = !showAllProducts)}
			aria-expanded={showAllProducts}
		>
			Bekijk producten
			<span>{showAllProducts ? '▲' : '▼'}</span>
		</button>
	</div>
</div>

	{#if showAllProducts}
		<section
			class="products-details"
			transition:slide={{ duration: 220, easing: cubicOut, axis: 'y' }}
		>
			{#each scan.detectedProducts as detectedProduct, index}
				<article class="product-details-card">
					<h2 class="product-name">{detectedProduct.product?.productName ?? `Onbekend product ${index + 1}`}</h2>

					<div class="details-section">
						<button
							type="button"
							class="view-details-link"
							onclick={() => (selectedProduct = detectedProduct)}
						>
							Toon details
						</button>
					</div>
				</article>
			{/each}
		</section>
	{/if}
</article>

{#if selectedProduct}
	{@const selectedRiskLevel = selectedProduct.product?.riskLevel ?? 'Onbekend'}
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

		<h3 class="modal-product-name">{selectedProduct.product?.productName ?? 'Onbekend product'}</h3>
		<p class="modal-manufacturer"><strong>Producttype:</strong> {selectedProduct.product?.productType ?? '-'}</p>

		<div class="modal-meta-row">
			<p>
				<strong>Risiconiveau:</strong>
				<span class={getRiskClass(selectedRiskLevel)}>{getRiskLabel(selectedRiskLevel)}</span>
			</p>
		</div>

		<p class="modal-warnings">
			<strong>Waarschuwingen:</strong>
			{formatWarningLabels(selectedProduct.product?.warningLabels)}
		</p>

		<p class="modal-warnings">
			<strong>Gevaar:</strong> {formatTextList(selectedProduct.product?.dangers)}
		</p>

		<p class="modal-warnings">
			<strong>Voorzorgsmaatregelen:</strong> {formatTextList(selectedProduct.product?.precautions)}
		</p>

		<p class="modal-warnings">
			<strong>Alternatieven:</strong> {formatTextList(selectedProduct.product?.alternatives)}
		</p>
	</div> 
{/if}

<style>
	.scan-card {
		background-color: var(--color-bg);
		border: 1px solid var(--color-border);
		box-shadow: var(--shadow-card);
		border-radius: 10px;
		padding: 18px;
		width: 100%;
		box-sizing: border-box;
		margin-bottom: 14px;
	}

	.scan-card-header {
	display: flex;
	justify-content: space-between;
	align-items: center;
	gap: 24px;
}

.scan-left {
	display: flex;
	align-items: center;
	gap: 18px;
	flex: 1;
}

.scan-info {
	display: flex;
	flex-direction: column;
	gap: 4px;
}

.scan-meta {
	display: flex;
	align-items: center;
	gap: 12px;
}

.municipality-tag {
	background: #dff0d0;
	color: #2f5d1f;
	font-size: 0.72rem;
	font-weight: 700;
	padding: 4px 10px;
	border-radius: 999px;
	letter-spacing: 0.04em;
}

.municipality-icon {
	width: 34px;
	height: 34px;
	color: var(--color-primary);
	flex-shrink: 0;
}

.municipality-name {
	margin: 0;
	font-size: 1.2rem;
	font-weight: 700;
	color: var(--color-primary-dark);
}

.scan-date {
	color: var(--color-text-muted);
	font-size: 0.9rem;
}

.detected-products {
	margin: 0;
	color: var(--color-text-muted);
	font-size: 0.95rem;
}

.scan-right {
	display: flex;
	align-items: center;
	gap: 18px;
}

.sustainability-summary {
	display: flex;
	gap: 8px;
	flex-wrap: nowrap;
}

.sustainability-safe,
.sustainability-riskant,
.sustainability-unsafe {
	display: inline-flex;
	align-items: center;
	padding: 8px 14px;
	border-radius: 999px;
	font-size: 0.9rem;
	font-weight: 700;
}

.sustainability-safe {
	background: #dff0d0;
	color: #1b7a2f;
	border: none;
}

.sustainability-riskant {
	background: #f8ead0;
	color: #9a5c00;
	border: none;
}

.sustainability-unsafe {
	background: #f5dddd;
	color: #a42323;
	border: none;
}

	.show-products-toggle {
	display: flex;
	align-items: center;
	gap: 8px;

	padding: 10px 16px;

	background: white;
	color: var(--color-primary-dark);

	border: 1px solid var(--color-border);
	border-radius: 14px;

	font-weight: 600;
	font-size: 0.95rem;

	min-width: fit-content;
}

.show-products-toggle:hover {
	background: #fafafa;
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
		display: flex;
		flex-direction: row;
		justify-content: space-between;
		border-radius: 10px;
		padding: 12px;
		background-color: #ffffff;
		background-image: none;
	}

	.product-name {
		margin-top: 6px;
		font-size: 1rem;
		font-weight: 500;
		color: var(--color-primary-dark);
	}

	.product-meta-row {
		display: flex;
		gap: 16px;
		align-items: baseline;
		flex-wrap: wrap;
		color: var(--color-text);
	}

	.sustainability-row {
		display: flex;
		align-items: center;
		gap: 10px;
		margin-top: 6px;
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
	color: var(--color-text);
	}

.warnings-riskant{
color: #92400e;
}

.warnings-no{
		color: var(--color-secondary-dark);
	}


	.view-details-link {
		background: var(--color-bg);
		border: 1px solid var(--color-primary);
		padding: 6px 10px;
		font-size: 0.95rem;
		font-weight: 500;
		color: var(--color-primary);
		text-decoration: none;
		cursor: pointer;
		border-radius: 2rem;
	}

	.view-details-link:hover {
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




