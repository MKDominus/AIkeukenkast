const API_BASE_URL = import.meta.env.PUBLIC_API_BASE_URL ?? 'http://localhost:5141';

function buildApiUrl(path: string): string {
    return `${API_BASE_URL}${path}`;
}

export async function getProducts(fetch: typeof window.fetch) {
    const res = await fetch(buildApiUrl('/api/products'));
    return res.json();
}

export async function getProductIngredients(fetch: typeof window.fetch, productId: number) {
    const res = await fetch(buildApiUrl(`/api/products/${productId}/ingredients`));
    return res.json();
}