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
        background-color: var(--color-bg);
        border: 2px solid var(--color-primary);
        border-top: 6px solid var(--color-secondary-dark);
        padding: 16px;
        margin: auto;
        min-width: 210px;
        max-width: 360px;
        border-radius: 10px;
    }

    #statisticTitle {
        color: var(--color-primary-dark);
        font-weight: 700;
        font-size: 1.3rem;
        margin: 0;
    }

    #statistic {
        color: var(--color-primary);
        font-weight: 800;
        font-size: 1.2rem;
        margin-top: 6px;
        margin-bottom: 0;
    }
</style>