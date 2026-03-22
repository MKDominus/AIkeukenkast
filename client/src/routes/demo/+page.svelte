<script lang="ts">
	import Header from '$lib/components/Header.svelte';
    import KpiStatistic from '$lib/components/KpiStatistic.svelte';
    import DropdownMenu from '$lib/components/DropdownMenu.svelte';
    import ScanCard from '$lib/components/ScanCard.svelte';
    import type { Scan } from '$lib/services/scanService';

    type MunicipalityDropDownItem = {
        label: string
        value: any
    }

    type Statistic = {
        title: string
        value: string
    }

    type Props = {
        data: {
            scans: Scan[];
        };
    };

    let { data }: Props = $props();

    let exampleMunicipalities: MunicipalityDropDownItem[] = [
        {label: "test", value: 1}, 
        {label: "test2", value: "testvalue2"}
    ]

    //Dummy data
    let statistics: Statistic[] = [
        {title: "Totale Scans", value: "16"},
        {title: "Producten Gescanned", value: "104"},
        {title: "Veiligheidsbeoordeling", value: "64%"},
        {title: "Gemiddelde Duurzaamheid", value: "78%"}
    ]

    function applyFilter(value: any){
        //do something here to apply the chosen filter on the list
        console.log(value)
    }
</script>

<Header></Header>
<div id="KpiStatisticsFlexBox">
    {#each statistics as statistic}
        <KpiStatistic statisticTitle={statistic.title} statistic={statistic.value}></KpiStatistic>
    {/each}
</div>

<div id="filtersContainer">
    <DropdownMenu dropdownTitle="Test" dropdownItems={exampleMunicipalities} itemChosenEvent={applyFilter}></DropdownMenu>
</div>

<div class="scansContainer">
    {#if data.scans.length === 0}
        <p>No scans found.</p>
    {:else}
        {#each data.scans as scan}
            <ScanCard {scan} />
        {/each}
    {/if}
</div>

<style>
    #KpiStatisticsFlexBox {
        display: flex;
        margin-top: 30px;
    }

    #filtersContainer {
        background-color: grey;
        padding: 10px;
        margin: 30px;
    }

	.scansContainer {
        padding: 10px;
        margin: 30px;
        width: calc(100% - 60px);
        box-sizing: border-box;
	}
</style>
