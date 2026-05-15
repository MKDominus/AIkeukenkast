<script lang="ts">
	import PhoneHeader from "$lib/components/PhoneHeader.svelte";
	import TitleCard from "$lib/components/TitleCard.svelte";
	import ScannedProduct from "$lib/components/ScannedProduct.svelte";

	import ThuishulpHeaderImage from "$lib/assets/Thuishulp header card.png"

    let titleCardTitle = $state("Scan resultaten");
</script>

<PhoneHeader />
<img id="headerImage" src={ThuishulpHeaderImage} alt="Thuishulp Header Image">
<div id="titleCardwrapper">
	<TitleCard title={titleCardTitle} />
</div>
<div id="scannedProductsContainer">
	<ScannedProduct
		productName="Ajax Eucalyptus"
		productType="Allesreiniger"
		riskLevel="Onveilig"
		warningLabels={[
			{ type: "ghs7", description: "Schadelijk, irriterend, allergische huidreactie" }
		]}
		image="https://static.ah.nl/dam/product/AHI_366f484d5a31642d524e4b4a4a316f4473504f757241?revLabel=1&rendition=400x400_JPG_Q85&fileType=binary"

	/>
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
	}

	@media (max-width: 768px) {
		#scannedProductsContainer {
			width: 100%;
			display: flex;
			flex-direction: column;
			align-items: stretch;
			justify-self: center;
			height: calc(100dvh - 250px);
		}
	}
</style>

