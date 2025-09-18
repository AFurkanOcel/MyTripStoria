<script setup>
import { ref, onMounted } from 'vue'

const data = ref(null)

async function getData() {
  try {
    const res = await fetch('https://localhost:7160/api/Ping')
    data.value = await res.json()
  } catch (err) {
    data.value = { error: 'API connection failed' }
    console.error(err)
  }
}

onMounted(() => {
  getData()
})
</script>

<template>
  <div class="flex items-center justify-center min-h-screen bg-gray-200">
    <div class="bg-white p-8 rounded-2xl shadow-xl w-full max-w-md text-center">
      <h1 class="text-2xl font-bold mb-4">Welcome!</h1>
      <pre class="text-left bg-gray-100 p-4 rounded-lg overflow-x-auto">
        {{ data }}
      </pre>
    </div>
  </div>
</template>
