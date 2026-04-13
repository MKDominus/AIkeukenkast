<!--
@component
A dropdown menu component that displays a title and a list of selectable items.

Props
- dropdownTitle (string): Title displayed on the dropdown button
- dropdownItems (Item): List of items to render in the dropdown
-----
Events
- itemChosenEvent: Fired when a user selects an item. 
the value of the chosen item is passed on with the event
-----
Usage
```svelte
	let exampleMunicipalities = [{label: Amsterdam, value: amsterdam}, ...]

	function applyFilter(value){
		console.log(value)
	}

	<DropdownMenu dropdownTitle="Test" dropdownItems={exampleMunicipalities} itemChosenEvent={applyFilter}/>
```
 

-->

<script lang="ts">

	type Item = {
		label: string;
		value: any;
	}

	type Props = {
		dropdownTitle: string;
		dropdownItems: Item[];
		itemChosenEvent: (value: any) => void;
	}

	let {
		dropdownTitle,
		dropdownItems,
		itemChosenEvent
	}: Props = $props();

	function handleChange(e: Event) {
		const value = (e.target as HTMLSelectElement).value;

		const selectedItem = dropdownItems.find(item => item.value === value);
		if (selectedItem) {
			itemChosenEvent(selectedItem.value);
		}
	}
</script>

<select id="dropdownMenu" onchange={handleChange}>
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
		text-align: center;
		border: 1px solid var(--color-secondary-dark);
		border-radius: 2rem;
		padding: 10px 14px;
		font-weight: 600;
		cursor: pointer;
		box-shadow: 0 2px 8px rgba(102, 39, 115, 0.16);
		transition: background-color 0.2s ease, border-color 0.2s ease, box-shadow 0.2s ease, transform 0.2s ease;
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

	#dropdownMenu option {
		background: var(--color-bg);
		color: var(--color-primary-dark);
	}
</style>