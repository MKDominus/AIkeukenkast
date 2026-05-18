<script lang="ts">
	import PhoneHeader from "$lib/components/PhoneHeader.svelte";
	import TitleCard from "$lib/components/TitleCard.svelte";
    import StyledButton from "$lib/components/StyledButton.svelte";
    import AlternativeProduct from "$lib/components/AlternativeProduct.svelte";

    import { page } from "$app/state";
	import { getProductById } from "$lib/services/scanResultsService";
    import type { ScannedProduct } from "$lib/stores/thuishulpScanResultaten.svelte";


	const productId = $derived(Number(page.params.productId));

	const product = $derived(
		getProductById(productId) as ScannedProduct | undefined
	);
	
	import ThuishulpHeaderImage from "$lib/assets/Thuishulp header card.png"

</script>

<PhoneHeader />
<img id="headerImage" src={ThuishulpHeaderImage} alt="Thuishulp Header Image">
<div id="titleCardwrapper">
	<TitleCard title="Alternatieven" />
</div>
<div id="alternativesContainer">
	<StyledButton 
        buttonTitle="Terug"
        width="100px"
        height="30px"
        onclick={() => history.back()}
    />

    <div id="alternativeProductsWrapper">
        {#each product?.alternatives as alternative}
            <AlternativeProduct 
                imageURL={alternative.imageURL}
                productName={alternative.productName || "Product naam niet beschikbaar"}
                productType={alternative.productType || "Product type niet beschikbaar"}
            />
        {/each}
    </div>
    


</div>



<style>
    #alternativeProductsWrapper {
        margin-top: 16px;
    }

    #titleCardwrapper {
		padding: 16px 16px 0 16px;
	}

	#headerImage {
		width: 100%;
		height: auto;
	}

	#alternativesContainer {
		width: 40%;
		display: flex;
		flex-direction: column;
		align-items: stretch;
		justify-self: center;
		height: calc(100dvh - 200px);
		padding: 16px;
		overflow-y: auto;
	}

	@media (max-width: 768px) {
		#alternativesContainer {
			width: 100%;
			display: flex;
			flex-direction: column;
			align-items: stretch;
			justify-self: center;
			height: calc(100dvh - 250px);
		}
	}
</style>

