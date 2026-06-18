import { env } from '$env/dynamic/public';
const API_BASE_URL = env.PUBLIC_API_BASE_URL ?? 'http://localhost:5141';

function buildApiUrl(path: string): string {
    return `${API_BASE_URL}${path}`;
}

export type MunicipalityScanCount = {
    municipalityId: number;
    municipalityName: string;
    scanCount: number;
};

export async function getMunicipalityScanCounts(fetch: typeof window.fetch): Promise<MunicipalityScanCount[]> {
    const res = await fetch(buildApiUrl('/api/municipalities/scan-counts'));

    if (!res.ok) {
        throw new Error(`Failed to load municipality scan counts: ${res.status} ${res.statusText}`);
    }

    return res.json();
}
