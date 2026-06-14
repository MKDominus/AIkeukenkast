import { getScans, getScanStats } from '$lib/services/scanService';
import { getDetectedProductCategoryCounts } from '$lib/services/scanService';

export const load = async ({ fetch }: { fetch: typeof window.fetch }) => {
	const [scans, stats, categoryCounts] = await Promise.all([
		getScans(fetch),
		getScanStats(fetch),
		getDetectedProductCategoryCounts(fetch)
	]);
	return { scans, stats, categoryCounts };
};
