<template>
  <div>
    <header class="topbar">
      <div>
        <p class="eyebrow">Trip records</p>
        <h1 class="section-title">Manage every trip in one place</h1>
        <p class="subtitle">Search, filter, edit, or remove your planned routes and completed memories.</p>
      </div>
      <NuxtLink class="btn btn-accent" to="/trips/new">New trip</NuxtLink>
    </header>

    <section class="panel records-toolbar">
      <label class="field">
        <span>Search</span>
        <input v-model.trim="search" class="input" placeholder="Search by title, place, city, or country" />
      </label>
      <label class="field">
        <span>Status</span>
        <select v-model="statusFilter" class="select">
          <option value="All">All</option>
          <option value="Planned">Planned</option>
          <option value="Ongoing">Ongoing</option>
          <option value="Completed">Completed</option>
          <option value="Cancelled">Cancelled</option>
        </select>
      </label>
    </section>

    <section class="panel">
      <div class="actions" style="justify-content: space-between; margin-bottom: 14px;">
        <h2 style="margin:0;">Records</h2>
        <span style="color: var(--muted); font-weight: 700;">{{ filteredTrips.length }} records</span>
      </div>

      <div v-if="loading" class="subtitle">Loading records...</div>
      <div v-else-if="!filteredTrips.length" class="premium-note">No trips match your current filters.</div>
      <div v-else class="records-grid">
        <article v-for="trip in filteredTrips" :key="trip.tripID" class="record-card">
          <img v-if="firstPhoto(trip)" class="record-card-photo" :src="firstPhoto(trip)" :alt="trip.title" />
          <div class="record-card-body">
            <div class="actions" style="justify-content: space-between;">
              <StatusBadge :status="trip.status" />
              <span style="color: var(--muted); font-size: 13px; font-weight: 700;">{{ dateRange(trip) }}</span>
            </div>
            <h3>{{ trip.title }}</h3>
            <p>{{ trip.placeName || trip.cityName || 'No location selected' }} - {{ trip.countryName || 'No country selected' }}</p>
            <div class="record-meta">
              <span>Budget</span>
              <strong>{{ money(trip.plannedBudget) }}</strong>
            </div>
            <div class="actions">
              <NuxtLink class="btn btn-ghost" :to="`/trips/${trip.tripID}`">View</NuxtLink>
              <NuxtLink class="btn btn-primary" :to="`/trips/edit/${trip.tripID}`">Edit</NuxtLink>
              <button class="icon-danger" type="button" @click="tripToRemove = trip">Remove</button>
            </div>
          </div>
        </article>
      </div>
    </section>

    <div v-if="tripToRemove" class="modal-backdrop" role="dialog" aria-modal="true">
      <div class="modal">
        <h2>Remove this trip?</h2>
        <p>This will permanently delete the trip and its photos.</p>
        <div class="actions" style="justify-content: flex-end;">
          <button class="btn btn-ghost" type="button" @click="tripToRemove = null">Cancel</button>
          <button class="btn btn-primary" type="button" @click="removeTrip">Remove</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Trip } from '~/types'

const api = useApi()
const loading = ref(true)
const trips = ref<Trip[]>([])
const search = ref('')
const statusFilter = ref('All')
const tripToRemove = ref<Trip | null>(null)

const filteredTrips = computed(() => {
  const term = search.value.toLowerCase()
  return trips.value.filter((trip) => {
    const matchesStatus = statusFilter.value === 'All' || trip.status === statusFilter.value
    const haystack = [
      trip.title,
      trip.placeName,
      trip.cityName,
      trip.countryName,
      trip.tripType
    ].filter(Boolean).join(' ').toLowerCase()
    return matchesStatus && (!term || haystack.includes(term))
  })
})

const loadTrips = async () => {
  loading.value = true
  try {
    trips.value = await api.getTrips()
  } finally {
    loading.value = false
  }
}

onMounted(loadTrips)

const firstPhoto = (trip: Trip) => {
  const photo = trip.photos?.[0]
  return photo?.url ? `${api.apiBase}${photo.url}` : ''
}

const dateRange = (trip: Trip) => {
  const start = new Date(trip.startDate).toLocaleDateString('en-US')
  const end = new Date(trip.endDate).toLocaleDateString('en-US')
  return `${start} - ${end}`
}

const money = (value?: number) => new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value || 0)

const removeTrip = async () => {
  if (!tripToRemove.value) return
  await api.deleteTrip(tripToRemove.value.tripID)
  tripToRemove.value = null
  await loadTrips()
}
</script>
