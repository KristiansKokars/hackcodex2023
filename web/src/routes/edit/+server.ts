import { dev } from '$app/environment';
import { PUBLIC_BACKEND_URL } from '$env/static/public';

export async function GET({ fetch, request, locals, cookies }) {
	if (dev) {
		// To allow self-signed certs during development to pass
		process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';
	}

    const data = await request.json();
    const documentId = data.id as string;

	const response = await fetch(`${PUBLIC_BACKEND_URL}/docs/${documentId}`);
	return response;
}
