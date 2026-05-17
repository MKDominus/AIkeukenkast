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
        productId: number;
        productName: string;
        productType: string;
        imageURL?: string | null;
        riskLevel: string;
        warningLabels: { type: string; description: string }[];
        dangers: string[];
        precautions: string[];
        alternatives: string[];
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

export type ScanStats = {
    totalScans: number;
    productsScanned: number;
    averageSafety: number;
    averageRisk: number;
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

export async function getScanStats(fetch: typeof window.fetch): Promise<ScanStats> {
    const res = await fetch(buildApiUrl('/api/scans/stats'));

    if (!res.ok) {
        throw new Error(`Failed to load scan stats: ${res.status} ${res.statusText}`);
    }

    return res.json();
}
