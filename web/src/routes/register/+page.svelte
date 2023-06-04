<script lang="ts">
	import register from '$lib/assets/register.jpg';
	import { goto } from '$app/navigation';
	import type { SimpleErrorMessage } from '$lib/common/SimpleErrorMessage';
	import { Button, Card, Input, Label, Spinner } from 'flowbite-svelte';
	import HackathonCallout from '$lib/components/HackathonCallout.svelte';

	let username = '';
	let password = '';
	let errorMessage: string | undefined;
	let isRegistering = false;

	async function loginUser() {
		errorMessage = '';
		isRegistering = true;
		const response = await fetch('/register', {
			method: 'POST',
			// TODO: this would need a double check for proper auth header use laterTM
			headers: {
				authorization: 'Basic ' + btoa(username + ':' + password)
			}
		});

		isRegistering = false;
		if (response.redirected) {
			goto('/');
		}
		if (!response.ok) {
			errorMessage = ((await response.json()) as SimpleErrorMessage).message;
		}
	}
</script>

<div class="flex w-full min-h-screen">
	<div
		class="flex flex-1 flex-col justify-center px-4 py-12 sm:px-6 lg:flex-none lg:px-20 xl:px-24"
	>
		<div class="mx-auto w-full max-w-sm lg:w-96">
			<div>
				<HackathonCallout />
				<!-- TODO: this causes a cumulative layout shift, which is not good for UX, but time constraints force to keep it this way for now -->
				{#if errorMessage}
					<Card color="red" class="mb-4"
						><p>Failed to login.</p>
						<p>Error: {errorMessage}</p></Card
					>
				{/if}
				<h2 class="mt-8 text-2xl font-bold leading-9 tracking-tight text-gray-900">Register</h2>
				<p class="mt-2 text-sm leading-6 text-gray-500">
					Have an account?
					<a href="/login" class="font-semibold text-indigo-600 hover:text-indigo-500">Login</a>
				</p>
			</div>

			<div class="mt-10">
				<div>
					<form action="#" method="POST" class="space-y-6">
						<div>
							<Label for="username">Username</Label>
							<div class="mt-2">
								<Input id="username" required bind:value={username} />
							</div>
						</div>

						<div>
							<Label for="password">Password</Label>
							<div class="mt-2">
								<Input
									id="password"
									type="password"
									autocomplete="current-password"
									required
									bind:value={password}
								/>
							</div>
						</div>

						<div>
							<Button color="dark" class="w-full" on:click={loginUser} enabled={!isRegistering}
								>{#if isRegistering}
									<Spinner />
								{:else}
									Register
								{/if}
							</Button>
						</div>
					</form>
				</div>
			</div>
		</div>
	</div>
	<div class="relative hidden w-0 flex-1 lg:block">
		<img class="absolute inset-0 h-full w-full object-cover" src={register} alt="" />
	</div>
</div>
