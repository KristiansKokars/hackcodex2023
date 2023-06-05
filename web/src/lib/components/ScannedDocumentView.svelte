<script lang="ts">
	export let scannedDocumentLink: string;

	$: fileExtension = determineExtension(scannedDocumentLink);

	function determineExtension(link: string): 'pdf' | 'image' {
		const linkParts = link.split('.');
		const extension = linkParts[linkParts.length - 1];

		if (extension === 'pdf') {
			return 'pdf';
		} else {
			// TODO: Sleep is a need, so we only check for PDFs and everything else will be an image
			return 'image';
		}
	}
</script>

{#if fileExtension === 'pdf'}
	<embed
		title="document"
		type="application/pdf"
		src={scannedDocumentLink}
		class="w-2/3 border rounded-lg p-4 h-full"
	/>
{:else}
	<img src={scannedDocumentLink} alt="document" class="w-2/3 border rounded-lg p-4 h-full" />
{/if}
