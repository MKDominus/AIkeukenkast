<script lang="ts">
	import TzorgDefault from "$lib/assets/tzorgDefault.png"
    
    type AlternativeProduct = {
        imageURL?: string;
        productName: string;
        productType: string;
    }

	let {
		imageURL,
		productName,
		productType
	}: AlternativeProduct = $props()

</script>

<div id="alternativeProductContainer"> 
    <div class="productItem" style:border-color=var(--color-secondary)>
        <div class="imageContainer">
            {#if imageURL}
                <img
                    src={imageURL}
                    alt={productName}
                />
            {:else}
                <img
                    src={TzorgDefault}
                    alt="geen product afbeelding beschikbaar"
                />
            {/if}
        </div>

        <div class="infoContainer">
            <div class="headerRow">
                <div class="titleBlock">
                    <h3>{productName}</h3>
                    <p>{productType}</p>
                </div>
            </div>
        </div>
    </div>
</div>



<style>
    #alternativeProductContainer{
        margin-bottom: 30px;
    }

	.productItem {
        height: 100px;
        margin-bottom: 10px;

        display: flex;
        gap: 0;

        box-sizing: border-box;

        box-shadow: 1px 10px 12px -6px rgba(0,0,0,0.42);
        -webkit-box-shadow: 1px 10px 12px -6px rgba(0,0,0,0.42);
        -moz-box-shadow: 1px 10px 12px -6px rgba(0,0,0,0.42);
    }

    .imageContainer {
        width: 100px;
        height: 100%;

        flex: 0 0 100px;

        display: flex;
        align-items: center;
        justify-content: center;

        background: white;

        border: 4px solid;

        border-color: inherit;

        box-sizing: border-box;
    }

    .imageContainer img {
        width: 90px;
        height: 90px;

        object-fit: contain;
    }

    .infoContainer {
        flex: 1;
        min-width: 0;
        height: 100%;

        display: flex;
        flex-direction: column;

        background: white;

        border-top: 4px solid;
        border-right: 4px solid;
        border-bottom: 4px solid;

        border-color: inherit;

        box-sizing: border-box;

        color: var(--color-primary);
    }

    .headerRow {
        height: fit-content;


        display: flex;
        align-items: flex-start;
        justify-content: space-between;

        box-sizing: border-box;

        min-width: 0;
        overflow: hidden;
    }

    .titleBlock {
        padding: 4px 6px;

        flex: 1;
        min-width: 0;

        overflow: hidden;
    }

    .titleBlock h3 {
        margin: 0;

        font-size: 0.8rem;
        line-height: 1.3;

        font-weight: 800;

        color: var(--color-primary);

        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
    }

    .titleBlock p {
        margin: 2px 0 0 0;

        font-size: 0.8rem;
        line-height: 1.2;

        color: var(--color-primary);

        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
    }
</style>