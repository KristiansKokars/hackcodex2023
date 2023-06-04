import { PUBLIC_BACKEND_URL } from '$env/static/public';
import type { User } from '$lib/features/auth/User';
import { redirect, type Handle } from '@sveltejs/kit';
import { sequence } from '@sveltejs/kit/hooks';

const unauthorizedRoutes = ['/login', 'register'];

export async function handleFetch({ event, request, fetch }) {
	const sessionUserToken = event.cookies.get('sessionUser');

	if (request.url.startsWith(PUBLIC_BACKEND_URL) && sessionUserToken !== undefined) {
		const token = JSON.parse(sessionUserToken) as string;
		request.headers.append('Authorization', 'Bearer ' + token);
	}

	const response = await fetch(request);

	// TODO: Any unauthorized error will be considered for now as "token expired", you would check more specifically in production!
	if (response.status === 401) {
		console.log('deleting token');
		event.cookies.delete('sessionUser');
		throw redirect(302, '/login');
	}

	return response;
}

const handleAuth: Handle = async ({ event, resolve }) => {
	return await resolve(event);
};

const handleProtectedRoutes: Handle = async ({ event, resolve }) => {
	const isLoggedIn = event.cookies.get('sessionUser') !== undefined;

	// TODO: do not remember correct HTTP code under time constraints, fun times :D
	if (!isLoggedIn && !unauthorizedRoutes.includes(event.url.pathname)) {
		throw redirect(302, '/login');
	}

	if (isLoggedIn && unauthorizedRoutes.includes(event.url.pathname)) {
		throw redirect(302, '/');
	}

	const response = await resolve(event);
	return response;
};

export const handle = sequence(handleAuth, handleProtectedRoutes);
