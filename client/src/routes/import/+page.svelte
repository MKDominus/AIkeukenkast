<script lang="ts">
	import { goto } from "$app/navigation";
	import PhoneHeader from "$lib/components/PhoneHeader.svelte";
	import TitleCard from "$lib/components/TitleCard.svelte";
	import ProgressIndicator from "$lib/components/ProgressIndicator.svelte";
	import ImageButton from "$lib/components/ImageButton.svelte";
	import ImageCarousel from "$lib/components/ImageCarousel.svelte";
	import ErrorMessage from "$lib/components/ErrorMessage.svelte";

	import ThuishulpHeaderImage from "$lib/assets/Thuishulp header card.png"
	import CameraIcon from "$lib/assets/camera_icon.png"
	import GalleryIcon from "$lib/assets/photo_icon.png"
	import LoadingGif from "$lib/assets/loading.gif"
	import ImportCompleteIcon from "$lib/assets/importComplete_icon.png"

	let currentStep = $state(1);
	let totalSteps = 3;
	let errorMessage = $state("");
	let errorOccurred = $state(false);

	let uploadedImages = $state<
		{
			file: File;
			url: string;
		}[]
	>([]);

	function deleteImage(index: number) {
		URL.revokeObjectURL(uploadedImages[index].url);
		uploadedImages.splice(index, 1);
	}

	async function submitImages(images: { file: File; url: string }[]) {
		const formData = new FormData();

		for (const image of images) {
			formData.append("images", image.file);
		}

		try {
			currentStep = 2;
			errorOccurred = false;

			// stuur afbeelding naar backend
			const response = await fetch("http://localhost:5141/api/scans", {
				method: "POST",
				body: formData
			});

			if (!response.ok) {
				throw new Error("Afbeelding analyseren mislukt.");
			}

			const result = await response.json();
			currentStep = 3;

			setTimeout(() => {
				goto(`/scan_resultaten/${result[0]["id"]}`);
			}, 3000);

		} catch (error) {
			currentStep = 1;
			errorOccurred = true;
			errorMessage = error instanceof Error
				? error.message
				: "Er ging iets mis bij het analyseren.";
		}
	}

	let cameraInput: HTMLInputElement;
	let galleryInput: HTMLInputElement;

	function handleImageSelected(event: Event) {
		const input = event.currentTarget as HTMLInputElement;
		const files = input.files;

		if (!files || files.length === 0) return;

		for (const file of files) {
			if (!file.type.startsWith("image/")) {
				errorOccurred = true;
				errorMessage = `"${file.name}" is geen geldige afbeelding.`;

				continue;
			}

			uploadedImages.push({
				file,
				url: URL.createObjectURL(file)
			});
		}

		input.value = "";
	}
</script>

<PhoneHeader />
<img id="headerImage" src={ThuishulpHeaderImage} alt="Thuishulp Header Image">
<TitleCard title="Scan keukenkastje" />
<div id="importContainer">
	<ProgressIndicator steps={totalSteps} visualCurrentStep={currentStep} />
	<div id="stepContent">
		{#if currentStep === 1}
			<p class="instructionText_small">
				Maak een foto of selecteer een afbeelding uit uw galerij
			</p>
			<form id="imageForm" onsubmit={handleImageSelected}>
				<input
					bind:this={cameraInput}
					type="file"
					name="image"
					accept="image/*"
					capture="environment"
					hidden
					onchange={handleImageSelected}
				/>

				<input
					bind:this={galleryInput}
					type="file"
					name="image"
					accept="image/*"
					hidden
					onchange={handleImageSelected}
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
			<ImageCarousel images={uploadedImages} onDelete={deleteImage} onSubmit={submitImages} />
			{#if errorOccurred}
				<ErrorMessage message={errorMessage} />
			{/if}
		{:else if currentStep === 2}
			<img class="visualAidIcon" src={LoadingGif} alt="Bezig met verwerken" />
			<p class="instructionText">Afbeelding(en) Analyseren</p>
			{#if errorOccurred}
				<ErrorMessage message={errorMessage} />
			{/if}
		{:else if currentStep === 3}
			<img class="visualAidIcon" src={ImportCompleteIcon} alt="Importeren voltooid" />
			<p class="instructionText">Scannen voltooid!</p>
			<p class="instructionText_small">U wordt automatisch doorverwezen naar de resultatenpagina.</p>
		{/if}
	</div>
</div>



<style>
	
	.visualAidIcon {
		width: 150px;
		height: 150px;
		margin-bottom: 20px;
	}

	#stepContent {
		display: flex;
		justify-content: center;
		align-items: center;
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
		height: calc(100dvh - 200px);
		padding: 16px;
	}

	.instructionText {
		font-size: 1.2rem;
		font-weight: bold;
	}

	.instructionText_small {
		text-align: center;
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

