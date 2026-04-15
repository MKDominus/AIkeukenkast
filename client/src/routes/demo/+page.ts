import { getScans, getScanStats } from '$lib/services/scanService';
import { getProductCategoryCounts } from '$lib/services/productService';

export const load = async ({ fetch }: { fetch: typeof window.fetch }) => {
	const [scans, stats, categoryCounts] = await Promise.all([
		getScans(fetch),
		getScanStats(fetch),
		getProductCategoryCounts(fetch)
	]);
	return { scans, stats, categoryCounts };
};
