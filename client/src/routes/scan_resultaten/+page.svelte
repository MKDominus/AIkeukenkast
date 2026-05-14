<script lang="ts">
	import PhoneHeader from "$lib/components/PhoneHeader.svelte";
	import TitleCard from "$lib/components/TitleCard.svelte";
	import ScannedProduct from "$lib/components/ScannedProduct.svelte";

	import ThuishulpHeaderImage from "$lib/assets/Thuishulp header card.png"

    let titleCardTitle = $state("Scan resultaten");
</script>

<PhoneHeader />
<img id="headerImage" src={ThuishulpHeaderImage} alt="Thuishulp Header Image">
<div id="importContainer">
	<TitleCard title={titleCardTitle} />
	<ScannedProduct
		productName="Appelssssssssssssssssssssss"
		productType="Fruit"
		riskLevel="Veilig"
		warningLabels={[]}
		image=""
	/>
</div>



<style>
	#headerImage {
		width: 100%;
		height: auto;
	}

	#importContainer {
		width: 40%;
		display: flex;
		flex-direction: column;
		align-items: stretch;
		justify-self: center;
		height: calc(100dvh - 200px);
		padding: 16px;
	}

	@media (max-width: 768px) {
		#importContainer {
			width: 100%;
			display: flex;
			flex-direction: column;
			align-items: stretch;
			justify-self: center;
			height: calc(100dvh - 185px);
		}
	}
</style>

