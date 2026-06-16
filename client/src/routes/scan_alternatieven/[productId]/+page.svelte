<script lang="ts">
	import PhoneHeader from "$lib/components/PhoneHeader.svelte";
	import TitleCard from "$lib/components/TitleCard.svelte";
    import StyledButton from "$lib/components/StyledButton.svelte";
    import AlternativeProduct from "$lib/components/AlternativeProduct.svelte";
	import ThuishulpHeaderImage from "$lib/assets/Thuishulp header card.png"
	import TzorgDefault from "$lib/assets/tzorgDefault.png"

	import type { PageData } from "./$types";

	let { data }: { data: PageData } = $props();

	const alternatives = data.alternatives;

</script>

<PhoneHeader />
<img id="headerImage" src={ThuishulpHeaderImage} alt="Thuishulp Header Image">
<TitleCard title="Alternatieven" />
<div id="alternativesContainer">
	<StyledButton 
        buttonTitle="Terug"
        width="100px"
        height="30px"
        onclick={() => history.back()}
    />

    <div id="alternativeProductsWrapper">
        {#each alternatives as alternative}
            <AlternativeProduct 
                imageURL={alternative.imageURL || TzorgDefault}
                productName={alternative.productName}
                productType={alternative.productType}
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

