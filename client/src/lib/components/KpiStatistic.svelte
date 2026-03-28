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
    <p id="statistic">{statistic}</p>
</div>

<style>
    #contentBody {
        background-color: #b5daab;
        padding: 20px;
        margin: auto;
        min-width: 230px;
        max-width: 400px;
        border-radius: 10px;
    }

    #statistic {
        font-weight: bold;
        font-size: larger;
    }
</style>