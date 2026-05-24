<template>
  <div>
    <header class="topbar">
      <div>
        <p class="eyebrow">Tatil detayı</p>
        <h1 class="section-title">{{ trip?.title || 'Yükleniyor' }}</h1>
        <p class="subtitle">{{ trip?.placeName || trip?.cityName || 'Konum bilgisi' }} · {{ trip?.countryName || '' }}</p>
      </div>
      <NuxtLink class="btn btn-ghost" to="/">Dashboard</NuxtLink>
    </header>

    <section v-if="trip" class="dashboard-grid">
      <div class="panel">
        <div class="actions" style="justify-content: space-between; margin-bottom: 14px;">
          <StatusBadge :status="trip.status" />
          <span style="color: var(--muted); font-weight: 700;">{{ dateRange }}</span>
        </div>
        <p style="line-height: 1.7;">{{ trip.description }}</p>
        <div class="metrics" style="grid-template-columns: repeat(3, minmax(0, 1fr)); margin-top: 18px;">
          <div class="metric"><span>Planlanan bütçe</span><strong>{{ money(trip.plannedBudget) }}</strong></div>
          <div class="metric"><span>Gerçek maliyet</span><strong>{{ money(trip.actualCost) }}</strong></div>
          <div class="metric"><span>Fotoğraf</span><strong>{{ trip.photos?.length || 0 }}</strong></div>
        </div>
        <div v-if="trip.photos?.length" class="trip-list">
          <article v-for="photo in trip.photos" :key="photo.id" class="trip-card">
            <img :src="absoluteUrl(photo.url)" :alt="photo.caption || trip.title" style="width:100%; max-height:320px; object-fit:cover; border-radius:8px; margin-bottom:10px;" />
            <h3>{{ photo.caption || photo.originalFileName }}</h3>
          </article>
        </div>
      </div>

      <aside class="panel">
        <h2 style="margin-top:0;">Premium fotoğraflar</h2>
        <p v-if="!profile?.isPremium" class="premium-note">Fotoğraf yükleme premium plana özeldir.</p>
        <form v-else class="form" @submit.prevent="upload">
          <label class="field">
            <span>Fotoğraf</span>
            <input class="input" type="file" accept="image/png,image/jpeg,image/webp" @change="pickFile" />
          </label>
          <label class="field">
            <span>Açıklama</span>
            <input v-model="caption" class="input" />
          </label>
          <label class="field" style="display:flex; grid-template-columns: auto 1fr; align-items:center; gap:10px;">
            <input v-model="isCover" type="checkbox" />
            <span>Kapak fotoğrafı yap</span>
          </label>
          <p v-if="error" class="error">{{ error }}</p>
          <button class="btn btn-primary" type="submit" :disabled="uploading">
            {{ uploading ? 'Yükleniyor' : 'Fotoğraf yükle' }}
          </button>
        </form>
      </aside>
    </section>
  </div>
</template>

<script setup lang="ts">
import type { Trip, UserProfile } from '~/types'

const route = useRoute()
const api = useApi()
const profile = useState<UserProfile | null>('profile')
const trip = ref<Trip | null>(null)
const file = ref<File | null>(null)
const caption = ref('')
const isCover = ref(false)
const uploading = ref(false)
const error = ref('')

const load = async () => {
  const [profileData, tripData] = await Promise.all([
    profile.value ? Promise.resolve(profile.value) : api.getMe(),
    api.getTrip(route.params.id as string)
  ])
  profile.value = profileData
  trip.value = tripData
}

onMounted(load)

const dateRange = computed(() => {
  if (!trip.value) return ''
  return `${new Date(trip.value.startDate).toLocaleDateString('tr-TR')} - ${new Date(trip.value.endDate).toLocaleDateString('tr-TR')}`
})

const money = (value?: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(value || 0)
const absoluteUrl = (url: string) => `${api.apiBase}${url}`
const pickFile = (event: Event) => {
  file.value = (event.target as HTMLInputElement).files?.[0] || null
}

const upload = async () => {
  if (!trip.value || !file.value) {
    error.value = 'Bir fotoğraf seçmelisin.'
    return
  }

  uploading.value = true
  error.value = ''
  try {
    await api.uploadPhoto(trip.value.tripID, file.value, caption.value, isCover.value)
    await load()
    file.value = null
    caption.value = ''
    isCover.value = false
  } catch {
    error.value = 'Fotoğraf yüklenemedi. Premium durumunu ve dosya boyutunu kontrol et.'
  } finally {
    uploading.value = false
  }
}
</script>
