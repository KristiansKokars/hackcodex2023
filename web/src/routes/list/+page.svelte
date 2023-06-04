<script lang="ts">
	import NavBar from '$lib/components/NavBar.svelte';
	import {
		Table,
		TableBody,
		TableBodyCell,
		TableBodyRow,
		TableHead,
		TableHeadCell
	} from 'flowbite-svelte';
	import { onMount } from 'svelte';
	import { writable } from 'svelte/store';

	interface ScanDocument {
		id: string;
		content: DocumentInvoiceId;
		link: string;
		status: string;
		createdAt: number;
	}

	interface DocumentInvoiceId {
		invoiceId: DocumentRow;
	}

	interface DocumentRow {
		Value: string;
		Confidence: number;
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

	let items: {
		mark: string;
		statuss: string;
		doc_num: string;
		// TODO: add created date in backend
		date: number;
		link: string;
	}[] = [];

	async function getDocuments() {
		const response = await fetch('/list');
		const jsonData = await response.json();
		const data = jsonData as ScanDocument[];

		items = data.map((data) => {
			const rowMark = data.status == 'Correct' ? '✅' : '❌';

			const item = {
				mark: rowMark,
				statuss: data.status,
				doc_num: data.content.invoiceId.Value,
				// TODO: add created date in backend
				date: data.createdAt,
				link: data.link
			};
			return item;
		});
	}

	onMount(() => {
		getDocuments();
	});

	const sortKey = writable('id'); // default sort key
	const sortDirection = writable(1); // default sort direction (ascending)
	const sortItems = writable(items.slice()); // make a copy of the items array

	$: {
		const key = $sortKey;
		const direction = $sortDirection;
		const sorted = [...$sortItems].sort((a, b) => {
			const aVal = a[key];
			const bVal = b[key];
			if (aVal < bVal) {
				return -direction;
			} else if (aVal > bVal) {
				return direction;
			}
			return 0;
		});
		sortItems.set(sorted);
	}

	// Define a function to sort the items
	const sortTable = (key) => {
		// If the same key is clicked, reverse the sort direction
		if ($sortKey === key) {
			sortDirection.update((val) => -val);
		} else {
			sortKey.set(key);
			sortDirection.set(1);
		}
	};
</script>

<NavBar />

<Table class="mx-auto max-w-7xl px-2 sm:px-6 lg:px-8 mt-8" hoverable={true}>
	<TableHead>
		<TableHeadCell on:click={() => sortTable('mark')} />
		<TableHeadCell on:click={() => sortTable('statuss')}>Statuss</TableHeadCell>
		<TableHeadCell on:click={() => sortTable('doc_num')}>Document Number</TableHeadCell>
		<TableHeadCell on:click={() => sortTable('date')}>Upload Date</TableHeadCell>
		<TableHeadCell on:click={() => sortTable('link')}>Link</TableHeadCell>
		<TableHeadCell />
	</TableHead>
	<TableBody>
		{#each items as item}
			<TableBodyRow>
				<TableBodyCell>{item.mark}</TableBodyCell>
				<TableBodyCell>{item.statuss}</TableBodyCell>
				<TableBodyCell>{item.doc_num}</TableBodyCell>
				<TableBodyCell>{item.date}</TableBodyCell>
				<TableBodyCell>{item.link}</TableBodyCell>
				<TableBodyCell><a class="text-purple-400 hover:text-purple-600">Open</a></TableBodyCell>
			</TableBodyRow>
		{/each}
	</TableBody>
</Table>
