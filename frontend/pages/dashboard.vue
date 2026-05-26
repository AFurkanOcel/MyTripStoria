<template>
  <div>
    <header class="topbar">
      <div>
        <p class="eyebrow">Dashboard</p>
        <h1 class="section-title">Welcome{{ profile?.username ? ` ${profile.username}` : '' }}</h1>
        <p class="subtitle">Your completed trips and planned routes, gathered on one world map.</p>
      </div>
      <NuxtLink class="btn btn-accent" to="/trips/new">Plan a new trip</NuxtLink>
    </header>

    <section class="metrics">
      <div class="metric"><span>Total trips</span><strong>{{ summary?.totalTrips ?? 0 }}</strong></div>
      <div class="metric"><span>Planned</span><strong>{{ summary?.plannedTrips ?? 0 }}</strong></div>
      <div class="metric"><span>Completed</span><strong>{{ summary?.completedTrips ?? 0 }}</strong></div>
      <div class="metric"><span>Travel days</span><strong>{{ summary?.totalTravelDays ?? 0 }}</strong></div>
    </section>

    <section class="dashboard-grid">
      <div class="panel">
        <div class="actions" style="justify-content: space-between; margin-bottom: 14px;">
          <h2 style="margin:0;">World map</h2>
          <div class="actions">
            <span class="badge badge-planned">Planned</span>
            <span class="badge badge-completed">Completed</span>
            <span class="badge badge-ongoing">Ongoing</span>
            <span class="badge badge-home">Home</span>
          </div>
        </div>
        <TripWorldMap :markers="mapMarkers" />
      </div>

      <aside class="panel">
        <div class="actions" style="justify-content: space-between; margin-bottom: 14px;">
          <h2 style="margin:0;">Trip records</h2>
          <NuxtLink class="text-link" to="/trips">{{ trips.length }} records</NuxtLink>
        </div>
        <div v-if="loading" class="subtitle">Loading...</div>
        <div v-else-if="!trips.length" class="premium-note">You do not have any trips yet. Start by planning your first route.</div>
        <div v-else class="trip-list">
          <article v-for="trip in trips" :key="trip.tripID" class="trip-record">
            <TripCard :trip="trip" />
            <button class="trip-remove-button" type="button" @click="tripToRemove = trip">Remove</button>
          </article>
        </div>
      </aside>
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
import type { City, DashboardSummary, Trip, TripMarker, UserProfile } from '~/types'

const api = useApi()
const profile = useState<UserProfile | null>('profile')
const loading = ref(true)
const summary = ref<DashboardSummary | null>(null)
const trips = ref<Trip[]>([])
const markers = ref<TripMarker[]>([])
const cities = ref<City[]>([])
const tripToRemove = ref<Trip | null>(null)

const homeMarker = computed<TripMarker | null>(() => {
  if (!profile.value) return null
  const homeCity = cities.value.find((city) => city.id === profile.value?.cityId)
  if (!homeCity?.latitude || !homeCity.longitude) return null

  return {
    title: 'Home',
    status: 'Home',
    markerType: 'Home',
    markerColor: '#f97316',
    cityName: profile.value.cityName || homeCity.name,
    countryName: profile.value.countryName || homeCity.countryName,
    latitude: homeCity.latitude,
    longitude: homeCity.longitude,
    startDate: '',
    endDate: ''
  }
})

const mapMarkers = computed(() => {
  return homeMarker.value ? [homeMarker.value, ...markers.value] : markers.value
})

const loadDashboard = async () => {
  try {
    loading.value = true
    const [profileData, summaryData, tripData, markerData, cityData] = await Promise.all([
      api.getMe(),
      api.getSummary(),
      api.getTrips(),
      api.getMarkers(),
      api.getCities()
    ])
    profile.value = profileData
    summary.value = summaryData
    trips.value = tripData
    markers.value = markerData
    cities.value = cityData
  } finally {
    loading.value = false
  }
}

onMounted(loadDashboard)

const removeTrip = async () => {
  if (!tripToRemove.value) return

  await api.deleteTrip(tripToRemove.value.tripID)
  tripToRemove.value = null
  await loadDashboard()
}
</script>
