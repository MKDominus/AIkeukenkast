    export type RiskLevel = "Veilig" | "Riskant" | "Onveilig"

    export type Ghs = {
        type: string;
        description: string;
    }

    export type Alternative = {
        imageURL?: string;
        productName: string;      
        productType: string;
    }


	export type ScannedProduct = {
		imageURL?: string;
		productName: string;
		productType: string;
        productId: number;
		warningLabels?: Ghs[];
		riskLevel: RiskLevel;
        dangers?: string[];
        precautions?: string[];
        alternatives?: Alternative[];
	}

    export const thuishulpScanResultaten = $state({
        scannedProducts: [] as ScannedProduct[]
    })