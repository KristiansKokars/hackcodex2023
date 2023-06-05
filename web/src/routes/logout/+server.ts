export async function POST({ cookies, locals }) {
	cookies.delete('sessionUser');
	console.log('Deleted session user token');
	locals.user = undefined;

	return new Response();
}
