import { dev } from '$app/environment';
import { PUBLIC_BACKEND_URL } from '$env/static/public';

export async function GET({ fetch, request, locals, cookies, url }) {
	if (dev) {
		// To allow self-signed certs during development to pass
		process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';
	}

	const documentId = url.searchParams.get('id');
	console.log(`Server Document ID: ${documentId}`);

	const response = await fetch(`${PUBLIC_BACKEND_URL}/docs/${documentId}`);
	return response;
}
