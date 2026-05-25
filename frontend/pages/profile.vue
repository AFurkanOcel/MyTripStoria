<template>
  <div>
    <header class="topbar">
      <div>
        <p class="eyebrow">Profile</p>
        <h1 class="section-title">Your travel identity</h1>
        <p class="subtitle">Review and update the information connected to your MyTripStoria account.</p>
      </div>
      <button class="btn btn-primary" type="button" @click="startEditing">
        {{ editing ? 'Editing profile' : 'Edit profile' }}
      </button>
    </header>

    <section v-if="loading" class="panel">
      <p class="subtitle">Loading profile...</p>
    </section>

    <section v-else-if="form.id" class="profile-grid">
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

      <section v-if="!editing" class="panel profile-details">
        <div>
          <span>Username</span>
          <strong>{{ form.username }}</strong>
        </div>
        <div>
          <span>Email</span>
          <strong>{{ form.email }}</strong>
        </div>
        <div>
          <span>Phone number</span>
          <strong>{{ form.phoneNumber }}</strong>
        </div>
        <div>
          <span>Location</span>
          <strong>{{ form.cityName || 'No city selected' }} - {{ form.countryName || 'No country selected' }}</strong>
        </div>
        <div class="profile-detail-wide">
          <span>About</span>
          <p>{{ form.bio || 'No profile note yet.' }}</p>
        </div>
      </section>

      <section v-else class="panel">
        <form class="form" @submit.prevent="save">
          <div class="grid-2">
            <label class="field">
              <span>Display name</span>
              <input v-model.trim="editForm.displayName" class="input" />
            </label>

            <label class="field">
              <span>Username</span>
              <input v-model.trim="editForm.username" class="input" required />
            </label>
          </div>

          <div class="grid-2">
            <label class="field">
              <span>Email</span>
              <input v-model="editForm.email" class="input" type="email" disabled />
            </label>

            <label class="field">
              <span>Phone number</span>
              <input v-model.trim="editForm.phoneNumber" class="input" required />
            </label>
          </div>

          <div class="grid-2">
            <label class="field">
              <span>Country</span>
              <select v-model.number="editForm.countryId" class="select" required>
                <option v-for="country in countries" :key="country.id" :value="country.id">{{ country.name }}</option>
              </select>
            </label>

            <label class="field">
              <span>City</span>
              <select v-model.number="editForm.cityId" class="select" required>
                <option v-for="city in filteredCities" :key="city.id" :value="city.id">{{ city.name }}</option>
              </select>
            </label>
          </div>

          <label class="field">
            <span>About</span>
            <textarea v-model.trim="editForm.bio" class="textarea" placeholder="A short note about your travel style." />
          </label>

          <p v-if="message" class="success">{{ message }}</p>
          <p v-if="error" class="error">{{ error }}</p>

          <div class="actions">
            <button class="btn btn-primary" type="submit" :disabled="saving">
              {{ saving ? 'Updating profile' : 'Update profile' }}
            </button>
            <button class="btn btn-ghost" type="button" @click="cancelEditing">Cancel</button>
          </div>
        </form>
      </section>
    </section>
  </div>
</template>

<script setup lang="ts">
import type { City, Country, UserProfile } from '~/types'

const api = useApi()
const profile = useState<UserProfile | null>('profile')
const loading = ref(true)
const editing = ref(false)
const saving = ref(false)
const photoUploading = ref(false)
const photoFile = ref<File | null>(null)
const message = ref('')
const error = ref('')
const countries = ref<Country[]>([])
const cities = ref<City[]>([])

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

const editForm = reactive<UserProfile>({ ...form })

const profilePhoto = computed(() => form.profilePhotoUrl ? `${api.apiBase}${form.profilePhotoUrl}` : '/icon.png')
const filteredCities = computed(() => cities.value.filter((city) => city.countryId === editForm.countryId))

watch(filteredCities, (items) => {
  if (items.length && !items.some((city) => city.id === editForm.cityId)) {
    editForm.cityId = items[0].id
  }
})

const hydrate = (data: UserProfile) => {
  Object.assign(form, data)
  Object.assign(editForm, data)
}

onMounted(async () => {
  const [profileData, countryData, cityData] = await Promise.all([
    api.getMe(),
    api.getCountries(),
    api.getCities()
  ])
  profile.value = profileData
  countries.value = countryData
  cities.value = cityData
  hydrate(profileData)
  loading.value = false
})

const startEditing = () => {
  message.value = ''
  error.value = ''
  Object.assign(editForm, form)
  editing.value = true
}

const cancelEditing = () => {
  Object.assign(editForm, form)
  editing.value = false
  message.value = ''
  error.value = ''
}

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
    const updated = await api.updateProfile({
      ...editForm,
      email: form.email
    })
    profile.value = updated
    hydrate(updated)
    editing.value = false
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
  if (data?.message) return Array.isArray(data.errors) ? `${data.message} ${data.errors.join(' ')}` : data.message
  if (data?.title) return data.title
  if (data?.errors) return Object.values(data.errors).flat().join(' ')
  return fallback
}
</script>
