<script lang="ts">
	import PhoneHeader from "$lib/components/PhoneHeader.svelte";
	import TitleCard from "$lib/components/TitleCard.svelte";
	import ScannedProduct from "$lib/components/ScannedProduct.svelte";

	import ThuishulpHeaderImage from "$lib/assets/Thuishulp header card.png";

	import type { PageData } from "./$types";

	let { data }: { data: PageData } = $props();
</script>

<PhoneHeader />

<img
	id="headerImage"
	src={ThuishulpHeaderImage}
	alt="Thuishulp Header Image"
/>

<div id="titleCardwrapper">
	<TitleCard title="Scan resultaten" />
</div>

<div id="scannedProductsContainer">
	{#each data.scanResults as product}
		<ScannedProduct
			imageURL={product.imageURL}
			productName={product.productName}
			productType={product.productType}
			productId={product.productId}
			warningLabels={product.warningLabels}
			riskLevel={product.riskLevel}
			dangers={product.dangers}
			precautions={product.precautions}
			alternatives={product.alternatives}
		/>
	{/each}
</div>

<style>
	#titleCardwrapper {
		padding: 16px 16px 0 16px;
	}

	#headerImage {
		width: 100%;
		height: auto;
	}

	#scannedProductsContainer {
		width: 40%;

		display: flex;
		flex-direction: column;
		align-items: stretch;

		justify-self: center;

		height: calc(100dvh - 200px);

		padding: 16px;

		overflow-y: auto;

		box-sizing: border-box;
	}

	@media (max-width: 768px) {
		#scannedProductsContainer {
			width: 100%;
			height: calc(100dvh - 250px);
		}
	}
</style>