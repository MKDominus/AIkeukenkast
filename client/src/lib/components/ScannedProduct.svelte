<script lang="ts">
	import TzorgDefault from "$lib/assets/tzorgDefault.png";
    import StyledButton from "$lib/components/StyledButton.svelte";

	type RiskLevel = "Veilig" | "Riskant" | "Onveilig";

	type Props = {
		image?: string;
		productName: string;
		productType: string;
		warningLabels?: string[];
		riskLevel: RiskLevel;
	};

	let {
		image,
		productName,
		productType,
		warningLabels = [],
		riskLevel
	}: Props = $props();

	const riskColors: Record<RiskLevel, string> = {
		Veilig: "var(--color-secondary)",
		Riskant: "orange",
		Onveilig: "red"
	};

	const riskColor = $derived(riskColors[riskLevel]);
</script>

<div class="productItem" style:border-color={riskColor}>
	<div class="imageContainer">
        {#if image}
            <img
                src={image}
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

			<div
				class="riskBadge"
				style:background-color={riskColor}
			>
				{riskLevel}
			</div>
		</div>

		<div class="warningContainer">
			{#if warningLabels.length > 0}
				{#each warningLabels as warning}
					<p>{warning}</p>
				{/each}
			{:else}
				<p class="noWarnings">Geen gevaren info</p>
			{/if}
		</div>
	</div>
</div>
<div class="buttonContainer">
    <StyledButton
        buttonTitle="Extra info"
        width="100%"
        height="30px"
        color="var(--color-primary)"
        onclick={() => alert("Meer info over het product")}
    />

    <StyledButton
        buttonTitle="Alternatieven"
        width="100%"
        height="30px"
        onclick={() => alert("Meer info over het product")}
    />
</div>


<style>
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

        border-bottom: 1px solid var(--color-primary);

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
        line-height: 1;

        font-weight: 800;

        color: var(--color-primary);

        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
    }

    .titleBlock p {
        margin: 2px 0 0 0;

        font-size: 0.8rem;
        line-height: 1;

        color: var(--color-primary);

        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
    }

    .riskBadge {
        min-width: 80px;
        height: 24px;

        padding: 0 10px;

        display: flex;
        align-items: center;
        justify-content: center;

        border-bottom-left-radius: 8px;

        color: white;

        font-size: 0.8rem;
        font-weight: 800;

        box-sizing: border-box;

        flex: 0 0 auto;
    }

    .warningContainer {
        flex: 1;
        margin: 0 0 4px 0;

        padding: 6px;

        overflow-y: auto;

        box-sizing: border-box;
    }

    .warningContainer p {

        font-size: 0.72rem;
        line-height: 1.1;

        color: var(--color-primary);
    }

    .noWarnings {
        height: 100%;
        font-size: 1.3rem;

        display: flex;
        align-items: center;
        justify-content: center;

        text-align: center;
    }

    .buttonContainer {
        display: flex;
        gap: 10px;
    }
</style>