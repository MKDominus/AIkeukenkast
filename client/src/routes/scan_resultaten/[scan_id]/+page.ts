import { getScanResultsById } from "$lib/services/scanResultsService";

export async function load({ params, fetch }: { params: { scan_id: string }, fetch: typeof window.fetch }) {
	return {
		scanResults: await getScanResultsById(parseInt(params.scan_id), fetch)
	};
}