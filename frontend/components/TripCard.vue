<template>
  <NuxtLink class="trip-card" :to="`/trips/${trip.tripID}`">
    <div class="actions" style="justify-content: space-between; margin-bottom: 10px;">
      <StatusBadge :status="trip.status" />
      <span style="color: var(--muted); font-size: 13px;">{{ dateRange }}</span>
    </div>
    <h3>{{ trip.title }}</h3>
    <p>{{ trip.placeName || trip.cityName || 'Konum seçilmedi' }} · {{ trip.countryName || 'Ülke seçilmedi' }}</p>
  </NuxtLink>
</template>

<script setup lang="ts">
import type { Trip } from '~/types'

const props = defineProps<{ trip: Trip }>()
const dateRange = computed(() => {
  const start = new Date(props.trip.startDate).toLocaleDateString('tr-TR')
  const end = new Date(props.trip.endDate).toLocaleDateString('tr-TR')
  return `${start} - ${end}`
})
</script>
