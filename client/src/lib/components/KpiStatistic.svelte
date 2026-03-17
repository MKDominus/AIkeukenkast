<div id="contentBody">
    <slot id="statisticTitle" name="statisticTitle"></slot>
    <slot name="statistic"></slot>
</div>

<style>
    #contentBody {
        background-color: grey;
        padding: 20px;
        margin: auto;
        min-width: 200px;
        max-width: 400px;
    }

    #statisticTitle {
        font-weight: bold;
    }
</style>