<script lang="ts">
    import NextIcon from "$lib/assets/next_icon.png";
    import PreviousIcon from "$lib/assets/previous_icon.png";

	type UploadedImage = {
		file: File;
		url: string;
	};

	type ImageCarouselProps = {
		images: UploadedImage[];
		onDelete?: (index: number) => void;
		onSubmit?: (images: UploadedImage[]) => void;
	};

	let { images, onDelete, onSubmit }: ImageCarouselProps = $props();

	let activeImageIndex = $state(0);

	function previousImage() {
		activeImageIndex = activeImageIndex === 0 ? images.length - 1 : activeImageIndex - 1;
	}

	function nextImage() {
		activeImageIndex = activeImageIndex === images.length - 1 ? 0 : activeImageIndex + 1;
	}

	function deleteActiveImage() {
		onDelete?.(activeImageIndex);

		if (activeImageIndex >= images.length - 1) {
			activeImageIndex = Math.max(images.length - 2, 0);
		}
	}
</script>

{#if images.length > 0}
	<div id="carouselContainer">
		<button type="button" class="carouselButton" onclick={previousImage}>
			<img src={PreviousIcon} alt="Vorige afbeelding" />
		</button>

		<div id="imagePreviewWrapper">
			<img
				id="previewImage"
				src={images[activeImageIndex].url}
				alt="Geselecteerde afbeelding"
			/>

			<button type="button" id="deleteButton" onclick={deleteActiveImage}>
				×
			</button>
		</div>

		<button type="button" class="carouselButton" onclick={nextImage}>
			<img src={NextIcon} alt="Volgende afbeelding" />
		</button>
	</div>

	<p id="imageCounter">
		{activeImageIndex + 1} / {images.length}
	</p>

	<button type="button" id="scanButton" onclick={() => onSubmit?.(images)}>
		Scannen
	</button>
{/if}

<style>
	#carouselContainer {
		margin-top: 24px;
		display: flex;
		align-items: center;
		justify-content: center;
		gap: 20px;
	}

	#imagePreviewWrapper {
		position: relative;
		width: 150px;
		height: 100px;
		border: 2px solid black;
		background: white;
	}

	#previewImage {
		width: 100%;
		height: 100%;
		object-fit: cover;
	}

	.carouselButton {
        width: 42px;
        height: 42px;
        padding: 0;
        border: none;
        border-radius: 50%;
        display: flex;
        align-items: center;
        justify-content: center;
        background: var(--color-primary);
        cursor: pointer;
        flex-shrink: 0;
    }

    .carouselButton img {
        width: 50%;
        height: 50%;
        object-fit: contain;
        pointer-events: none;
    }

	#deleteButton {
		position: absolute;
        top: -10px;
        right: -10px;
        width: 30px;
        height: 30px;
        padding: 0;
        border: none;
        border-radius: 50%;
        display: flex;
        align-items: center;
        justify-content: center;
        line-height: 1;
        font-size: 1.2rem;
        font-weight: bold;
        background: red;
        color: white;
        cursor: pointer;
        flex-shrink: 0;
	}

	#imageCounter {
		margin: 8px 0;
		font-weight: bold;
	}

	#scanButton {
		width: 100%;
		max-width: 320px;
		padding: 12px;
		border: none;
		border-radius: 6px;
		background: var(--color-primary);
		color: white;
		font-size: 1rem;
		font-weight: bold;
		cursor: pointer;
	}
</style>