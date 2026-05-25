<template>
  <div>
    <header class="topbar">
      <div>
        <p class="eyebrow">Trip details</p>
        <h1 class="section-title">{{ trip?.title || 'Loading' }}</h1>
        <p class="subtitle">{{ trip?.placeName || trip?.cityName || 'Location details' }} · {{ trip?.countryName || '' }}</p>
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
          <div class="metric"><span>Budget</span><strong>{{ money(trip.plannedBudget) }}</strong></div>
          <div class="metric"><span>Photos</span><strong>{{ trip.photos?.length || 0 }}</strong></div>
          <div class="metric"><span>Visibility</span><strong>{{ trip.visibility }}</strong></div>
        </div>

        <div v-if="trip.photos?.length" class="trip-list">
          <article v-for="photo in trip.photos" :key="photo.id" class="trip-card photo-card">
            <img :src="absoluteUrl(photo.url)" :alt="photo.caption || trip.title" />
            <div class="actions" style="justify-content: space-between;">
              <h3>{{ photo.caption || photo.originalFileName }}</h3>
              <button class="icon-danger" type="button" title="Remove photo" @click.prevent="removePhoto(photo.id)">
                Remove
              </button>
            </div>
          </article>
        </div>
      </div>

      <aside class="panel">
        <h2 style="margin-top:0;">Premium photos</h2>
        <p v-if="!profile?.isPremium" class="premium-note">Photo uploads are available on the premium plan.</p>
        <form v-else class="form" @submit.prevent="upload">
          <label class="field">
            <span>Photo</span>
            <input class="input" type="file" accept="image/png,image/jpeg,image/webp" @change="pickFile" />
          </label>
          <label class="field">
            <span>Caption</span>
            <input v-model="caption" class="input" />
          </label>
          <label class="check-field">
            <input v-model="isCover" type="checkbox" />
            <span>Use as cover photo</span>
          </label>
          <p v-if="error" class="error">{{ error }}</p>
          <button class="btn btn-primary" type="submit" :disabled="uploading">
            {{ uploading ? 'Uploading photo' : 'Upload photo' }}
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
  return `${new Date(trip.value.startDate).toLocaleDateString('en-US')} - ${new Date(trip.value.endDate).toLocaleDateString('en-US')}`
})

const money = (value?: number) => new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value || 0)
const absoluteUrl = (url: string) => `${api.apiBase}${url}`
const pickFile = (event: Event) => {
  file.value = (event.target as HTMLInputElement).files?.[0] || null
}

const upload = async () => {
  if (!trip.value || !file.value) {
    error.value = 'Choose a photo first.'
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
    error.value = 'The photo could not be uploaded. Please check your premium status and file size.'
  } finally {
    uploading.value = false
  }
}

const removePhoto = async (photoId: number) => {
  if (!trip.value) return

  error.value = ''
  try {
    await api.deletePhoto(trip.value.tripID, photoId)
    await load()
  } catch {
    error.value = 'The photo could not be removed.'
  }
}
</script>
