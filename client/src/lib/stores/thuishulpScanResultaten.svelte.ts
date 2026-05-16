    export type RiskLevel = "Veilig" | "Riskant" | "Onveilig"

    type Ghs = {
        type: string;
        description: string;
    }

    type Alternative = {
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