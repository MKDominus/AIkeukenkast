<script lang="ts">
	import PhoneHeader from "$lib/components/PhoneHeader.svelte";
	import TitleCard from "$lib/components/TitleCard.svelte";
	import ProgressIndicator from "$lib/components/ProgressIndicator.svelte";
	import ImageButton from "$lib/components/ImageButton.svelte";

	import ThuishulpHeaderImage from "$lib/assets/Thuishulp header card.png"
	import CameraIcon from "$lib/assets/camera_icon.png"
	import GalleryIcon from "$lib/assets/photo_icon.png"
	import LoadingGif from "$lib/assets/loading.gif"

	let currentStep = $state(1);

	let cameraInput: HTMLInputElement;
	let galleryInput: HTMLInputElement;

	function submitForm() {
		const form = document.getElementById("imageForm") as HTMLFormElement;
		form.requestSubmit();
	}

	function handleSubmit(event: SubmitEvent) {
		event.preventDefault();

		const formData = new FormData(event.currentTarget as HTMLFormElement);

		console.log("Submitted image:", formData.get("image"));
	}
</script>

<PhoneHeader />
<img id="headerImage" src={ThuishulpHeaderImage} alt="Thuishulp Header Image">
<div id="importContainer">
	<TitleCard title="Scan keukenkastje" />
	<ProgressIndicator steps={3} visualCurrentStep={currentStep} />
	<div id="stepContent">
		{#if currentStep === 1}
			<p id="instructionText">
				Maak een foto of selecteer een afbeelding uit uw galerij
			</p>
			<form id="imageForm" onsubmit={handleSubmit}>
				<input
					bind:this={cameraInput}
					type="file"
					name="image"
					accept="image/*"
					capture="environment"
					hidden
					onchange={submitForm}
				/>

				<input
					bind:this={galleryInput}
					type="file"
					name="image"
					accept="image/*"
					hidden
					onchange={submitForm}
				/>

				<div id="buttonsContainer">
					<ImageButton
						src={CameraIcon}
						onclick={() => cameraInput.click()}
					/>

					<div class="divider">
						<span>of</span>
					</div>

					<ImageButton
						src={GalleryIcon}
						onclick={() => galleryInput.click()}
					/>
				</div>
			</form>
		{:else if currentStep === 2}

		{:else if currentStep === 2}

		{/if}
		
	</div>
</div>



<style>
	#stepContent {
		display: flex;
		justify-content: center;
		align-items: center;
		margin: 16px;
		flex: 1;
		flex-direction: column;
	}

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
		height: calc(100dvh - 800px);
	}

	#instructionText {
		font-size: 1.5rem;
	}

	#buttonsContainer {
		margin-top: 20px;
		display: flex;
		align-items: center;
		gap: 30px;
	}

	.divider {
		display: flex;
		flex-direction: column;
		align-items: center;
		justify-content: center;
		height: 100px; 
	}

	.divider::before,
	.divider::after {
		content: "";
		width: 2px;
		flex: 1;
		background-color: #000000; /* line color */
	}

	.divider span {
		font-size: 1.2rem;
		padding: 6px 0;
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

		#instructionText {
			font-size: 1rem;
		}
	}
</style>

