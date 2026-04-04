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

  let colors: string[] = values.map((_, i) => {
    const hue = (i * 360) / values.length;
    return `hsl(${hue}, 70%, 60%)`;
  });

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
    background-color: #DFE7F0;
    padding: 10px;
    height: fit-content;
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
  }
</style>
