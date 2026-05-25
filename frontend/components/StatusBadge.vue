<template>
  <span :class="['badge', className]">{{ label }}</span>
</template>

<script setup lang="ts">
const props = defineProps<{ status?: string }>()

const label = computed(() => {
  if (props.status === 'Completed') return 'Completed'
  if (props.status === 'Ongoing') return 'Ongoing'
  if (props.status === 'Cancelled') return 'Cancelled'
  return 'Planned'
})

const className = computed(() => {
  if (props.status === 'Completed') return 'badge-completed'
  if (props.status === 'Ongoing') return 'badge-ongoing'
  if (props.status === 'Cancelled') return 'badge-muted'
  return 'badge-planned'
})
</script>
