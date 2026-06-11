
import type { ScannedProduct, Ghs, Alternative } from "$lib/stores/thuishulpScanResultaten.svelte";

const API_BASE_URL = import.meta.env.PUBLIC_API_BASE_URL ?? 'http://localhost:5141';

function buildApiUrl(path: string): string {
    return `${API_BASE_URL}${path}`;
}

export async function getScanResultsById(id: number, fetch: typeof window.fetch): Promise<ScannedProduct[]> {
	const response = await fetch(buildApiUrl(`/api/scans/${id}`));
	const data = await response.json();
	const scanResults: ScannedProduct[] = []

	for (const scannedProduct of data.detectedProducts) {
		let warningLabels: Ghs[] = []
		for (const warningLabel of scannedProduct.product.warningLabels) {

			warningLabels.push({
				type: warningLabel.type,
				description: warningLabel.description
			});
		}

		scanResults.push({
			imageURL: scannedProduct.product.imageURL,
			productName: scannedProduct.product.productName,
			productType: scannedProduct.product.productType,
			productId: scannedProduct.product.productId,
			riskLevel: scannedProduct.product.riskLevel
		});
	}
 
	
	

	return scanResults;
}

export function getProductById(productId: number): ScannedProduct | undefined {
	return scanResults.find(
		(product) => product.productId === productId
	);
}