<script lang="ts">
	type ImageButtonProps = {
		src: string;
		width?: string;
		height?: string;
		imageWidth?: string;
		imageHeight?: string;
		color?: string;
		onclick: () => void;
	};

	let {
		src,
		width = '100px',
		height = '100px',
		imageWidth = '75px',
		imageHeight = '75px',
		color = 'var(--color-secondary)',
		onclick
	}: ImageButtonProps = $props();
</script>

<button
	type="button"
	onclick={onclick}
	style:width
	style:height
	style:background-color={color}
>
	<img
		src={src}
		alt=""
		style:width={imageWidth}
		style:height={imageHeight}
	/>
</button>

<style>
	button {
		border: none;
		padding: 0;
		display: flex;
		align-items: center;
		justify-content: center;
		cursor: pointer;
        border-radius: 21px;
        box-shadow: 1px 10px 12px -6px rgba(0,0,0,0.42);
        -webkit-box-shadow: 1px 10px 12px -6px rgba(0,0,0,0.42);
        -moz-box-shadow: 1px 10px 12px -6px rgba(0,0,0,0.42);
	}

	img {
		object-fit: contain;
		pointer-events: none;
	}
</style>