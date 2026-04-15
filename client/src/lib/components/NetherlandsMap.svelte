<section class="mapSection" aria-label="Kaart van Nederland">
	<iframe
		title="OpenStreetMap Nederland"
		class="mapFrame"
		src="https://www.openstreetmap.org/export/embed.html?bbox=3.25%2C50.75%2C7.25%2C53.65&layer=mapnik"
		loading="lazy"
	></iframe>
</section>

<style>
	.mapSection {
		padding: 10px;
		margin: 30px;
		width: calc(100% - 60px);
		box-sizing: border-box;
		background-color: var(--color-bg);
		border: 2px solid var(--color-primary);
		border-top: 6px solid var(--color-secondary-dark);
		border-radius: 10px;
	}

	.mapFrame {
		display: block;
		width: 100%;
		height: 360px;
		border: 0;
		border-radius: 8px;
	}
</style>
