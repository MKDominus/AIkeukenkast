import { getScanResults } from "$lib/services/scanResultsService";

export async function load() {
	return {
		scanResults: await getScanResults()
	};
}