
import type { ScannedProduct, Ghs, Alternative } from "$lib/stores/thuishulpScanResultaten.svelte";
import { env } from '$env/dynamic/public';

const API_BASE_URL = env.PUBLIC_API_BASE_URL ?? 'http://localhost:5141';

function buildApiUrl(path: string): string {
    return `${API_BASE_URL}${path}`;
}

export async function getScanResultsById(id: number, fetch: typeof window.fetch): Promise<ScannedProduct[]> {
	const response = await fetch(buildApiUrl(`/api/scans/${id}`));
	const data = await response.json();
	const scanResults: ScannedProduct[] = []

	for (const scannedProduct of data.detectedProducts) {
		let warningLabels: Ghs[] = []
		for (const dangerSymbol of scannedProduct.product.dangerSymbols) {
			let dangerSymbolData = dangerSymbol.split(";");

			warningLabels.push({
				type: dangerSymbolData[0],
				description: dangerSymbolData[1]
			});
		}

		scanResults.push({
			imageURL: scannedProduct.product.imageURL,
			productName: scannedProduct.product.productName,
			productType: scannedProduct.product.productType,
			productId: scannedProduct.product.productId,
			riskLevel: scannedProduct.product.riskLevel,
			warningLabels: warningLabels,
			dangers: scannedProduct.product.dangers,
			precautions: scannedProduct.product.precautions,
			alternatives: scannedProduct.product.alternatives
		});
	}
 
	
	

	return scanResults;
}

export async function getProductById(productId: number, fetch: typeof window.fetch): Promise<ScannedProduct> {
	const response = await fetch(buildApiUrl(`/api/products/${productId}`));
	const data = await response.json();

	let warningLabels: Ghs[] = []
		for (const dangerSymbol of data.dangerSymbols) {
			let dangerSymbolData = dangerSymbol.split(";");

			warningLabels.push({
				type: dangerSymbolData[0],
				description: dangerSymbolData[1]
			});
		}

		const scannedProduct: ScannedProduct = {
			imageURL: data.imageURL,
			productName: data.productName,
			productType: data.productType,
			productId: data.productId,
			riskLevel: data.riskLevel,
			warningLabels: warningLabels,
			dangers: data.dangers,
			precautions: data.precautions,
			alternatives: data.alternatives
		};
	return scannedProduct;
}

export async function getAlternativesByProductId(productId: number, fetch: typeof window.fetch): Promise<Alternative[]> {
	const response = await fetch(buildApiUrl(`/api/products/${productId}/alternatives`));
	if (!response.ok) {
		throw new Error(`Failed to load alternatives for product ${productId}: ${response.status} ${response.statusText}`);
	}
	const data = await response.json();
	return data.map((alt: Alternative) => ({
		productName: alt.productName,
		productType: alt.productType,
		imageURL: alt.imageURL
	}));
}