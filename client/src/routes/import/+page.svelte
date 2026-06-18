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
	let uploadConsentGiven = $state(false);
	let postalCode = $state("");

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
		if (!uploadConsentGiven) {
			currentStep = 1;
			errorOccurred = true;
			errorMessage = "Je moet toestemming geven om een foto te uploaden.";
			return;
		}

		const formData = new FormData();

		for (const image of images) {
			formData.append("images", image.file);
		}

		if (postalCode.trim().length > 0) {
			formData.append("postalCode", postalCode.trim());
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
			console.log("AI result:", result);
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
			<div class="uploadConsentContainer">
				<label class="uploadConsentCard" for="uploadConsentGiven">
					<div class="uploadConsentToggleRow">
						<input
							id="uploadConsentGiven"
							type="checkbox"
							class="uploadConsentCheckbox"
							bind:checked={uploadConsentGiven}
						/>
						<span class="customCheckbox" aria-hidden="true"></span>
						<span class="uploadConsentText">toestemming voor upload</span>
					</div>
				</label>
			</div>

			<div class="postalCodeContainer">
				<label class="postalCodeLabel" for="postalCode">Postcode</label>
				<input
					id="postalCode"
					type="text"
					inputmode="text"
					placeholder="Bijv. 1234AB"
					bind:value={postalCode}
					maxlength="20"
				/>
			</div>
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

	.uploadConsentContainer {
		display: flex;
		width: 100%;
		max-width: 420px;
		margin-top: 24px;
		margin-bottom: 12px;
	}

	.uploadConsentCard {
		display: flex;
		width: 100%;
		padding: 12px 14px;
		border: 1px solid var(--color-border);
		border-radius: 12px;
		background: var(--color-surface-primary-soft);
		cursor: pointer;
	}

	.uploadConsentToggleRow {
		display: flex;
		align-items: center;
		gap: 10px;
		width: 100%;
	}

	.uploadConsentCheckbox {
		position: absolute;
		opacity: 0;
		pointer-events: none;
	}

	.customCheckbox {
		width: 20px;
		height: 20px;
		border-radius: 6px;
		border: 2px solid var(--color-primary);
		background: var(--color-bg);
		display: inline-flex;
		align-items: center;
		justify-content: center;
		transition: background-color 0.2s ease, border-color 0.2s ease;
		flex-shrink: 0;
	}

	.customCheckbox::after {
		content: "";
		width: 6px;
		height: 11px;
		border: solid var(--color-bg);
		border-width: 0 2px 2px 0;
		transform: rotate(45deg) scale(0);
		transition: transform 0.15s ease;
	}

	.uploadConsentCheckbox:checked + .customCheckbox {
		background: var(--color-primary);
		border-color: var(--color-primary);
	}

	.uploadConsentCheckbox:checked + .customCheckbox::after {
		transform: rotate(45deg) scale(1);
	}

	.uploadConsentCheckbox:focus-visible + .customCheckbox {
		outline: 2px solid var(--color-primary-dark);
		outline-offset: 2px;
	}

	.uploadConsentText {
		color: var(--color-primary-dark);
		font-weight: 600;
		text-transform: lowercase;
	}

	.postalCodeContainer {
		display: flex;
		flex-direction: column;
		gap: 8px;
		width: 100%;
		max-width: 420px;
		margin-bottom: 12px;
	}

	.postalCodeLabel {
		font-weight: 600;
		color: var(--color-primary-dark);
	}

	#postalCode {
		padding: 10px 12px;
		border: 1px solid var(--color-primary);
		border-radius: 8px;
		font-size: 1rem;
		color: var(--color-text);
		background: var(--color-bg);
		width: 100%;
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

