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
	type StyledButtonProps = {
		buttonTitle: string;
		width?: string;
		height?: string;
		color?: string;
		onclick: () => void;
	};

	let {
		buttonTitle,
		width = 'fit-content',
		height = 'fit-content',
		color = 'var(--color-secondary)',
		onclick
	}: StyledButtonProps = $props();
</script>

<button
	type="button"
	onclick={onclick}
	style:width
	style:height
	style:background-color={color}
>
	{buttonTitle}
</button>

<style>
	button {
		border: none;
		display: flex;
		align-items: center;
		justify-content: center;
		cursor: pointer;
        color: white;
        font-weight: 600;
        font-size: 0.9rem;
        border-radius: 21px;
        box-shadow: 1px 10px 12px -6px rgba(0,0,0,0.42);
        -webkit-box-shadow: 1px 10px 12px -6px rgba(0,0,0,0.42);
        -moz-box-shadow: 1px 10px 12px -6px rgba(0,0,0,0.42);
	}
</style>