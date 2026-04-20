import { getScans, getScanStats } from '$lib/services/scanService';
import { getProductCategoryCounts } from '$lib/services/productService';
import { getMunicipalityScanCounts } from '$lib/services/municipalityService';

export const load = async ({ fetch }: { fetch: typeof window.fetch }) => {
	const [scans, stats, categoryCounts, municipalityScanCounts] = await Promise.all([
		getScans(fetch),
		getScanStats(fetch),
		getProductCategoryCounts(fetch),
		getMunicipalityScanCounts(fetch)
	]);
	return { scans, stats, categoryCounts, municipalityScanCounts };
};
