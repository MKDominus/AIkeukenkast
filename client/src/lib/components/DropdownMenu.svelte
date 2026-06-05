<!--
@component

### DropdownMenu

---

#### Description

Custom styled dropdown/select component.

Renders a dropdown menu with selectable items and emits the selected item's value
through a callback function provided by the parent component.

---

#### Usage

```svelte
<script lang="ts">
	const items = [
		{ label: "Option 1", value: "option1" },
		{ label: "Option 2", value: "option2" }
	];

	function handleSelection(value: string) {
		console.log(value);
	}
</script>

<DropdownMenu
	dropdownTitle="Selecteer een optie"
	dropdownItems={items}
	itemChosenEvent={handleSelection}
/>
```

---

#### Props

| Prop | Type | Description |
| ---- | ---- | ----------- |
| dropdownTitle | string | Placeholder/default disabled option shown before selection |
| dropdownItems | Item[] | Array of dropdown items |
| itemChosenEvent | (value: any) => void | Callback executed when an item is selected |
| variant | 'default' | 'primary' | Optional variant for styling |
##### Item Structure

```ts
type Item = {
	label: string;
	value: any;
}
```

---

#### Events / Callbacks

| Name | Description |
| ---- | ----------- |
| itemChosenEvent(value) | Fired when the selected option changes |

-->

<script lang="ts">
	type Item = {
		label: string;
		value: any;
	};

	type Props = {
		dropdownTitle: string;
		dropdownItems: Item[];
		itemChosenEvent: (value: any) => void;
		variant?: 'default' | 'primary';
	};

	let {
		dropdownTitle,
		dropdownItems,
		itemChosenEvent,
		variant = 'default'
	}: Props = $props();

	function handleChange(e: Event) {
		const value = (e.target as HTMLSelectElement).value;

		const selectedItem = dropdownItems.find(
			(item) => String(item.value) === value
		);

		if (selectedItem) {
			itemChosenEvent(selectedItem.value);
		}
	}
</script>

<select
	id="dropdownMenu"
	class:primary={variant === 'primary'}
	onchange={handleChange}
>
	<option selected disabled>{dropdownTitle}</option>

	{#each dropdownItems as item}
		<option value={item.value}>
			{item.label}
		</option>
	{/each}
</select>

<style>
	#dropdownMenu {
		appearance: none;
		background: var(--color-secondary);
		color: var(--color-bg);
		width: 80px;
		text-align: center;
		border: 1px solid var(--color-secondary-dark);
		border-radius: 2rem;
		padding: 8px 12px;
		min-width: 140px;
		height: 38px;
		font-size: 0.8rem;
		font-weight: 600;
		cursor: pointer;
		box-shadow: 0 2px 8px rgba(102, 39, 115, 0.16);
		transition:
			background-color 0.2s ease,
			border-color 0.2s ease,
			box-shadow 0.2s ease,
			transform 0.2s ease;
	}

	#dropdownMenu:hover {
		background: var(--color-secondary-dark);
		border-color: var(--color-secondary-dark);
		box-shadow: 0 4px 12px rgba(65, 20, 71, 0.22);
		transform: translateY(-1px);
	}

	#dropdownMenu:focus-visible {
		outline: none;
		border-color: var(--color-primary-dark);
		box-shadow: 0 0 0 3px rgba(102, 39, 115, 0.28);
	}

	#dropdownMenu.primary {
		background: var(--color-primary);
		border-color: var(--color-primary-dark);
		color: var(--color-bg);
		font-size: 0.8rem;
	}

	#dropdownMenu.primary:hover {
		background: var(--color-primary-dark);
		border-color: var(--color-primary-dark);
		box-shadow: 0 4px 12px rgba(65, 20, 71, 0.28);
	}

	#dropdownMenu option {
		background: var(--color-bg);
		color: var(--color-primary-dark);
	}
</style>