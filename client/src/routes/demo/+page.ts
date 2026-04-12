import { getScans, getScanStats } from '$lib/services/scanService';

export const load = async ({ fetch }: { fetch: typeof window.fetch }) => {
	const [scans, stats] = await Promise.all([getScans(fetch), getScanStats(fetch)]);
	return { scans, stats };
};
