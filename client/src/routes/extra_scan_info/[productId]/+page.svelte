<script lang="ts">
	import PhoneHeader from "$lib/components/PhoneHeader.svelte";
	import TitleCard from "$lib/components/TitleCard.svelte";
    import StyledButton from "$lib/components/StyledButton.svelte";
    import { page } from "$app/state";
	import { getProductById } from "$lib/services/scanResultsService";
    import type { ScannedProduct } from "$lib/stores/thuishulpScanResultaten.svelte";

    import TzorgDefault from "$lib/assets/tzorgDefault.png"

    import type { PageData } from "./$types";

	let { data }: { data: PageData } = $props();

	const product = data.product;
	
	import ThuishulpHeaderImage from "$lib/assets/Thuishulp header card.png"

</script>

<PhoneHeader />
<img id="headerImage" src={ThuishulpHeaderImage} alt="Thuishulp Header Image">
<TitleCard title="Extra info" />
<div id="extraInfoContainer">
	<StyledButton 
        buttonTitle="Terug"
        width="100px"
        height="30px"
        onclick={() => history.back()}
    />
    <div id="productImageContainer">
        {#if product?.imageURL}
            <img id="productImage" src={product.imageURL} alt={product.productName} />
        {:else}
            <img id="productImage" src={TzorgDefault} alt="Geen product afbeelding beschikbaar" />
        {/if}
    </div>

    <h1 id="productName">{product?.productName}</h1>
    <p id="productType">{product?.productType}</p>
    <hr id="divider">
    <h2 class="sectionText">Gevaren</h2>
    {#if product?.dangers && product.dangers.length > 0}
        <ul>
            {#each product.dangers as danger}
                <li>{danger}</li>
            {/each}
        </ul>
    {:else}
        <p>Geen gevaren informatie beschikbaar</p>
    {/if}

    <h2 class="sectionText">Voorzorgsmaatregelen</h2>
    {#if product?.precautions && product.precautions.length > 0}
        <ul>
            {#each product.precautions as precaution}
                <li>{precaution}</li>
            {/each}
        </ul>
    {:else}
        <p>Geen voorzorgsmaatregelen informatie beschikbaar</p>
    {/if}
</div>



<style>
    ul {
        list-style: disc;
        padding-left: 20px;
    }

    .sectionText {
        font-size: 1.2rem;
        font-weight: bold;
        color: var(--color-primary);
        margin: 10px 0 10px 0;
    }

    #divider {
        margin: 10px 0 0 0;
        border: 2px solid var(--color-primary);
    }

    #productType {
        font-size: 1rem;
        color: var(--color-primary);
        word-wrap: break-word;
    }

    #productName {
        font-size: 1.3rem;
        margin-top: 16px;
        color: var(--color-primary);
        word-wrap: break-word;
    }

    #productImageContainer {
        display: flex;
        justify-content: center;
        margin-top: 30px;
        min-height: 115px;
    }

    #productImage {
        width: 115px;
        height: 115px;
        object-fit: contain;
    }

	#titleCardwrapper {
		padding: 16px 16px 0 16px;
	}

	#headerImage {
		width: 100%;
		height: auto;
	}

	#extraInfoContainer {
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
		#extraInfoContainer {
			width: 100%;
			display: flex;
			flex-direction: column;
			align-items: stretch;
			justify-self: center;
			height: calc(100dvh - 250px);
		}
	}
</style>

