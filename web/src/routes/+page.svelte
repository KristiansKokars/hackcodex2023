<script lang="ts">
	import { PUBLIC_BACKEND_URL } from '$env/static/public';
	import { Label, Card, Dropzone, Button } from 'flowbite-svelte';

	let files: FileList | undefined;

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

<nav class="bg-white shadow">
	<div class="mx-auto max-w-7xl px-2 sm:px-6 lg:px-8">
	  <div class="relative flex h-16 justify-between">
		<div class="absolute inset-y-0 left-0 flex items-center sm:hidden">
		  <!-- Mobile menu button -->
		  <button type="button" class="inline-flex items-center justify-center rounded-md p-2 text-gray-400 hover:bg-gray-100 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-inset focus:ring-indigo-500" aria-controls="mobile-menu" aria-expanded="false">
			<span class="sr-only">Open main menu</span>
			<!--
			  Icon when menu is closed.
  
			  Menu open: "hidden", Menu closed: "block"
			-->
			<svg class="block h-6 w-6" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" aria-hidden="true">
			  <path stroke-linecap="round" stroke-linejoin="round" d="M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25h16.5" />
			</svg>
			<!--
			  Icon when menu is open.
  
			  Menu open: "block", Menu closed: "hidden"
			-->
			<svg class="hidden h-6 w-6" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" aria-hidden="true">
			  <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
			</svg>
		  </button>
		</div>
		<div class="flex flex-1 items-center justify-center sm:items-stretch sm:justify-start">
		  <div class="flex flex-shrink-0 items-center">
			<img class="block h-8 w-auto lg:hidden" src="https://tailwindui.com/img/logos/mark.svg?color=indigo&shade=600" alt="Your Company">
			<img class="hidden h-8 w-auto lg:block" src="https://tailwindui.com/img/logos/mark.svg?color=indigo&shade=600" alt="Your Company">
		  </div>
		</div>
		<div class="absolute inset-y-0 right-0 flex items-center pr-2 sm:static sm:inset-auto sm:ml-6 sm:pr-0">
		  <button type="button" class="rounded-full bg-white p-1 text-gray-400 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2">
			<a href="#" class="block px-4 py-2 text-sm text-gray-700" role="menuitem" tabindex="-1" id="user-menu-item-0">Log Out</a>
		  </button>
  
		  <!-- Profile dropdown -->
		  <div class="relative ml-3">
  
			<!--
			  Dropdown menu, show/hide based on menu state.
  
			  Entering: "transition ease-out duration-200"
				From: "transform opacity-0 scale-95"
				To: "transform opacity-100 scale-100"
			  Leaving: "transition ease-in duration-75"
				From: "transform opacity-100 scale-100"
				To: "transform opacity-0 scale-95"
			-->
		  </div>
		</div>
	  </div>
	</div>
  
	<!-- Mobile menu, show/hide based on menu state. -->
	<div class="sm:hidden" id="mobile-menu">
	  <div class="space-y-1 pb-4 pt-2">
		<!-- Current: "bg-indigo-50 border-indigo-500 text-indigo-700", Default: "border-transparent text-gray-500 hover:bg-gray-50 hover:border-gray-300 hover:text-gray-700" -->
		<a href="#" class="block border-l-4 border-indigo-500 bg-indigo-50 py-2 pl-3 pr-4 text-base font-medium text-indigo-700">Dashboard</a>
		<a href="#" class="block border-l-4 border-transparent py-2 pl-3 pr-4 text-base font-medium text-gray-500 hover:border-gray-300 hover:bg-gray-50 hover:text-gray-700">Team</a>
		<a href="#" class="block border-l-4 border-transparent py-2 pl-3 pr-4 text-base font-medium text-gray-500 hover:border-gray-300 hover:bg-gray-50 hover:text-gray-700">Projects</a>
		<a href="#" class="block border-l-4 border-transparent py-2 pl-3 pr-4 text-base font-medium text-gray-500 hover:border-gray-300 hover:bg-gray-50 hover:text-gray-700">Calendar</a>
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
			{#each files as file}
				<Label>File: {file.name}</Label>
			{/each}
		{/if}
		<Button on:click={sendFiles} color="dark">Send Files</Button>
	</Card>
</div>
