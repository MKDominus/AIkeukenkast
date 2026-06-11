import { getProductById } from "$lib/services/scanResultsService";

export async function load({ params, fetch }: { params: { productId: string }, fetch: typeof window.fetch }) {
    return {
        product: await getProductById(parseInt(params.productId), fetch)
    };
}