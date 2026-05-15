<script lang="ts">
    type TitleCard = {
        title: string
    }

    let {
        title
    }: TitleCard = $props()
</script>

<div id="contentContainer">
    <h2 id="title">{title}</h2>
</div>

<style>
    #contentContainer {
        background-color: var(--color-primary);
        padding: 20px;
        display: flex;
        align-items: center;
        justify-content: center;
    }

    #title {
        color: var(--color-bg);
        font-weight: 700;
        font-size: 2rem;
        margin: 0;
    }

    @media (max-width: 768px) {
        #contentContainer {
            padding: 15px;
        }

        #title {
            font-weight: 700;
            font-size: 1.15rem;
        }
	}
    
</style>