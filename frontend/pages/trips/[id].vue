<template>
  <div>
    <header class="topbar">
      <div>
        <p class="eyebrow">Trip details</p>
        <h1 class="section-title">{{ trip?.title || 'Loading' }}</h1>
        <p class="subtitle">{{ trip?.placeName || trip?.cityName || 'Location details' }} - {{ trip?.countryName || '' }}</p>
      </div>
      <div class="actions">
        <NuxtLink v-if="trip" class="btn btn-primary" :to="`/trips/${trip.tripID}/edit`">Edit trip</NuxtLink>
        <NuxtLink class="btn btn-ghost" to="/">Dashboard</NuxtLink>
      </div>
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
              <button class="icon-danger" type="button" title="Remove photo" @click.prevent="photoToRemove = photo.id">
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
            <label class="file-picker">
              <input type="file" accept="image/png,image/jpeg,image/webp" @change="pickFile" />
              <span>Choose photo</span>
              <small>{{ file?.name || 'No file selected' }}</small>
            </label>
          </label>
          <label class="field">
            <span>Caption</span>
            <input v-model="caption" class="input" />
          </label>
          <p v-if="error" class="error">{{ error }}</p>
          <button class="btn btn-primary" type="submit" :disabled="uploading">
            {{ uploading ? 'Uploading photo' : 'Upload photo' }}
          </button>
        </form>
      </aside>
    </section>

    <div v-if="photoToRemove" class="modal-backdrop" role="dialog" aria-modal="true">
      <div class="modal">
        <h2>Remove this photo?</h2>
        <p>This action cannot be undone.</p>
        <div class="actions" style="justify-content: flex-end;">
          <button class="btn btn-ghost" type="button" @click="photoToRemove = null">Cancel</button>
          <button class="btn btn-primary" type="button" @click="removePhoto">Remove</button>
        </div>
      </div>
    </div>
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
const uploading = ref(false)
const photoToRemove = ref<number | null>(null)
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
    await api.uploadPhoto(trip.value.tripID, file.value, caption.value)
    await load()
    file.value = null
    caption.value = ''
  } catch (uploadError: any) {
    error.value = getErrorMessage(uploadError, 'The photo could not be uploaded. Please check your premium status and file size.')
  } finally {
    uploading.value = false
  }
}

const removePhoto = async () => {
  if (!trip.value || !photoToRemove.value) return

  error.value = ''
  try {
    await api.deletePhoto(trip.value.tripID, photoToRemove.value)
    photoToRemove.value = null
    await load()
  } catch (removeError: any) {
    error.value = getErrorMessage(removeError, 'The photo could not be removed.')
  }
}

const getErrorMessage = (apiError: any, fallback: string) => {
  const data = apiError?.data
  if (typeof data === 'string') return data
  if (data?.title) return data.title
  if (data?.errors) return Object.values(data.errors).flat().join(' ')
  return fallback
}
</script>
