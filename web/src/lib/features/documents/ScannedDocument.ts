export interface ScannedDocument {
	id: string;
	invoiceId: string;
	content: object;
	link: string;
	status: 'Faulty' | 'Correct';
	createdAt: number;
}

// interface ScanDocumentContent {
// 	invoiceDate: DocumentRow;
// 	invoiceId: DocumentRow;
// 	totalWithNoTax: DocumentRow;
// 	totalTax: DocumentRow;
// 	total: DocumentRow;
// 	vendorAddress: DocumentRow;
// 	buyerAddress: DocumentRow;
// 	dateToPay: DocumentRow;
// 	vendorRegNum: DocumentRow;
// 	buyerRegNum: DocumentRow;
// 	vendorCompanyName: DocumentRow;
// 	buyerCompanyName: DocumentRow;
// 	vendorBankAccount: DocumentRow;
// 	buyerBankAccount: DocumentRow;
// 	vendorPVNnum: DocumentRow;
// 	buyerPVNnum: DocumentRow;
// 	physicalBuyerName: DocumentRow;
// }
