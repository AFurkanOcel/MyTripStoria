<template>
  <section class="panel">
    <p class="subtitle">Opening trip editor...</p>
  </section>
</template>

<script setup lang="ts">
const route = useRoute()

onMounted(async () => {
  await navigateTo(`/trips/edit/${route.params.id}`)
})
</script>
