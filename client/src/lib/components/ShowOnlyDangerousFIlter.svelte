<script lang="ts">
	type FilterMode = 'all' | 'safe' | 'riskant' | 'unsafe';

	type Props = {
		itemChosenEvent?: (value: FilterMode) => void;
	}

	let { itemChosenEvent }: Props = $props();

	function handleChange(e: Event) {
		const value = (e.target as HTMLSelectElement).value as FilterMode;
		itemChosenEvent?.(value);
	}
</script>

<select id="dangerousFilter" onchange={handleChange}>
	<option value="all">laat alle scans zien</option>
	<option value="safe">laat alleen volledig veilige scans zien</option>
	<option value="riskant">laat scans met riskante producten zien</option>
	<option value="unsafe">laat scans met onveilige producten zien</option>
</select>

<style>
	#dangerousFilter {
		appearance: none;
		background: var(--color-secondary);
		color: var(--color-primary-dark);
		text-align: center;
		border: 1px solid var(--color-secondary-dark);
		border-radius: 2rem;
		padding: 10px 14px;
		font-weight: 600;
		font-size: 0.8rem;
		width: 160px;
		cursor: pointer;
		box-shadow: 0 2px 8px rgba(102, 39, 115, 0.16);
		transition: background-color 0.2s ease, border-color 0.2s ease, box-shadow 0.2s ease, transform 0.2s ease;
	}

	#dangerousFilter:hover {
		background: var(--color-secondary-dark);
		border-color: var(--color-secondary-dark);
		box-shadow: 0 4px 12px rgba(65, 20, 71, 0.22);
		transform: translateY(-1px);
	}

	#dangerousFilter:focus-visible {
		outline: none;
		border-color: var(--color-primary-dark);
		box-shadow: 0 0 0 3px rgba(102, 39, 115, 0.28);
	}

	#dangerousFilter option {
		background: var(--color-bg);
		color: var(--color-primary-dark);
	}
</style>
