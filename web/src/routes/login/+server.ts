import { dev } from '$app/environment';
import { PUBLIC_BACKEND_URL } from '$env/static/public';
import type { User } from '$lib/features/auth/User.js';
import { redirect } from '@sveltejs/kit';
import cookie from 'cookie';

export async function POST({ fetch, request, locals, cookies }) {
	if (dev) {
		// To allow self-signed certs during development to pass
		process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';
	}

	const authorizationHeader = request.headers.get('authorization');

	if (authorizationHeader === null) {
		return new Response(JSON.stringify({ message: 'No auth info sent!' }), {
			status: 400
		});
	}

	const response = await fetch(`${PUBLIC_BACKEND_URL}/login`, {
		method: 'POST',
		mode: 'cors',
		headers: {
			authorization: authorizationHeader
		}
	});

	if (response.ok) {
		const user = (await response.json()) as User;
		locals.user = user;

		const aspNetCookie = cookie.parse(response.headers.get('Set-Cookie'))['.AspNetCore.cookie'];
		console.log(aspNetCookie);

		cookies.set('.AspNetCore.cookie', aspNetCookie, {
			path: '/',
			httpOnly: true,
			secure: true,
			domain: 'localhost'
		});

		throw redirect(302, '/');
	}

	return response;
}
