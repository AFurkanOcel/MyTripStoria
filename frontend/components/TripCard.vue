<template>
  <NuxtLink class="trip-card trip-card-link" :to="`/trips/${trip.tripID}`">
    <img v-if="coverPhoto" class="trip-card-photo" :src="coverPhoto" :alt="trip.title" />
    <div class="actions" style="justify-content: space-between; margin-bottom: 10px;">
      <StatusBadge :status="trip.status" />
      <span style="color: var(--muted); font-size: 13px;">{{ dateRange }}</span>
    </div>
    <h3>{{ trip.title }}</h3>
    <p>{{ trip.placeName || trip.cityName || 'No location selected' }} - {{ trip.countryName || 'No country selected' }}</p>
  </NuxtLink>
</template>

<script setup lang="ts">
import type { Trip } from '~/types'

const props = defineProps<{ trip: Trip }>()
const api = useApi()

const coverPhoto = computed(() => {
  const firstPhoto = props.trip.photos?.[0]
  return firstPhoto?.url ? `${api.apiBase}${firstPhoto.url}` : ''
})

const dateRange = computed(() => {
  const start = new Date(props.trip.startDate).toLocaleDateString('en-US')
  const end = new Date(props.trip.endDate).toLocaleDateString('en-US')
  return `${start} - ${end}`
})
</script>
