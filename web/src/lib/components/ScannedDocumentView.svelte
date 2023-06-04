<script lang="ts">
	export let scannedDocumentLink: string;

	$: fileExtension = determineExtension(scannedDocumentLink);

	function determineExtension(link: string): 'pdf' | 'image' {
		const linkParts = link.split('.');
		const extension = linkParts[linkParts.length - 1];

		console.log(extension);
		if (extension === 'pdf') {
			return 'pdf';
		} else {
			// TODO: Sleep is a need, so we only check for PDFs and everything else will be an image
			return 'image';
		}
	}
</script>

<!-- TODO: need to determine if link is .pdf or .png and show it here appropriately -->
{#if fileExtension === 'pdf'}
	<embed src={scannedDocumentLink} class="w-2/3 border rounded-lg p-4 h-full" />
{:else}
	<img src={scannedDocumentLink} alt="document" class="w-2/3 border rounded-lg p-4 h-full" />
{/if}
