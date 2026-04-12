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
		background: linear-gradient(90deg, #e8f5e4 0%, #ffffff 100%);
		color: #25561d;
		text-align: center;
		border: 1px solid #83c171;
		border-radius: 10px;
		padding: 10px 14px;
		font-weight: 600;
		cursor: pointer;
		box-shadow: 0 2px 8px rgba(37, 86, 29, 0.12);
		transition: background-color 0.2s ease, border-color 0.2s ease, box-shadow 0.2s ease, transform 0.2s ease;
	}

	#dropdownMenu:hover {
		background: linear-gradient(90deg, #d9edd2 0%, #f6fbf4 100%);
		border-color: #6aa85a;
		box-shadow: 0 4px 12px rgba(37, 86, 29, 0.2);
		transform: translateY(-1px);
	}

	#dropdownMenu:focus-visible {
		outline: none;
		border-color: #4d8f40;
		box-shadow: 0 0 0 3px rgba(131, 193, 113, 0.35);
	}
</style>