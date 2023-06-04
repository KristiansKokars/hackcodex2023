import type { User } from "$lib/features/auth/User";

declare global {
	namespace App {
		// interface Error {}
		interface Locals {
			user: User | undefined
		}
		// interface PageData {}
		// interface Platform {}
	}
}

export {};
