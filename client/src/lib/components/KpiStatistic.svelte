<!--
@component

### KpiStatistic

---

#### Description

Displays a KPI/statistic card containing a title and a statistic value.

Used for dashboards or overview sections where compact statistic visualization
is required.

---

#### Usage

```svelte
<KpiStatistic
	statisticTitle="Totale gebruikers"
	statistic="1.245"
/>
```

---

#### Props

| Prop | Type | Description |
| ---- | ---- | ----------- |
| statisticTitle | string | Title/label describing the statistic |
| statistic | string | Statistic value displayed on the right side |

-->

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
        padding: 10px 14px;
        min-width: 0;
        width: 100%;
        max-width: none;
        min-height: 72px;
        height: 100%;
        border-radius: 10px;
        transition: transform 0.2s ease;
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 14px;
    }

    #contentBody:hover {
        transform: translateY(-4px);
    }

    #statisticTitle {
        color: var(--color-primary-dark);
        font-weight: 700;
        font-size: 1.05rem;
        margin: 0;
    }

    #statistic {
        color: var(--color-primary);
        font-weight: 800;
        font-size: 1.05rem;
        margin-top: 0;
        margin-bottom: 0;
        white-space: nowrap;
    }
</style>