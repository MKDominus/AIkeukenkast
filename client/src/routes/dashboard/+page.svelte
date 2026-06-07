<script lang="ts">
	import { onMount, tick } from 'svelte';
	import Header from '$lib/components/Header.svelte';
    import KpiStatistic from '$lib/components/KpiStatistic.svelte';
    import DropdownMenu from '$lib/components/DropdownMenu.svelte';
    import ShowOnlyDangerousFIlter from '$lib/components/ShowOnlyDangerousFIlter.svelte';
    import MunicipalitiesJson from '$lib/assets/Municipalities.json';
    import PieChart from '$lib/components/PieChart.svelte';
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
        {
            title: "Gemiddeld Producten per Scan",
            value: `${data.stats.totalScans > 0 ? (data.stats.productsScanned / data.stats.totalScans).toFixed(1) : '0.0'}`
        },
        {title: "Gemiddeld Risico", value: `${Math.round(data.stats.averageRisk)}%`}
    ])

    let categoryLabels = $derived(data.categoryCounts.map((item) => item.productType));
    let categoryValues = $derived(data.categoryCounts.map((item) => item.count));

    let filterRenderKey = $state(0)
    let selectedMunicipality = $state<string | null>(null)
    let selectedSafetyFilter = $state<'all' | 'safe' | 'riskant' | 'unsafe'>('all')
    let scansContainerElement: HTMLDivElement | null = $state(null)
    let scansContainerMaxHeight = $state('none')

    function getScanRiskCategory(scan: Scan): 'safe' | 'riskant' | 'unsafe' {
        const riskLevels = scan.detectedProducts
            .map((detectedProduct) => detectedProduct.product?.riskLevel)
            .filter((riskLevel): riskLevel is string => Boolean(riskLevel));

        if (riskLevels.includes('Onveilig')) {
            return 'unsafe';
        }

        if (riskLevels.includes('Riskant')) {
            return 'riskant';
        }

        return 'safe';
    }

    let filteredScans = $derived(
        data.scans.filter((scan) => {
            const municipalityName = scan.municipality?.name?.trim().toLowerCase() ?? '';
            const matchesMunicipality = !selectedMunicipality || municipalityName === selectedMunicipality;

            if (!matchesMunicipality) {
                return false;
            }

            if (selectedSafetyFilter === 'all') {
                return true;
            }

            return getScanRiskCategory(scan) === selectedSafetyFilter;
        })
    )

    function applyFilter(value: any){
        selectedMunicipality = typeof value === 'string' ? value.toLowerCase() : null;
    }

    function applySafetyFilter(value: 'all' | 'safe' | 'riskant' | 'unsafe') {
        selectedSafetyFilter = value;
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

</div>
<div id=pieAndFiltersContainer>
    <PieChart labels={categoryLabels} values={categoryValues} pieChartTitle="Product Categorieën"></PieChart>
    <div id="filtersContainer">
    <div class="filtersHeader">
        <h2 class="filterTitle">Filters</h2>

        <button id="clearFiltersButton" type="button" onclick={clearAllFilters}>
            ✕ Verwijder filters
        </button>
    </div>

    <div class="allFilters">
        {#key filterRenderKey}
            <DropdownMenu
                dropdownTitle="Gemeentes"
                dropdownItems={MunicipalitiesJson}
                itemChosenEvent={applyFilter}
                variant="primary"
            />
        {/key}

        {#key `danger-${filterRenderKey}`}
            <ShowOnlyDangerousFIlter
                itemChosenEvent={applySafetyFilter}
            />
        {/key}
    </div>

    <p id="scanCountText">
        {filteredScans.length} scan{filteredScans.length === 1 ? '' : 's'} zichtbaar
    </p>
</div>
</div>
<div class="scansContainer" bind:this={scansContainerElement} style={`max-height: ${scansContainerMaxHeight};`}>
<h2 class="recentScans"> Recente Scans</h2>
<p class="scanDescription">Per gemeente, gesorteerd op datum</p>
    {#if filteredScans.length === 0}
        <p>No scans found.</p>
    {:else}
        {#each filteredScans as scan}
            <ScanCard {scan} />
        {/each}
    {/if}
</div>

<style>
    #KpiStatisticsFlexBox {
        display: flex;
        gap: 16px;
        align-items: stretch;
        margin-top: 30px;
        margin-left: 100px;
        margin-right: 100px;
    }

    #kpiCardsGrid {
        display: flex;
        gap: 16px;
        flex: 1 1 auto;
        min-width: 0;
        height: 120px;
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

    #pieAndFiltersContainer {
    display: flex;
    gap: 24px;
    align-items: flex-start;
    margin-top: 30px;
    margin-left: 100px;
    margin-right: 100px;
}

   #filtersContainer {
    border: 2px solid var(--color-primary);
    border-radius: 10px;
    width: 100%;
    height: 190px;
    padding: 16px;
    display: flex;
    flex-direction: column;
    gap: 10px;
    box-shadow: var(--shadow-card);
}

.filtersHeader {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
}

.filterTitle {
    margin: 0;
    font-size: 1.1rem;
    font-weight: 600;
}

.allFilters {
    display: flex;
    gap: 10px;
    align-items: center;
    flex-wrap: wrap;
}

#clearFiltersButton {
    background-color: var(--color-bg);
    color: var(--color-primary);
    border: none;
    padding: 6px 10px;

    font-size: 0.8rem;
    font-weight: 700;
}

#clearFiltersButton:hover {
    background-color: var(--color-bg);
    color: var(--color-primary-dark);
    cursor: pointer;
}

#scanCountText {
    margin: 0;
    margin-top: auto;

    color: var(--color-text-muted);
    font-weight: 600;
    font-size: 0.9rem;
}

	.scansContainer {
        padding: 10px;
        padding-left: 70px;
        padding-right: 70px;
        margin: 30px;
        width: calc(100% - 60px);
        box-sizing: border-box;
	}

    .recentScans{
        font-weight: 600;
        font-size: 1.4rem;
        margin-bottom: 8px;
    }

    .scanDescription{
        font-size: 0.9rem;
        color: var(--color-text-muted);
        margin-bottom: 10px;
    }

    .scansContainer::-webkit-scrollbar {
        display: none;
    }

    

    
</style>
