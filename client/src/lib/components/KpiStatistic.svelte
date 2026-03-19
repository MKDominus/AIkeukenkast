<script lang="ts">
    type KpiStatistic = {
        statisticTitle: string
        statistic: string
    }

    let {
        statisticTitle,
        statistic
    }: KpiStatistic = $props()
</script>

<div id="contentBody">
    <h2 id="statisticTitle">{statisticTitle}</h2>
    <p>{statistic}</p>
</div>

<style>
    #contentBody {
        background-color: grey;
        padding: 20px;
        margin: auto;
        min-width: 230px;
        max-width: 400px;
    }

    #statisticTitle {
        font-weight: bold;
    }
</style>