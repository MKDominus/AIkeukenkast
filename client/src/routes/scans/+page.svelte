<script lang="ts">
	import ScanCard from '$lib/components/ScanCard.svelte';
	import type { Scan } from '$lib/services/scanService';

	type Props = {
		data: {
			scans: Scan[];
		};
	};

	let { data }: Props = $props();
</script>

<h1>Scans</h1>

{#if data.scans.length === 0}
	<p>No scans found.</p>
{:else}
	{#each data.scans as scan}
		<ScanCard {scan} />
	{/each}
{/if}
