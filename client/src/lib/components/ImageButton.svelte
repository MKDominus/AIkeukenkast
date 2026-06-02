<!--
@component

### ImageButton

---

#### Description

Reusable image-based button component.

Displays a styled button containing an image icon and delegates all click
behavior to the parent component.

---

#### Usage

```svelte
<ImageButton
	src={CameraIcon}
	onclick={openCamera}
/>
```

---

#### Props

| Prop | Type | Default | Description |
| ---- | ---- | ------- | ----------- |
| src | string | required | Image source displayed inside the button |
| width | string | `"100px"` | Width of the button |
| height | string | `"100px"` | Height of the button |
| imageWidth | string | `"75px"` | Width of the image inside the button |
| imageHeight | string | `"75px"` | Height of the image inside the button |
| color | string | `"var(--color-secondary)"` | Background color of the button |
| onclick | () => void | required | Callback executed when the button is clicked |

---

#### Events / Callbacks

| Name | Description |
| ---- | ----------- |
| onclick() | Fired when the button is clicked |

-->

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