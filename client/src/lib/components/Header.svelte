<script lang="ts">
    import TzorgLogo from "$lib/assets/tzorg_logo.png"
</script>

<div id="header">
    <img id="headerLogo" src={TzorgLogo} alt="Tzorg Logo">
    <h1 id="headerTitle">AI Keukenkastje</h1>

    <nav id="headerNav" aria-label="Pagina navigatie">
        <a class="headerLink" href="#dashboard-top">Dashboard</a>
        <a class="headerLink" href="#map-section">Map</a>
    </nav>
</div>

<style>
    #header {
        background: var(--color-bg);
        display: flex;
        align-items: center;
        gap: 18px;
        position: sticky;
        top: 0;
        z-index: 100;
        border-bottom: 1px solid var(--color-border);
        padding-right: 20px;
    }

    #headerTitle {
        color: var(--color-primary);
        font-size: x-large;
        font-family: var(--font-header);
        font-weight: 700;
        line-height: 1.4;
        letter-spacing: normal;
        margin-left: 8px;
        margin-top: 10px;
        margin-bottom: 10px;

    }

    #headerLogo {
        height: 80px;
        margin-left: 16px;
    }

    #headerNav {
        margin-left: 18px;
        flex: 0 1 auto;
        display: flex;
        align-items: center;
        justify-content: flex-start;
        gap: 20px;
        font-size: x-large;
        font-family: var(--font-header);
        font-weight: 700;
        line-height: 1.4;
        letter-spacing: normal;
    }

    .headerLink {
        text-decoration: none;
        color: var(--color-primary);
        font: inherit;
        font-weight: inherit;
        border: none;
        background: transparent;
        transition: color 0.2s ease, text-decoration-color 0.2s ease;
    }

    .headerLink:hover {
        color: var(--color-primary);
        text-decoration: underline;
    }

    @media (max-width: 820px) {
        #header {
            flex-wrap: wrap;
            padding-bottom: 10px;
            gap: 10px;
        }

        #headerTitle {
            margin-top: 0;
            margin-bottom: 0;
            margin-left: 0;
        }

        #headerNav {
            width: 100%;
            margin-left: 0;
            justify-content: space-around;
            gap: 16px;
        }
    }
</style>