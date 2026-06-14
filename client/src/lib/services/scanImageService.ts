const API_BASE_URL = import.meta.env.PUBLIC_API_BASE_URL ?? 'http://localhost:5141';

function buildApiUrl(path: string): string {
	return `${API_BASE_URL}${path}`;
}

export async function getScanImageObjectUrl(fetch: typeof window.fetch, scanId: number): Promise<string> {
	const res = await fetch(buildApiUrl(`/api/scans/${scanId}/image`));

	if (!res.ok) {
		throw new Error(`Failed to load scan image: ${res.status} ${res.statusText}`);
	}

	const blob = await res.blob();
	return URL.createObjectURL(blob);
}