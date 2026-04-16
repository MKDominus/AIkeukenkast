<script lang="ts">
	import { onMount, tick } from 'svelte';
	import Header from '$lib/components/Header.svelte';
    import KpiStatistic from '$lib/components/KpiStatistic.svelte';
    import DropdownMenu from '$lib/components/DropdownMenu.svelte';
    import ShowOnlyDangerousFIlter from '$lib/components/ShowOnlyDangerousFIlter.svelte';
    import MunicipalitiesJson from '$lib/assets/Municipalities.json';
    import PieChart from '$lib/components/PieChart.svelte';
    import NetherlandsMap from '$lib/components/NetherlandsMap.svelte';
    import ScanCard from '$lib/components/ScanCard.svelte';
    import type { Scan, ScanStats } from '$lib/services/scanService';
    import type { ProductCategoryCount } from '$lib/services/productService';

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
            categoryCounts: ProductCategoryCount[];
        };
    };

    let { data }: Props = $props();

    let statistics: Statistic[] = $derived([
        {title: "Totale Scans", value: data.stats.totalScans.toString()},
        {title: "Producten Gescanned", value: data.stats.productsScanned.toString()},
        {title: "Veiligheidsbeoordeling", value: `${Math.round(data.stats.averageSafety)}%`},
        {title: "Gemiddelde Duurzaamheid", value: `${Math.round(data.stats.averageSustainability)}%`}
    ])

    let categoryLabels = $derived(data.categoryCounts.map((item) => item.category));
    let categoryValues = $derived(data.categoryCounts.map((item) => item.count));

    let filterRenderKey = $state(0)
    let selectedMunicipality = $state<string | null>(null)
    let scansContainerElement: HTMLDivElement | null = $state(null)
    let scansContainerMaxHeight = $state('none')

    let filteredScans = $derived(
        data.scans.filter((scan) => {
            if (!selectedMunicipality) {
                return true;
            }

            const municipalityName = scan.municipality?.name?.trim().toLowerCase() ?? '';
            return municipalityName === selectedMunicipality;
        })
    )

    function applyFilter(value: any){
        selectedMunicipality = typeof value === 'string' ? value.toLowerCase() : null;
    }

    function applySafetyFilter(value: 'all' | 'dangerous' | 'safe') {
        //need to make this function still 
        console.log(value)
    }

    function clearAllFilters() {
        filterRenderKey += 1;
        applyFilter(null);
        applySafetyFilter('all');
    }

    async function updateScansContainerMaxHeight() {
        await tick();

        if (!scansContainerElement) {
            scansContainerMaxHeight = 'none';
            return;
        }

        const cards = Array.from(
            scansContainerElement.querySelectorAll<HTMLElement>('.scan-card')
        );

        if (cards.length === 0) {
            scansContainerMaxHeight = 'none';
            return;
        }

        const visibleCardsCount = Math.min(3, cards.length);
        const firstCard = cards[0];
        const lastVisibleCard = cards[visibleCardsCount - 1];
        const lastCardStyles = getComputedStyle(lastVisibleCard);
        const marginBottom = parseFloat(lastCardStyles.marginBottom || '0');

        const height =
            lastVisibleCard.offsetTop +
            lastVisibleCard.offsetHeight +
            marginBottom -
            firstCard.offsetTop;

        scansContainerMaxHeight = `${Math.ceil(height)}px`;
    }

    $effect(() => {
        filteredScans.length;
        updateScansContainerMaxHeight();
    });

    onMount(() => {
        updateScansContainerMaxHeight();

        const onResize = () => {
            updateScansContainerMaxHeight();
        };

        window.addEventListener('resize', onResize);

        return () => {
            window.removeEventListener('resize', onResize);
        };
    });
</script>

<div id="dashboard-top"></div>
<Header></Header>
<div id="KpiStatisticsFlexBox">
    <div id="kpiCardsGrid">
        {#each statistics as statistic}
            <KpiStatistic statisticTitle={statistic.title} statistic={statistic.value}></KpiStatistic>
        {/each}
    </div>

    <PieChart labels={categoryLabels} values={categoryValues} pieChartTitle="Product Categorieën"></PieChart>
</div>

<div id="filtersContainer">
    {#key filterRenderKey}
        <DropdownMenu dropdownTitle="Gemeentes" dropdownItems={MunicipalitiesJson} itemChosenEvent={applyFilter}></DropdownMenu>
    {/key}

    {#key `danger-${filterRenderKey}`}
        <ShowOnlyDangerousFIlter itemChosenEvent={applySafetyFilter}></ShowOnlyDangerousFIlter>
    {/key}

    <button id="clearFiltersButton" type="button" onclick={clearAllFilters}>
        x Verwijder filters
    </button>
</div>

<div class="scansContainer" bind:this={scansContainerElement} style={`max-height: ${scansContainerMaxHeight};`}>
    {#if filteredScans.length === 0}
        <p>No scans found.</p>
    {:else}
        {#each filteredScans as scan}
            <ScanCard {scan} />
        {/each}
    {/if}
</div>

<NetherlandsMap></NetherlandsMap>

<style>
    #KpiStatisticsFlexBox {
        display: flex;
        gap: 16px;
        align-items: stretch;
        margin-top: 30px;
        margin-left: 30px;
        margin-right: 30px;
    }

    #kpiCardsGrid {
        display: grid;
        grid-template-columns: repeat(2, minmax(0, 1fr));
        gap: 16px;
        flex: 1 1 auto;
        min-width: 0;
    }

    @media (max-width: 1200px) {
        #KpiStatisticsFlexBox {
            flex-direction: column;
        }

        #kpiCardsGrid {
            width: 100%;
        }
    }

    @media (max-width: 700px) {
        #kpiCardsGrid {
            grid-template-columns: 1fr;
        }
    }

    #filtersContainer {
        background-color: var(--color-primary);
        border: 1px solid var(--color-primary-dark);
        border-left: 4px solid var(--color-secondary-dark);
        display: flex;
        align-items: center;
        gap: 12px;
        flex-wrap: wrap;
        padding: 10px;
        margin: 30px;
        border-radius: 10px;
    }

    #clearFiltersButton {
        background-color: var(--color-bg);
        color: var(--color-primary-dark);
        border: 1px solid var(--color-primary-dark);
        font-weight: 700;
    }

    #clearFiltersButton:hover {
        background-color: var(--color-primary-dark);
        color: var(--color-bg);
        cursor: pointer;    
    }

	.scansContainer {
        padding: 10px;
        margin: 30px;
        width: calc(100% - 60px);
        box-sizing: border-box;
		overflow-y: auto;
		overscroll-behavior: contain;
        scrollbar-width: none;
        -ms-overflow-style: none;
	}

    .scansContainer::-webkit-scrollbar {
        display: none;
    }

    
</style>
