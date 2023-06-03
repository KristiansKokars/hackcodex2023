export async function POST(event) {
	// For testing files locally
	const files = await (await event.request.formData()).get('files');
	console.log(files);

	return new Response();
}
