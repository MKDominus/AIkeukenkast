<script lang="ts">
	type ProgressIndicatorProps = {
		steps: number;
		visualCurrentStep: number;
	};

	let { steps, visualCurrentStep }: ProgressIndicatorProps = $props();

	const clampedStep = $derived(Math.min(Math.max(visualCurrentStep, 1), steps));
	const progressScale = $derived(steps > 1 ? (clampedStep - 1) / (steps - 1) : 0);
</script>

<div id="mainContainer">
	<div class="trackContainer">
		<div class="track"></div>
		<div class="trackActive" style:transform={`scaleX(${progressScale})`}></div>
	</div>

	<div class="stepsRow">
		{#each Array(steps) as _, index}
			<div class="stepCircle" class:active={index + 1 <= clampedStep}>
				{index + 1}
			</div>
		{/each}
	</div>
</div>

<style>
    #mainContainer {
        position: relative;
        width: calc(100% - 32px);
        height: 30px;
        margin-left: 16px;
        margin-right: 16px;
        box-sizing: border-box;

        --circle-size: 30px;
        --track-height: 4px;
        --inactive-color: #999999;
    }

    .trackContainer {
        position: absolute;
        top: 50%;
        left: calc(var(--circle-size) / 2);
        right: calc(var(--circle-size) / 2);
        height: var(--track-height);
        transform: translateY(-50%);
    }

    .track,
    .trackActive {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        border-radius: 999px;
        transform-origin: left center;
    }

    .track {
        background-color: var(--inactive-color);
    }

    .trackActive {
        background-color: var(--color-primary);
    }

    .stepsRow {
        position: relative;
        z-index: 1;
        display: flex;
        align-items: center;
        justify-content: space-between;
        width: 100%;
        height: 100%;
    }

    .stepCircle {
        display: flex;
        align-items: center;
        justify-content: center;
        flex-shrink: 0;
        width: var(--circle-size);
        height: var(--circle-size);
        border-radius: 50%;
        background-color: var(--inactive-color);
        color: white;
        font-size: 20px;
        font-weight: 700;
        line-height: 1;
    }

    .stepCircle.active {
        background-color: var(--color-primary);
    }

	@media (max-width: 768px) {
        #mainContainer {
            position: relative;
            width: calc(100% - 32px);
            height: 30px;
            margin-left: 16px;
            margin-right: 16px;
            box-sizing: border-box;

            --circle-size: 30px;
            --track-height: 4px;
            --inactive-color: #999999;
        }

        .trackContainer {
            position: absolute;
            top: 50%;
            left: calc(var(--circle-size) / 2);
            right: calc(var(--circle-size) / 2);
            height: var(--track-height);
            transform: translateY(-50%);
        }

        .track,
        .trackActive {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            border-radius: 999px;
            transform-origin: left center;
        }

        .track {
            background-color: var(--inactive-color);
        }

        .trackActive {
            background-color: var(--color-primary);
        }

        .stepsRow {
            position: relative;
            z-index: 1;
            display: flex;
            align-items: center;
            justify-content: space-between;
            width: 100%;
            height: 100%;
        }

        .stepCircle {
            display: flex;
            align-items: center;
            justify-content: center;
            flex-shrink: 0;
            width: var(--circle-size);
            height: var(--circle-size);
            border-radius: 50%;
            background-color: var(--inactive-color);
            color: white;
            font-size: 18px;
            font-weight: 700;
            line-height: 1;
        }

        .stepCircle.active {
            background-color: var(--color-primary);
        }
    }
</style>