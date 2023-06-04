<script lang="ts">
	import type { FormField } from '$lib/features/documents/FormField';
	import { Label, Progressbar, Input } from 'flowbite-svelte';

	export let formField: FormField;
</script>

<div class="mb-6 w-96">
	<Label for="large-input" class="block mb-2">{formField.label}</Label>
	<Label>Confidence: {formField.confidence.toPrecision(2)}%</Label>
	<Progressbar
		progress={`${formField.confidence}`}
		color={formField.isHigherThanMinimumConfidence ? 'green' : 'red'}
	/>
	<Input
		class="h-8 rounded-none"
		id={formField.label}
		size="lg"
		placeholder={formField.value}
		bind:value={formField.value}
	/>
</div>
