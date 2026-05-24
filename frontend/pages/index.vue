<template>
  <div>
    <header class="topbar">
      <div>
        <p class="eyebrow">Dashboard</p>
        <h1 class="section-title">Hoş geldin{{ profile?.username ? `, ${profile.username}` : '' }}.</h1>
        <p class="subtitle">Geçmiş tatillerin ve planladığın rotalar tek haritada.</p>
      </div>
      <NuxtLink class="btn btn-accent" to="/trips/new">Yeni tatil planla</NuxtLink>
    </header>

    <section class="metrics">
      <div class="metric"><span>Toplam tatil</span><strong>{{ summary?.totalTrips ?? 0 }}</strong></div>
      <div class="metric"><span>Planlanan</span><strong>{{ summary?.plannedTrips ?? 0 }}</strong></div>
      <div class="metric"><span>Geçmiş</span><strong>{{ summary?.completedTrips ?? 0 }}</strong></div>
      <div class="metric"><span>Seyahat günü</span><strong>{{ summary?.totalTravelDays ?? 0 }}</strong></div>
    </section>

    <section class="dashboard-grid">
      <div class="panel">
        <div class="actions" style="justify-content: space-between; margin-bottom: 14px;">
          <h2 style="margin:0;">Dünya haritası</h2>
          <div class="actions">
            <span class="badge badge-planned">Planlanan</span>
            <span class="badge badge-completed">Geçmiş</span>
            <span class="badge badge-ongoing">Devam eden</span>
          </div>
        </div>
        <TripWorldMap :markers="markers" />
      </div>

      <aside class="panel">
        <div class="actions" style="justify-content: space-between; margin-bottom: 14px;">
          <h2 style="margin:0;">Tatil kayıtları</h2>
          <span style="color: var(--muted); font-weight: 700;">{{ trips.length }} kayıt</span>
        </div>
        <div v-if="loading" class="subtitle">Yükleniyor...</div>
        <div v-else-if="!trips.length" class="premium-note">Henüz tatil kaydın yok. İlk rotanı planlayarak başlayabilirsin.</div>
        <div v-else class="trip-list">
          <TripCard v-for="trip in trips" :key="trip.tripID" :trip="trip" />
        </div>
      </aside>
    </section>
  </div>
</template>

<script setup lang="ts">
import type { DashboardSummary, Trip, TripMarker, UserProfile } from '~/types'

const api = useApi()
const profile = useState<UserProfile | null>('profile')
const loading = ref(true)
const summary = ref<DashboardSummary | null>(null)
const trips = ref<Trip[]>([])
const markers = ref<TripMarker[]>([])

onMounted(async () => {
  try {
    const [profileData, summaryData, tripData, markerData] = await Promise.all([
      api.getMe(),
      api.getSummary(),
      api.getTrips(),
      api.getMarkers()
    ])
    profile.value = profileData
    summary.value = summaryData
    trips.value = tripData
    markers.value = markerData
  } finally {
    loading.value = false
  }
})
</script>
