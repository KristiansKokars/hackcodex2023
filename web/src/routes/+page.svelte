<script lang="ts">
	import { PUBLIC_BACKEND_URL } from '$env/static/public';
	import { Label, Card, Dropzone, Button } from 'flowbite-svelte';

	let value: string | undefined;
	let files: FileList | undefined;

	$: {
		console.log(files);
	}

	function sendFiles() {
		console.log('sending files');
		const filesFormData = new FormData();

		if (!files) return;

		for (const file of files) {
			filesFormData.append('files', file);
		}

		console.log(`${PUBLIC_BACKEND_URL}/upload`);
		try {
			fetch(`/upload`, {
				method: 'POST',
				mode: 'cors',
				body: filesFormData
			});
		} catch (error) {
			console.error(error);
		}
	}
</script>

<div class="min-h-screen m-auto flex justify-center items-center max-w-screen-md">
	<Card class="w-96">
		<Label class="pb-2">Upload Document</Label>
		<Dropzone id="dropzone" bind:value bind:files>
			<svg
				aria-hidden="true"
				class="mb-3 w-10 h-10 text-gray-400"
				fill="none"
				stroke="currentColor"
				viewBox="0 0 24 24"
				xmlns="http://www.w3.org/2000/svg"
				><path
					stroke-linecap="round"
					stroke-linejoin="round"
					stroke-width="2"
					d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12"
				/></svg
			>
			<p class="mb-2 text-sm text-gray-500 dark:text-gray-400">
				<span class="font-semibold">Click to upload</span> or drag and drop
			</p>
			<p class="text-xs text-gray-500 dark:text-gray-400">PDF, PNG, JPEG</p>
		</Dropzone>

		{#if files}
			{#each files as file}
				<Label>File: {file.name}</Label>
			{/each}
		{/if}
		<Button on:click={sendFiles} color="dark">Send Files</Button>
	</Card>
</div>
