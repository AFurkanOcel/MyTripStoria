<template>
  <div>
    <header class="topbar">
      <div>
        <p class="eyebrow">Profile</p>
        <h1 class="section-title">Your travel identity</h1>
        <p class="subtitle">Manage the information shown in your MyTripStoria workspace.</p>
      </div>
    </header>

    <section class="profile-grid">
      <aside class="panel profile-card">
        <img class="profile-photo-large" :src="profilePhoto" :alt="form.displayName || form.username" />
        <strong>{{ form.displayName || form.username || 'Traveler' }}</strong>
        <span>{{ form.isPremium ? 'Premium plan' : 'Standard plan' }}</span>

        <label class="field">
          <span>Profile photo</span>
          <label class="file-picker">
            <input type="file" accept="image/png,image/jpeg,image/webp" @change="pickPhoto" />
            <span>Choose photo</span>
            <small>{{ photoFile?.name || 'No file selected' }}</small>
          </label>
        </label>
        <button class="btn btn-ghost" type="button" :disabled="photoUploading || !photoFile" @click="uploadPhoto">
          {{ photoUploading ? 'Uploading photo' : 'Upload photo' }}
        </button>
      </aside>

      <section class="panel">
        <form class="form" @submit.prevent="save">
          <div class="grid-2">
            <label class="field">
              <span>Display name</span>
              <input v-model.trim="form.displayName" class="input" />
            </label>

            <label class="field">
              <span>Username</span>
              <input v-model.trim="form.username" class="input" required />
            </label>
          </div>

          <div class="grid-2">
            <label class="field">
              <span>Email</span>
              <input v-model.trim="form.email" class="input" type="email" required />
            </label>

            <label class="field">
              <span>Phone number</span>
              <input v-model.trim="form.phoneNumber" class="input" required />
            </label>
          </div>

          <label class="field">
            <span>About</span>
            <textarea v-model.trim="form.bio" class="textarea" placeholder="A short note about your travel style." />
          </label>

          <p v-if="message" class="success">{{ message }}</p>
          <p v-if="error" class="error">{{ error }}</p>

          <button class="btn btn-primary" type="submit" :disabled="saving">
            {{ saving ? 'Saving profile' : 'Save profile' }}
          </button>
        </form>
      </section>
    </section>
  </div>
</template>

<script setup lang="ts">
import type { UserProfile } from '~/types'

const api = useApi()
const profile = useState<UserProfile | null>('profile')
const saving = ref(false)
const photoUploading = ref(false)
const photoFile = ref<File | null>(null)
const message = ref('')
const error = ref('')

const form = reactive<UserProfile>({
  id: 0,
  username: '',
  displayName: '',
  email: '',
  phoneNumber: '',
  profilePhotoUrl: '',
  bio: '',
  age: 18,
  countryId: 1,
  cityId: 1,
  address: '',
  budget: 0,
  isPremium: false
})

const profilePhoto = computed(() => form.profilePhotoUrl ? `${api.apiBase}${form.profilePhotoUrl}` : '/icon.png')

const hydrate = (data: UserProfile) => {
  Object.assign(form, data)
}

onMounted(async () => {
  const data = await api.getMe()
  profile.value = data
  hydrate(data)
})

const pickPhoto = (event: Event) => {
  photoFile.value = (event.target as HTMLInputElement).files?.[0] || null
}

const uploadPhoto = async () => {
  if (!photoFile.value) return

  photoUploading.value = true
  error.value = ''
  message.value = ''
  try {
    const updated = await api.uploadProfilePhoto(photoFile.value)
    profile.value = updated
    hydrate(updated)
    photoFile.value = null
    message.value = 'Profile photo updated.'
  } catch (uploadError: any) {
    error.value = getErrorMessage(uploadError, 'Profile photo could not be uploaded.')
  } finally {
    photoUploading.value = false
  }
}

const save = async () => {
  saving.value = true
  error.value = ''
  message.value = ''
  try {
    const updated = await api.updateProfile(form)
    profile.value = updated
    hydrate(updated)
    message.value = 'Profile updated.'
  } catch (saveError: any) {
    error.value = getErrorMessage(saveError, 'Profile could not be updated.')
  } finally {
    saving.value = false
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
