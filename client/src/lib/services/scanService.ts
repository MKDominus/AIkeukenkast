export type ProductIngredient = {
    id: number;
    name: string;
    description?: string | null;
    isHazardous: boolean;
    concentration: number;
};

export type ScanProduct = {
    id: number;
    productId: number;
    confidence: number;
    count: number;
    product?: {
        id: number;
        name: string;
        brand?: string | null;
        imageUrl?: string | null;
        category: string;
        sustainabilityScore: number;
        isSustainable: boolean;
        safetyWarnings?: string | null;
        ingredients: ProductIngredient[];
    } | null;
};

export type Scan = {
    id: number;
    scanDate: string;
    imageUrl: string;
    municipalityId?: number | null;
    municipality?: {
        id: number;
        name: string;
        population: number;
    } | null;
    userId?: number | null;
    user?: {
        id: number;
        name: string;
        age: number;
    } | null;
    detectedProducts: ScanProduct[];
};

const API_BASE_URL = import.meta.env.PUBLIC_API_BASE_URL ?? 'http://localhost:5141';

function buildApiUrl(path: string): string {
    return `${API_BASE_URL}${path}`;
}

export async function getScans(fetch: typeof window.fetch): Promise<Scan[]> {
    const res = await fetch(buildApiUrl('/api/scans'));

    if (!res.ok) {
        throw new Error(`Failed to load scans: ${res.status} ${res.statusText}`);
    }

    return res.json();
}
