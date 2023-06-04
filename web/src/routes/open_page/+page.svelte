<script>
    import { Label, Input, Button } from 'flowbite-svelte'
</script>

<nav class="bg-white shadow mb-16">
  <div class="mx-auto max-w-7xl px-2 sm:px-6 lg:px-8">
    <div class="relative flex h-16 justify-between">
      <div class="absolute inset-y-0 left-0 flex items-center sm:hidden">
        <!-- Mobile menu button -->
        <button type="button" class="inline-flex items-center justify-center rounded-md p-2 text-gray-400 hover:bg-gray-100 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-inset focus:ring-indigo-500" aria-controls="mobile-menu" aria-expanded="false">
          <span class="sr-only">Open main menu</span>
          <!--
            Icon when menu is closed.

            Menu open: "hidden", Menu closed: "block"
          -->
		  <a href="/maksims" class="inline-flex items-center border-b-2 border-transparent px-1 pt-1 text-sm font-medium text-gray-900">Log Out</a>
          <!--
            Icon when menu is open.

            Menu open: "block", Menu closed: "hidden"
          -->
		  <a href="/maksims" class="inline-flex items-center border-b-2 border-transparent px-1 pt-1 text-sm font-medium text-gray-900">Log Out</a>
        </button>
      </div>
      <div class="flex flex-1 items-center justify-center sm:items-stretch sm:justify-start">
        <div class="flex flex-shrink-0 items-center">
          <img class="block h-8 w-auto lg:hidden" src="https://tailwindui.com/img/logos/mark.svg?color=indigo&shade=600" alt="Your Company">
          <img class="hidden h-8 w-auto lg:block" src="https://tailwindui.com/img/logos/mark.svg?color=indigo&shade=600" alt="Your Company">
        </div>
        <div class="hidden sm:ml-6 sm:flex sm:space-x-8">
          <!-- Current: "border-indigo-500 text-gray-900", Default: "border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700" -->
          <a href="/list" class="inline-flex items-center border-b-2 border-transparent px-1 pt-1 text-sm font-medium text-gray-500 hover:border-gray-300 hover:text-gray-700">List</a>
        </div>
      </div>
      <div class="absolute inset-y-0 right-0 flex items-center pr-2 sm:static sm:inset-auto sm:ml-6 sm:pr-0">
		<a class="mr-4" href="#"><button type="button" class="relative inline-flex items-center gap-x-1.5 rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600">
            <svg class="-ml-0.5 h-5 w-5" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
              <path d="M10.75 4.75a.75.75 0 00-1.5 0v4.5h-4.5a.75.75 0 000 1.5h4.5v4.5a.75.75 0 001.5 0v-4.5h4.5a.75.75 0 000-1.5h-4.5v-4.5z" />
            </svg>
            Upload File
          </button></a>
        <button type="button" class="rounded-full bg-white p-1 text-gray-400 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2">
          <span class="sr-only">View notifications</span>
		  <a href="/register" class="inline-flex items-center border-b-2 border-transparent px-1 pt-1 text-sm font-medium text-gray-900">Log Out</a>
        </button>

        <!-- Profile dropdown -->
        <div class="relative ml-3">
        </div>
      </div>
    </div>
  </div>

  <!-- Mobile menu, show/hide based on menu state. -->
  <div class="sm:hidden" id="mobile-menu">
    <div class="space-y-1 pb-4 pt-2">
      <!-- Current: "bg-indigo-50 border-indigo-500 text-indigo-700", Default: "border-transparent text-gray-500 hover:bg-gray-50 hover:border-gray-300 hover:text-gray-700" -->
      <a href="/list" class="block border-l-4 border-transparent py-2 pl-3 pr-4 text-base font-medium text-gray-500 hover:border-gray-300 hover:bg-gray-50 hover:text-gray-700">List</a>
	  <a href="#" class="block border-l-4 border-indigo-500 bg-indigo-50 py-2 pl-3 pr-4 text-base font-medium text-indigo-700">Upload File</a>
    </div>
  </div>
</nav>

<!-- https://www.tutorialspoint.com/assets/questions/media/426142-1668760872.png-->

<div class="grid grid-cols-2 divide-x">
    <div class="flex items-center justify-center">
      <img src="your-image-url.jpg" alt="Document" class="w-2/3 border rounded-lg p-4">
    </div>
    <div class="flex flex-col items-center justify-center">     
        <div class='mb-6 w-96'>
        <Label for='large-input' class='block mb-2'>Large input</Label>
        <Input class="h-8 rounded-none" id="large-input" size="lg" placeholder="Large input" />
        </div>
        <div class="flex flex-col items-center justify-center">     
            <div class='mb-6 w-96'>
            <Label for='large-input' class='block mb-2'>Large input</Label>
            <Input class="h-8 rounded-none" id="large-input" size="lg" placeholder="Large input" />
            </div>
        </div>
        <div class="flex flex-col items-center justify-center">     
            <div class='mb-6 w-96'>
            <Label for='large-input' class='block mb-2'>Large input</Label>
            <Input class="h-8 rounded-none" id="large-input" size="lg" placeholder="Large input" />
            </div>
        </div>
        <div class="flex flex-col items-center justify-center">     
            <div class='mb-6 w-96'>
            <Label for='large-input' class='block mb-2'>Large input</Label>
            <Input class="h-8 rounded-none" id="large-input" size="lg" placeholder="Large input" />
            </div>
        </div>
        <div class="flex flex-col items-center justify-center">     
            <div class='mb-6 w-96'>
            <Label for='large-input' class='block mb-2'>Large input</Label>
            <Input class="h-8 rounded-none" id="large-input" size="lg" placeholder="Large input" />
            </div>
        </div>
        <Button class="w-56" color="green">Update</Button>
    </div>
  </div>
  