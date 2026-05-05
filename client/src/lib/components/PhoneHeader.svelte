<script lang="ts">
    import TzorgLogo_trans from "$lib/assets/Tzorg_logo_transparant.png"
    import MenuButton from "$lib/assets/MenuButton.png";
</script>

<div id="mainHeader">
        <img id="menuButton" src={MenuButton} alt="Menu Button">
        <img id="headerLogo" src={TzorgLogo_trans} alt="Tzorg Logo">
</div>

<style>
    #menuButton {
        display: none;
    }

    #headerLogo {
        height: 80px;
        margin: 13px;
    }

    
    @media (max-width: 768px) {
        #mainHeader {
            background: var(--color-bg);
            display: flex;
            align-items: center;
        }

        #headerLogo {
            height: 45px;
            margin: 0;
        }

        #menuButton {
            display: block;
            height: 20px;
            margin: 15px;
            margin-right: 10px; 
        }
	}
</style>