import { dev } from '$app/environment';
import { PUBLIC_BACKEND_URL } from '$env/static/public';

export async function GET({ fetch, url }) {
	if (dev) {
		// To allow self-signed certs during development to pass
		process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';
	}

	const documentId = url.searchParams.get('id');
	console.log(`Server Document ID: ${documentId}`);

	const response = await fetch(`${PUBLIC_BACKEND_URL}/docs/${documentId}`);
	return response;
}

export async function POST({ request, url, fetch }) {
	if (dev) {
		// To allow self-signed certs during development to pass
		process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';
	}

	const documentId = url.searchParams.get('id');

	// TODO: this is terrible but it is 1:31AM and I want this to just work, so JSON pain it is
	const jsonContent = await request.json();
	console.log(jsonContent);
	const response = await fetch(`${PUBLIC_BACKEND_URL}/resolve/${documentId}`, {
		method: 'POST',
		body: JSON.stringify(jsonContent)
	});
	return response;
}
