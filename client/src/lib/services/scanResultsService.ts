import dummyScanResults from "$lib/assets/ThuishulpScannedProducts.json";
import type { ScannedProduct } from "$lib/stores/thuishulpScanResultaten.svelte";

// replace this with actual API calls when backend is ready
const scanResults = dummyScanResults as ScannedProduct[];

export async function getScanResults(): Promise<ScannedProduct[]> {
	return scanResults;
}

export function getProductById(productId: number): ScannedProduct | undefined {
	return scanResults.find(
		(product) => product.productId === productId
	);
}