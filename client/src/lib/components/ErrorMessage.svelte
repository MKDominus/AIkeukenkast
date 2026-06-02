<script lang="ts">
    type ErrorMessage = {
        message: string
    }

    let {
        message
    }: ErrorMessage = $props()
</script>

<div id="contentContainer">
    <p id="message">{message}</p>
</div>

<style>
    #contentContainer {
        background-color: #E95555;
        margin: 16px;
        padding: 20px;
        display: flex;
        align-items: center;
        justify-content: center;
        border-radius: 10px;
        box-shadow: 1px 10px 12px -6px rgba(0,0,0,0.42);
        -webkit-box-shadow: 1px 10px 12px -6px rgba(0,0,0,0.42);
        -moz-box-shadow: 1px 10px 12px -6px rgba(0,0,0,0.42);
    }

    #message {
        color: var(--color-bg);
    }

    @media (max-width: 768px) {
        #contentContainer {
            margin: 16px;
            padding: 15px;
        }
	}
    
</style>