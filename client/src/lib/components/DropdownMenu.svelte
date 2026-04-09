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
		background: gray;
		text-align: center;
		border-radius: 10px;
	}
</style>