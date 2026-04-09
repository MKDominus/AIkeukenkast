import { getScans } from '$lib/services/scanService';

export const load = async ({ fetch }: { fetch: typeof window.fetch }) => {
	const scans = await getScans(fetch);
	return { scans };
};
