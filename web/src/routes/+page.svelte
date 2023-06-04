<script lang="ts">
	import { goto } from '$app/navigation';
	import { PUBLIC_BACKEND_URL } from '$env/static/public';
	import Logo from '$lib/components/Logo.svelte';
	import { Label, Card, Dropzone, Button } from 'flowbite-svelte';

	let files: FileList | undefined;

	function sendFiles() {
		console.log('sending files');
		const filesFormData = new FormData();

		if (!files) return;

		for (const file of files) {
			filesFormData.append('files', file);
		}

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

	async function logOut() {
		await fetch('/logout', {
			method: 'POST'
		});
		goto('/login');
	}
</script>

<nav class="bg-white shadow">
	<div class="mx-auto max-w-7xl px-2 sm:px-6 lg:px-8">
		<div class="relative flex h-16 justify-between">
			<div class="absolute inset-y-0 left-0 flex items-center sm:hidden">
				<!-- Mobile menu button -->
				<div
					class="inline-flex items-center justify-center rounded-md p-2 text-gray-400 hover:bg-gray-100 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-inset focus:ring-indigo-500"
					aria-controls="mobile-menu"
					aria-expanded="false"
				>
					<span class="sr-only">Open main menu</span>
					<!--
            Icon when menu is closed.

            Menu open: "hidden", Menu closed: "block"
          -->
					<Button
						on:click={logOut}
						class="inline-flex items-center border-b-2 border-transparent px-1 pt-1 text-sm font-medium text-gray-900"
						>Log Out</Button
					>
					<!--
            Icon when menu is open.

            Menu open: "block", Menu closed: "hidden"
          -->
					<Button
						on:click={logOut}
						class="inline-flex items-center border-b-2 border-transparent px-1 pt-1 text-sm font-medium text-gray-900"
						>Log Out</Button
					>
				</div>
			</div>
			<div class="flex flex-1 items-center justify-center sm:items-stretch sm:justify-start">
				<Logo />
				<div class="hidden sm:ml-6 sm:flex sm:space-x-8">
					<!-- Current: "border-indigo-500 text-gray-900", Default: "border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700" -->
					<a
						href="/list"
						class="inline-flex items-center border-b-2 border-transparent px-1 pt-1 text-sm font-medium text-gray-500 hover:border-gray-300 hover:text-gray-700"
						>List</a
					>
				</div>
			</div>
			<div
				class="absolute inset-y-0 right-0 flex items-center pr-2 sm:static sm:inset-auto sm:ml-6 sm:pr-0"
			>
				<a class="mr-4" href="#"
					><button
						type="button"
						class="relative inline-flex items-center gap-x-1.5 rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600"
					>
						<svg class="-ml-0.5 h-5 w-5" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
							<path
								d="M10.75 4.75a.75.75 0 00-1.5 0v4.5h-4.5a.75.75 0 000 1.5h4.5v4.5a.75.75 0 001.5 0v-4.5h4.5a.75.75 0 000-1.5h-4.5v-4.5z"
							/>
						</svg>
						Upload File
					</button></a
				>
				<Button color="alternative" on:click={logOut}>Log Out</Button>

				<!-- Profile dropdown -->
				<div class="relative ml-3" />
			</div>
		</div>
	</div>

	<!-- Mobile menu, show/hide based on menu state. -->
	<div class="sm:hidden" id="mobile-menu">
		<div class="space-y-1 pb-4 pt-2">
			<!-- Current: "bg-indigo-50 border-indigo-500 text-indigo-700", Default: "border-transparent text-gray-500 hover:bg-gray-50 hover:border-gray-300 hover:text-gray-700" -->
			<a
				href="/list"
				class="block border-l-4 border-transparent py-2 pl-3 pr-4 text-base font-medium text-gray-500 hover:border-gray-300 hover:bg-gray-50 hover:text-gray-700"
				>List</a
			>
			<a
				href="#"
				class="block border-l-4 border-indigo-500 bg-indigo-50 py-2 pl-3 pr-4 text-base font-medium text-indigo-700"
				>Upload File</a
			>
		</div>
	</div>
</nav>

<div class="min-h-screen m-auto flex justify-center items-center max-w-screen-md">
	<Card class="w-96">
		<Label class="pb-2">Upload Document</Label>
		<Dropzone id="dropzone" bind:files>
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
			<div class="mt-4">
				{#each files as file}
					<Label>File: {file.name}</Label>
				{/each}
			</div>
		{/if}
		<Button on:click={sendFiles} color="dark" class="mt-4">Send Files</Button>
	</Card>
</div>
