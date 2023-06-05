export interface ScannedDocument {
	id: string;
	invoiceId: string;
	content: object;
	link: string;
	status: 'Faulty' | 'Correct';
	minAllowedPercent: number;
	createdAt: number;
}
