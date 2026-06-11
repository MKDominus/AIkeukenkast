<script lang="ts">
	import PhoneHeader from "$lib/components/PhoneHeader.svelte";
	import TitleCard from "$lib/components/TitleCard.svelte";
	import ScannedProduct from "$lib/components/ScannedProduct.svelte";
	import ErrorMessage from "$lib/components/ErrorMessage.svelte";
	import StyledButton from "$lib/components/StyledButton.svelte";

	import ThuishulpHeaderImage from "$lib/assets/Thuishulp header card.png";
	import TzorgIllustration from "$lib/assets/tzorg_illustratie.png";

	import type { PageData } from "./$types";

	let { data }: { data: PageData } = $props();
	console.log("Received scan results:", data);
</script>

<PhoneHeader />

<img
	id="headerImage"
	src={ThuishulpHeaderImage}
	alt="Thuishulp Header Image"
/>

<TitleCard title="Scan resultaten" />

<div id="scannedProductsContainer">
	{#if data.scanResults.length === 0}
		<ErrorMessage message="Geen producten herkend." />
		<p id="errorExplanation">Maak een duidelijkere foto of zet de product(en) rechtop in beeld</p>
		<img id="errorIllustration" src={TzorgIllustration} alt="Tzorg Illustratie">
		<div id="retryButtonContainer">
			<StyledButton 
				buttonTitle="Probeer opnieuw" 
				onclick={() => history.back()} 
				color="var(--color-primary)"
				width = "200px"
				height = "50px"
			/>
		</div>
	{:else}
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
	{/if}
	
</div>

<style>
	#retryButtonContainer {
		margin-top: 16px;
		display: flex;
		justify-content: center;
	}

	#errorExplanation {
		margin: 16px 0 16px 0;
		text-align: center;
	}

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