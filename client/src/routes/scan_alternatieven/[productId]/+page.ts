import { getAlternativesByProductId } from "$lib/services/scanResultsService";

export async function load({ params, fetch }: { params: { productId: string }, fetch: typeof window.fetch }) {
    return {
        alternatives: await getAlternativesByProductId(parseInt(params.productId), fetch)
    };
}