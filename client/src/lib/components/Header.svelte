<script lang="ts">
    import TzorgLogo from "$lib/assets/tzorg_logo.png"
</script>

<div id="header">
    <h1 id="headerTitle">Keukenkastje dashboard</h1>
    <img id="headerLogo" src={TzorgLogo} alt="Tzorg Logo">
</div>

<style>
    #header {
        background: #83C171;
        background: linear-gradient(90deg, rgba(131, 193, 113, 1) 0%, rgba(255, 255, 255, 1) 90%);
        display: flex;
        align-items: center;
    }

    #headerTitle {
        color: white;
        font-size: x-large;
        margin-left: 20px;
        margin-top: 10px;
        margin-bottom: 10px;

    }

    #headerLogo {
        height: 80px;
        margin-left: auto;
    }
</style>