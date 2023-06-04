<script lang="ts">
	import { page } from '$app/stores';
	import type { DocumentRow } from '$lib/features/documents/DocumentRow.js';
	import { Label, Input, Button, Card } from 'flowbite-svelte';

	export let data;

	interface FormField {
		label: string;
		value: string;
		confidence: number;
	}

	// TODO: there was a better way to rename page data, but HACKATHON COUNTDOWN IS CALLING
	const scannedDocument = data;
	const documentId = $page.params.page;

	const isDocumentFaulty = data.status === 'Faulty';

	let formFields: FormField[] = [];

	for (const [key, value] of Object.entries(scannedDocument.content)) {
		const documentValue = value as DocumentRow;
		const fields: FormField[] = [];

		fields.push({
			label: key,
			value: documentValue.Value,
			confidence: documentValue.Confidence
		});

		formFields = fields;
	}

	async function editOnBackend() {
		await fetch(`/edit?id=${documentId}`);
	}

	$: {
		console.log(scannedDocument.content);
	}
</script>

<svelte:head>
	<title>Document - {documentId}</title>
</svelte:head>

<div class="grid grid-cols-2 divide-x grow">
	<div class="flex items-center justify-center h-full">
		<embed src={scannedDocument.link} class="w-2/3 border rounded-lg p-4 h-full" />
	</div>
	<div class="flex flex-col items-center justify-center">
		{#if isDocumentFaulty}
			<Card color="red" class="mb-4"
				>OMG YOUR DOCUMENT IS FUCKED, HELP THE AI FIX IT WITH YOUR HUMAN POWERS</Card
			>
			{#each formFields as formField}
				<div class="mb-6 w-96">
					<Label for="large-input" class="block mb-2">{formField.label}</Label>
					<Input
						class="h-8 rounded-none"
						id={formField.label}
						size="lg"
						placeholder={formField.label}
					/>
				</div>
			{/each}
			<Button class="w-56" color="green">Update</Button>
		{:else}
			<p>Your document is cool</p>
		{/if}
	</div>
</div>
