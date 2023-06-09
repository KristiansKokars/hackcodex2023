import { PUBLIC_BACKEND_URL } from '$env/static/public';
import type { ScannedDocument } from '$lib/features/documents/ScannedDocument';
import { error, redirect } from '@sveltejs/kit';

export async function load({ fetch, params }) {
	const documentId = params.page;

	if (documentId === null) {
		throw redirect(302, '/list');
	}

	const response = await fetch(`${PUBLIC_BACKEND_URL}/docs/${documentId}`);

	if (!response.ok) {
		throw error(response.status, await response.text());
	}

	const document = (await response.json()) as ScannedDocument;

	return document;
}
