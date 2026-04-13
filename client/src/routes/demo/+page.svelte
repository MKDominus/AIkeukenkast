<script lang="ts">
	import Header from '$lib/components/Header.svelte';
    import KpiStatistic from '$lib/components/KpiStatistic.svelte';
    import DropdownMenu from '$lib/components/DropdownMenu.svelte';
    import MunicipalitiesJson from '$lib/assets/Municipalities.json';
    import PieChart from '$lib/components/PieChart.svelte';
    import ScanCard from '$lib/components/ScanCard.svelte';
    import type { Scan, ScanStats } from '$lib/services/scanService';

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
            stats: ScanStats;
        };
    };

    let { data }: Props = $props();

    let exampleMunicipalities: MunicipalityDropDownItem[] = [
        {label: "test", value: 1}, 
        {label: "test2", value: "testvalue2"}
    ]

    let statistics: Statistic[] = $derived([
        {title: "Totale Scans", value: data.stats.totalScans.toString()},
        {title: "Producten Gescanned", value: data.stats.productsScanned.toString()},
        {title: "Veiligheidsbeoordeling", value: `${Math.round(data.stats.averageSafety)}%`},
        {title: "Gemiddelde Duurzaamheid", value: `${Math.round(data.stats.averageSustainability)}%`}
    ])

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

    <PieChart labels={["vloerReiniger", "kalk", "toiletreiniger"]} values={[10, 50, 40]} pieChartTitle="Product Categorieën"></PieChart>
</div>

<div id="filtersContainer">
    <DropdownMenu dropdownTitle="Gemeentes" dropdownItems={MunicipalitiesJson} itemChosenEvent={applyFilter}></DropdownMenu>
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
        flex-wrap: wrap;
        gap: 16px;
        align-items: stretch;
        margin-top: 30px;
        margin-left: 30px;
        margin-right: 30px;
    }

    #filtersContainer {
        background-color: var(--color-primary);
        border: 1px solid var(--color-primary-dark);
        border-left: 4px solid var(--color-secondary-dark);
        padding: 10px;
        margin: 30px;
        border-radius: 10px;
    }

	.scansContainer {
        padding: 10px;
        margin: 30px;
        width: calc(100% - 60px);
        box-sizing: border-box;
	}

    
</style>
