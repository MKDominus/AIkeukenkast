<script lang="ts">
	import Header from '$lib/components/Header.svelte';
    import KpiStatistic from '$lib/components/KpiStatistic.svelte';
</script>

<Header></Header>
<div id="KpiStatisticsFlexBox">
    <KpiStatistic>
        <h2 slot="statisticTitle">Totale Scans</h2>
        <p slot="statistic">16</p>
    </KpiStatistic>
    <KpiStatistic>
        <h2 slot="statisticTitle">Producten Gescanned</h2>
        <p slot="statistic">104</p>
    </KpiStatistic>
    <KpiStatistic>
        <h2 slot="statisticTitle">Veiligheidsbeoordeling</h2>
        <p slot="statistic">64%</p>
    </KpiStatistic>
    <KpiStatistic>
        <h2 slot="statisticTitle">Gemiddelde Duurzaamheid</h2>
        <p slot="statistic">78</p>
    </KpiStatistic>
</div>

<style>
    #KpiStatisticsFlexBox {
        display: flex;
        margin-top: 30px;
    }
</style>
