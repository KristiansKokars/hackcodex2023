<script lang="ts">
	import { Label, Fileupload, Card } from 'flowbite-svelte';

	let fileuploadprops = {
		id: 'document'
	};
</script>

<div class="min-h-screen m-auto flex justify-center items-center max-w-screen-md">
	<Card>
		<Label class="pb-2">Upload Document</Label>
		<Fileupload {...fileuploadprops} />
	</Card>
</div>
