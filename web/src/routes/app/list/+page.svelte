<script lang="ts">
	import type { ScannedDocument } from '$lib/features/documents/ScannedDocument';
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

	let items: {
		id: string;
		mark: string;
		statuss: string;
		doc_num: string; // TODO: not named correctly to naming conventions, problem for laaaaater
		// TODO: add created date in backend
		date: number;
		link: string;
	}[] = [];

	async function getDocuments() {
		const response = await fetch('/app/list');
		const jsonData = await response.json();
		const scannedDocuments = jsonData as ScannedDocument[];

		items = scannedDocuments.map((scannedDocument) => {
			const rowMark = scannedDocument.status == 'Correct' ? '✅' : '❌';

			const item = {
				id: scannedDocument.id,
				mark: rowMark,
				statuss: scannedDocument.status,
				doc_num: scannedDocument.invoiceId,
				// TODO: add created date in backend
				date: scannedDocument.createdAt,
				link: scannedDocument.link
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

<Table class="mx-auto max-w-7xl px-2 sm:px-6 lg:px-8 mt-8" hoverable={true}>
	<TableHead>
		<TableHeadCell on:click={() => sortTable('mark')} />
		<TableHeadCell on:click={() => sortTable('statuss')}>Statuss</TableHeadCell>
		<TableHeadCell on:click={() => sortTable('doc_num')}>Document Number</TableHeadCell>
		<TableHeadCell on:click={() => sortTable('date')}>Upload Date</TableHeadCell>
		<TableHeadCell on:click={() => sortTable('link')}>Document Link</TableHeadCell>
		<TableHeadCell />
	</TableHead>
	<TableBody>
		{#each items as document}
			<TableBodyRow>
				<TableBodyCell>{document.mark}</TableBodyCell>
				<TableBodyCell>{document.statuss}</TableBodyCell>
				<TableBodyCell>{document.doc_num}</TableBodyCell>
				<TableBodyCell>{document.date}</TableBodyCell>
				<TableBodyCell
					><a href={document.link} class="text-purple-400 hover:text-purple-600" target="_blank"
						>Download</a
					></TableBodyCell
				>
				<TableBodyCell
					><a class="text-purple-400 hover:text-purple-600" href={`/app/edit/${document.id}`}
						>Open</a
					></TableBodyCell
				>
			</TableBodyRow>
		{/each}
	</TableBody>
</Table>
{#if items.length == 0}
	<div class="grid divide-x grow">
		<div class="flex flex-col items-center justify-center">
			<p>There are no documents yet!</p>
		</div>
	</div>
{/if}
