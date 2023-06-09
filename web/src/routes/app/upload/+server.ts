import { dev } from '$app/environment';
import { PUBLIC_BACKEND_URL } from '$env/static/public';

export async function POST(event) {
	if (dev) {
		// To allow self-signed certs during development to pass
		process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';
	}

	const filesFormData = await event.request.formData();
	const response = await event.fetch(`${PUBLIC_BACKEND_URL}/upload`, {
		method: 'POST',
		mode: 'cors',
		body: filesFormData
	});

	return response;
}
