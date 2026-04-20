<!-- Documentation to come -->
<script lang="ts">
  import { onMount, onDestroy } from 'svelte';
  import {
    Chart,
    PieController,
    ArcElement,
    Tooltip,
    Legend,
    type ChartConfiguration,
    type Chart as ChartType
  } from 'chart.js';

  Chart.register(PieController, ArcElement, Tooltip, Legend);

  let canvas: HTMLCanvasElement | null = null;
  let chart: ChartType | null = null;

  type PieChartData = {
    pieChartTitle: string
    labels: string[]
    values: number[]
  }
  
  let { labels, values, pieChartTitle }: PieChartData = $props();

  const palette = ['#662773', '#87B140', '#411447', '#446817', '#8C4A98', '#A5C86D'];
  let colors = $derived(values.map((_, i) => palette[i % palette.length]));

  onMount(() => {
    if (!canvas) return;

    let config: ChartConfiguration<'pie'> = {
      type: 'pie',
      data: {
        labels,
        datasets: [
          {
            data: values,
            backgroundColor: colors,
            hoverOffset: 8
          }
        ]
      },
      options: {
        responsive: true,
        plugins: {
          legend: {
            position: 'right'
          }
        }
      }
    };

    chart = new Chart(canvas, config);
  });

  onDestroy(() => {
    chart?.destroy();
  });


</script>

<div id="pieChartContainer">
  <h2 id="pieChartTitle">{pieChartTitle}</h2>
  <canvas id="pieChart" bind:this={canvas}></canvas>
</div>

<style>
  #pieChartContainer {
    background-color: var(--color-bg);
    border: 2px solid var(--color-primary);
    border-top: 6px solid var(--color-secondary-dark);
    padding: 16px;
    height: 100%;
    width: 300px;
    border-radius: 10px;
  }

  #pieChart {
    max-height: 100px;
    margin-top: 10px;
  }

  #pieChartTitle {
    text-align: center;
    font-size: medium;
    color: var(--color-primary-dark);
  }
</style>
