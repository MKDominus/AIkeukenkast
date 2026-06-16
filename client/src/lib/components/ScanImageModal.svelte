<script lang="ts">
	import { onDestroy } from 'svelte';
	import { getScanImageObjectUrl } from '$lib/services/scanImageService';

	type Props = {
		scanId: number;
		municipalityName?: string | null;
		onClose: () => void;
	};

	let { scanId, municipalityName, onClose }: Props = $props();
	let imageObjectUrl = $state<string | null>(null);
	let loading = $state(true);
	let error = $state<string | null>(null);

	async function loadImage() {
		loading = true;
		error = null;

		try {
			imageObjectUrl = await getScanImageObjectUrl(fetch, scanId);
		} catch (cause) {
			error = cause instanceof Error ? cause.message : 'Kon scan foto niet laden.';
		} finally {
			loading = false;
		}
	}

	loadImage();

	onDestroy(() => {
		if (imageObjectUrl) {
			URL.revokeObjectURL(imageObjectUrl);
		}
	});
</script>

<button
	type="button"
	class="ingredients-modal-backdrop"
	onclick={onClose}
	aria-label="Sluit scan foto"
></button>

<div class="scan-image-modal" role="dialog" aria-modal="true" aria-label="Scan foto">
	<button type="button" class="close-modal-button" onclick={onClose} aria-label="Sluiten">
		×
	</button>

	<h3 class="modal-product-name">Afbeelding scan</h3>
	<p class="modal-manufacturer">{municipalityName ?? 'Onbekende gemeente'}</p>

	<div class="scan-image-stage">
		{#if loading}
			<p class="scan-image-loading">Foto laden...</p>
		{:else if error}
			<p class="scan-image-error">{error}</p>
		{:else if imageObjectUrl}
			<img
				src={imageObjectUrl}
				alt={`Scan foto van ${municipalityName ?? 'onbekende gemeente'}`}
				class="scan-image-preview"
			/>
		{/if}
	</div>
</div>

<style>
	.ingredients-modal-backdrop {
		position: fixed;
		inset: 0;
		border: none;
		padding: 0;
		background: rgba(17, 24, 39, 0.28);
		backdrop-filter: blur(4px);
		-webkit-backdrop-filter: blur(4px);
		z-index: 40;
	}

    .modal-product-name{
        color: var(--color-text);
    }

    .modal-manufacturer{
        color: var(--color-text-muted);
    }

	.scan-image-modal {
		position: fixed;
		top: 50%;
		left: 50%;
		transform: translate(-50%, -50%);
		z-index: 41;
		width: min(960px, calc(100vw - 32px));
		height: min(72vh, calc(100vh - 32px));
		overflow: hidden;
		background: var(--color-bg);
		border-radius: 14px;
		box-shadow: 0 18px 40px rgba(65, 20, 71, 0.18);
		padding: 14px;
		display: flex;
		flex-direction: column;
		box-sizing: border-box;
	}

	.close-modal-button {
		position: absolute;
		top: 10px;
		right: 12px;
		border: none;
		background: transparent;
		font-size: 1.5rem;
		line-height: 1;
		color: var(--color-primary);
		cursor: pointer;
	}

	.scan-image-preview {
		display: block;
		width: 100%;
		height: 100%;
		object-fit: contain;
		border-radius: 14px;
		background: #fff;
		box-sizing: border-box;
	}

	.scan-image-stage {
		flex: 1;
		min-height: 0;
		display: flex;
		align-items: center;
		justify-content: center;
		margin-top: 10px;
		border-radius: 14px;
		background: #fff;
		padding: 12px;
		box-sizing: border-box;
		overflow: hidden;
	}

	.scan-image-loading,
	.scan-image-error {
		margin: 0;
		color: var(--color-text-muted);
		text-align: center;
		max-width: 100%;
	}

	.scan-image-error {
		color: #b91c1c;
	}
</style>