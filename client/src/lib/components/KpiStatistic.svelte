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
    import totaleScansIcon from '$lib/assets/dashboard_icons/totale_scans.png';
	import productenGescannedIcon from '$lib/assets/dashboard_icons/producten_gescanned.png';
	import gemiddeldProductenPerScanIcon from '$lib/assets/dashboard_icons/gemiddeld_producten_per_scan.png';
	import gemiddeldRisicoIcon from '$lib/assets/dashboard_icons/gemiddeld_risico.png';

	type KpiStatistic = {
		statisticTitle: string;
		statistic: string;
	};

	let {
		statisticTitle,
		statistic
	}: KpiStatistic = $props();

	const icons: Record<string, string> = {
		'Totale Scans': totaleScansIcon,
		'Producten Gescanned': productenGescannedIcon,
		'Gemiddeld Producten per Scan': gemiddeldProductenPerScanIcon,
		'Gemiddeld Risico': gemiddeldRisicoIcon
	};

	const fallbackIcon = '/dashboard_icons/default.png';
</script>

<div id="contentBody">
    <div class="statisticHeader">
        <div class="imageBox">
        <img
        src={icons[statisticTitle] ?? fallbackIcon}
        alt={statisticTitle}
        class="statisticIcon"
        /> 
    </div>
    <h2 id="statisticTitle">{statisticTitle}</h2>
    </div>
	<p id="statistic">{statistic}</p>
</div>

<style>
	#contentBody {
		background-color: var(--color-bg);
		border: 2px solid var(--color-primary);

		padding: 14px 18px;

		min-width: 0;
		width: 100%;
		max-width: none;

		min-height: 72px;
		height: 100%;

		border-radius: 10px;
		transition:
			transform 0.2s ease,
			box-shadow 0.2s ease;

		display: flex;
		flex-direction: column;
		gap: 14px;
		box-shadow: var(--shadow-card);
	}


	#contentBody:hover {
		transform: translateY(-4px);
		box-shadow: 0 6px 16px rgba(0, 0, 0, 0.08);
	}
    .statisticHeader {
        display: flex;
        align-items: left;
        gap: 12px;
        height: 80px;
     }

	.imageBox {
	display: flex;
	align-items: center;
	justify-content: center;
	padding: 8px;
    max-width: 30px;
    max-height: 30px;
	background-color: rgba(135, 177, 64, 0.648);
	border-radius: 10px;
    }

	.statisticIcon {
		width: 24px;
		height: 24px;
		flex-shrink: 0;
		object-fit: contain;
	}

	#statisticTitle {
		color: var(--color-text-muted);
		font-weight: 700;
		font-size: 1.05rem;
		margin: 0;
		line-height: 1.2;
	}

	#statistic {
		color: var(--color-primary);
		font-weight: 800;
		font-size: 1.25rem;
		margin: 0;
		white-space: nowrap;
	}
</style>