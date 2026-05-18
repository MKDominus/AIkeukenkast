<!--
	@component

	### StyledButton

	---

	#### Description

	Reusable styled button component.

	Displays a rounded button with customizable dimensions and background color.
	All click behavior is delegated to the parent component.

	---

	#### Usage

	```svelte
	<StyledButton
	buttonTitle="Scannen"
	onclick={handleScan}
	/>
	```

	---

	#### Props

	| Prop | Type | Default | Description |
	| ---- | ---- | ------- | ----------- |
	| buttonTitle | string | required | Text displayed inside the button |
	| width | string | `"fit-content"` | Width of the button |
	| height | string | `"fit-content"` | Height of the button |
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
		isDisabled?: boolean;
		onclick: () => void;
	}

	let {
		buttonTitle,
		width = 'fit-content',
		height = 'fit-content',
		color = 'var(--color-secondary)',
		isDisabled = false,
		onclick
	}: StyledButtonProps = $props()
</script>

<button
	type="button"
	onclick={onclick}
	style:width
	style:height
	style:background-color={isDisabled ? "gray" : color}
	disabled={isDisabled}
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