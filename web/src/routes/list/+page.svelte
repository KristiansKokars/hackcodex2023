<script lang="ts">
	import { PUBLIC_BACKEND_URL } from '$env/static/public';
	import NavBar from '$lib/components/NavBar.svelte';
	import {
		Button,
		Table,
		TableBody,
		TableBodyCell,
		TableBodyRow,
		TableHead,
		TableHeadCell
	} from 'flowbite-svelte';
	import { writable } from 'svelte/store';

	let items = [
		{
			mark: 1,
			statuss: 'Success',
			doc_num: '1111222',
			supplier_num: 'SUP1122',
			doc_date: '04.06.2023',
			uploader: 'Ludvig Hero',
			date: '04.06.2023'
		},
		{
			mark: 0,
			statuss: 'Needs validation',
			doc_num: '1111222',
			supplier_num: 'SUP1122',
			doc_date: '04.06.2023',
			uploader: 'Ludvig Hero',
			date: '04.06.2023'
		},
		{
			mark: 1,
			statuss: 'Success',
			doc_num: '1111222',
			supplier_num: 'SUP1122',
			doc_date: '04.06.2023',
			uploader: 'Ludvig Hero',
			date: '04.06.2023'
		},
		{
			mark: 1,
			statuss: 'Success',
			doc_num: '1111222',
			supplier_num: 'SUP1122',
			doc_date: '04.06.2023',
			uploader: 'Ludvig Hero',
			date: '04.06.2023'
		}
	];

	const sortKey = writable('id'); // default sort key
	const sortDirection = writable(1); // default sort direction (ascending)
	const sortItems = writable(items.slice()); // make a copy of the items array

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

	async function getDocuments() {
		await fetch('/list');
	}
</script>

<NavBar />

<Table class="mx-auto max-w-7xl px-2 sm:px-6 lg:px-8 mt-8" hoverable={true}>
	<TableHead>
		<TableHeadCell on:click={() => sortTable('mark')} />
		<TableHeadCell on:click={() => sortTable('statuss')}>Statuss</TableHeadCell>
		<TableHeadCell on:click={() => sortTable('doc_num')}>Document Number</TableHeadCell>
		<TableHeadCell on:click={() => sortTable('supplier_num')}>Supplier Number</TableHeadCell>
		<TableHeadCell on:click={() => sortTable('doc_date')}>Document Date</TableHeadCell>
		<TableHeadCell on:click={() => sortTable('uploader')}>Uploader</TableHeadCell>
		<TableHeadCell on:click={() => sortTable('date')}>Upload Date</TableHeadCell>
		<TableHeadCell />
	</TableHead>
	<TableBody>
		{#each $sortItems as item}
			<TableBodyRow>
				<TableBodyCell>{item.mark}</TableBodyCell>
				<TableBodyCell>{item.statuss}</TableBodyCell>
				<TableBodyCell>{item.doc_num}</TableBodyCell>
				<TableBodyCell>{item.supplier_num}</TableBodyCell>
				<TableBodyCell>{item.doc_date}</TableBodyCell>
				<TableBodyCell>{item.uploader}</TableBodyCell>
				<TableBodyCell>{item.date}</TableBodyCell>
				<TableBodyCell><a class="text-purple-400 hover:text-purple-600">Open</a></TableBodyCell>
			</TableBodyRow>
		{/each}
	</TableBody>
</Table>
