import { PUBLIC_BACKEND_URL } from '$env/static/public';
import { redirect, type Handle } from '@sveltejs/kit';
import { sequence } from '@sveltejs/kit/hooks';
import cookie from 'cookie';

export async function handleFetch({ event, request, fetch }) {
	if (request.url.startsWith(PUBLIC_BACKEND_URL)) {
		console.log('SETTING COOKIE!!');
		const cookieHeaders = request.headers.get('Set-Cookie');
		if (cookieHeaders !== null) {
			const aspNetCookie = cookie.parse(cookieHeaders)['.AspNetCore.cookie'] as string;
			console.log(aspNetCookie);
			request.headers.set('.AspNetCore.cookie', aspNetCookie);
		}
	}

	return fetch(request);
}

const handleAuth: Handle = async ({ event, resolve }) => {
	return await resolve(event);
};

const handleProtectedRoutes: Handle = async ({ event, resolve }) => {
	// TODO: handle reauthentication with server!
	const cookie = event.cookies.get('.AspNetCore.cookie');

	// TODO: do not remember correct code, under time constraints, fun times :D
	if (!cookie && event.url.pathname === '/') {
		throw redirect(302, '/login');
	}

	if (cookie && (event.url.pathname === '/login' || event.url.pathname === '/register')) {
		throw redirect(302, '/');
	}

	const response = await resolve(event);
	return response;
};

export const handle = sequence(handleAuth, handleProtectedRoutes);
