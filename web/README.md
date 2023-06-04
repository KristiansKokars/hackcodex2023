# Svelte frontend

## Setup PNPM

If `pnpm` is not setup, do so by follow this link: [https://pnpm.io/installation](https://pnpm.io/installation)

## Add a .env file

Follow the `.env.example` file and create a `.env` file with all the environment variables.

## Developing

On first install and on adding dependencies, make sure to run:

```bash
pnpm i
```

```bash
pnpm run dev

# or start the server and open the app in a new browser tab
pnpm run dev -- --open
```

## Building

To create a production version:

```bash
pnpm run build
```

You can preview the production build with `pnpm run preview`.
