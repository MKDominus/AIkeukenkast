<script lang="ts">
    import TzorgLogo from "$lib/assets/tzorg_logo.png"
</script>

<div id="header">
    <img id="headerLogo" src={TzorgLogo} alt="Tzorg Logo">
    <h1 id="headerTitle">Keukenkastje dashboard</h1>
</div>

<style>
    #header {
        background: var(--color-bg);
        display: flex;
        align-items: center;
        position: sticky;
        top: 0;
        z-index: 100;
        border-bottom: 1px solid var(--color-border);
    }

    #headerTitle {
        color: var(--color-primary);
        font-size: x-large;
        margin-left: 20px;
        margin-top: 10px;
        margin-bottom: 10px;

    }

    #headerLogo {
        height: 80px;
        margin-left: 16px;
    }
</style>