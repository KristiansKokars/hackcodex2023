import { dev } from '$app/environment';
import { PUBLIC_BACKEND_URL } from '$env/static/public';

export async function POST(event) {
	if (dev) {
		// To allow self-signed certs during development to pass
		process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';
	}

	// TODO: remove in PROD
	var userId = event.locals.user?.id;

	const response = await fetch(`${PUBLIC_BACKEND_URL}/upload/${userId}`, {
		method: 'POST',
		mode: 'cors',
		body: await event.request.formData()
	});

	return response;
}
