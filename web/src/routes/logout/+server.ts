export async function POST({ cookies, locals }) {
	cookies.delete('sessionUser');
	locals.user = undefined;
}
