<script lang="ts">
	import { page } from '$app/stores';
	import { Button } from 'flowbite-svelte';

	const documentId = $page.params.page;

	async function editOnBackend() {
		await fetch(`/edit?id=${documentId}`);
	}
</script>

<h1>Document of {documentId}</h1>
<Button on:click={editOnBackend} color="dark">Edit</Button>
