<template>
  <div>
    <header class="topbar">
      <div>
        <p class="eyebrow">Profile</p>
        <h1 class="section-title">Your travel identity</h1>
        <p class="subtitle">Review the information connected to your MyTripStoria account.</p>
      </div>
      <NuxtLink class="btn btn-primary" to="/profile/edit">Edit profile</NuxtLink>
    </header>

    <section v-if="loading" class="panel">
      <p class="subtitle">Loading profile...</p>
    </section>

    <section v-else-if="form.id" class="profile-grid">
      <aside class="panel profile-card">
        <img class="profile-photo-large" :src="profilePhoto" :alt="form.displayName || form.username" />
        <strong>{{ form.displayName || form.username || 'Traveler' }}</strong>
        <span>{{ form.isPremium ? 'Premium plan' : 'Standard plan' }}</span>
      </aside>

      <section class="panel profile-details">
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
    </section>
  </div>
</template>

<script setup lang="ts">
import type { UserProfile } from '~/types'

const api = useApi()
const profile = useState<UserProfile | null>('profile')
const loading = ref(true)

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

onMounted(async () => {
  const data = await api.getMe()
  profile.value = data
  Object.assign(form, data)
  loading.value = false
})
</script>
