import { getMunicipalityScanCounts } from '$lib/services/municipalityService';

export const load = async ({ fetch }: { fetch: typeof window.fetch }) => {
	const municipalityScanCounts = await getMunicipalityScanCounts(fetch);
	return { municipalityScanCounts };
};
