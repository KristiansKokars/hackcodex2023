import { PUBLIC_BACKEND_URL } from '$env/static/public';
import type { User } from '$lib/features/auth/User';
import { redirect, type Handle } from '@sveltejs/kit';
import { sequence } from '@sveltejs/kit/hooks';

export async function handleFetch({ event, request, fetch }) {
	const sessionUserText = event.cookies.get('sessionUser');

	if (request.url.startsWith(PUBLIC_BACKEND_URL) && sessionUserText !== undefined) {
		const token = JSON.parse(sessionUserText) as User;
		request.headers.append('Authorization', 'Bearer ' + token);
	}

	return fetch(request);
}

const handleAuth: Handle = async ({ event, resolve }) => {
	return await resolve(event);
};

const handleProtectedRoutes: Handle = async ({ event, resolve }) => {
	const isLoggedIn = event.cookies.get('sessionUser') !== undefined;

	// TODO: do not remember correct code, under time constraints, fun times :D
	if (!isLoggedIn && event.url.pathname === '/') {
		throw redirect(302, '/login');
	}

	if (isLoggedIn && (event.url.pathname === '/login' || event.url.pathname === '/register')) {
		throw redirect(302, '/');
	}

	const response = await resolve(event);
	return response;
};

export const handle = sequence(handleAuth, handleProtectedRoutes);
