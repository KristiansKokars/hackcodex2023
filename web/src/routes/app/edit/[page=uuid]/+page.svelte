<script lang="ts">
	import { page } from '$app/stores';
	import type { DocumentRow } from '$lib/features/documents/DocumentRow.js';
	import type { FormField } from '$lib/features/documents/FormField.js';
	import { Label, Input, Button, Card, Progressbar } from 'flowbite-svelte';
	import DynamicFormInput from './DynamicFormInput.svelte';
	import ScannedDocumentView from '$lib/components/ScannedDocumentView.svelte';
	import { goto } from '$app/navigation';

	export let data;

	// TODO: there was a better way to rename page data, but HACKATHON COUNTDOWN IS CALLING
	const scannedDocument = data;
	const documentId = $page.params.page;

	const isDocumentFaulty = data.status === 'Faulty';

	let originalDocumentRows: Map<string, DocumentRow> = new Map();
	let formFields: FormField[] = [];

	for (const [key, value] of Object.entries(scannedDocument.content)) {
		const documentValue = value as DocumentRow | null;

		if (documentValue === null) {
			continue;
		}

		originalDocumentRows.set(key, documentValue);

		console.log(`Min Percent: ${scannedDocument.minAllowedPercent}`);

		formFields.push({
			label: key,
			value: documentValue.Value,
			confidence: documentValue.Confidence * 100,
			isHigherThanMinimumConfidence: documentValue.Confidence > scannedDocument.minAllowedPercent
		});
	}

	formFields = formFields;

	async function updateWithoutEdits() {
		const response = await fetch(`/app/edit?id=${documentId}`, {
			method: 'POST',
			body: JSON.stringify(Object.fromEntries(originalDocumentRows))
		});

		if (response.ok) {
			goto('/app/list');
		}
	}

	async function update() {
		const newDocumentMap = new Map<string, DocumentRow>();

		for (const formField of formFields) {
			newDocumentMap.set(formField.label, {
				Value: formField.value,
				Confidence: formField.confidence
			});
		}

		const newDocumentRows = JSON.stringify(Object.fromEntries(newDocumentMap));
		// TODO: someone do something with this response and show loading/success/error, I want to sleep
		const response = await fetch(`/app/edit?id=${documentId}`, {
			method: 'POST',
			body: newDocumentRows
		});

		if (response.ok) {
			goto('/app/list');
		}
	}
</script>

<svelte:head>
	<title>Document - {documentId}</title>
</svelte:head>

<div class="grid grid-cols-2 divide-x grow">
	<div class="flex items-center justify-center h-full">
		<ScannedDocumentView scannedDocumentLink={scannedDocument.link} />
	</div>
	<div class="flex flex-col items-center justify-center">
		{#if isDocumentFaulty}
			<Card color="red" class="mb-4 mt-4"
				>Document has problems, see below fields for confidence ratings and edit if neccessary.</Card
			>
			<Button class="w-56 mb-3" color="green" on:click={updateWithoutEdits}
				>Send Original AI Fields</Button
			>
			<Button class="w-56 mb-5" color="green" on:click={update}>Update with current values</Button>
			{#each formFields as formField}
				<DynamicFormInput bind:formField />
			{/each}
		{:else}
			<p>Your document is cool</p>
		{/if}
	</div>
</div>
