<template>
  <div v-if="isAuthPage">
    <slot />
  </div>

  <div v-else class="app-shell">
    <aside class="sidebar">
      <div class="sidebar-top">
        <NuxtLink class="brand" to="/dashboard">
          <img src="/icon.png" alt="MyTripStoria" />
          <span>MyTripStoria</span>
        </NuxtLink>

        <NuxtLink v-if="profile" class="sidebar-profile" to="/profile">
          <img :src="profilePhoto" :alt="displayName" />
          <span>
            <strong>{{ displayName }}</strong>
            <small>{{ profile.isPremium ? 'Premium' : 'Standard' }}</small>
          </span>
        </NuxtLink>

        <nav class="nav">
          <NuxtLink to="/dashboard">Dashboard</NuxtLink>
          <NuxtLink to="/trips">Trip records</NuxtLink>
          <NuxtLink to="/trips/new">New trip</NuxtLink>
          <NuxtLink to="/profile">Profile</NuxtLink>
          <NuxtLink to="/settings">Settings</NuxtLink>
        </nav>
      </div>

      <button class="sidebar-signout" type="button" @click="showSignOut = true">Sign out</button>
    </aside>

    <main class="main">
      <slot />
    </main>

    <div v-if="showSignOut" class="modal-backdrop" role="dialog" aria-modal="true" aria-labelledby="sign-out-title">
      <section class="modal">
        <h2 id="sign-out-title">Sign out?</h2>
        <p>Are you sure you want to sign out of MyTripStoria?</p>
        <div class="actions" style="justify-content: flex-end;">
          <button class="btn btn-ghost" type="button" @click="showSignOut = false">Cancel</button>
          <button class="btn btn-primary" type="button" @click="confirmSignOut">Sign out</button>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { UserProfile } from '~/types'

const route = useRoute()
const api = useApi()
const profile = useState<UserProfile | null>('profile', () => null)
const showSignOut = ref(false)
const publicShellRoutes = ['/', '/login', '/register', '/forgot-password', '/reset-password']
const isAuthPage = computed(() => publicShellRoutes.includes(route.path))

const displayName = computed(() => profile.value?.displayName || profile.value?.username || 'Traveler')
const profilePhoto = computed(() => {
  if (!profile.value?.profilePhotoUrl) return '/icon.png'
  return `${api.apiBase}${profile.value.profilePhotoUrl}`
})

onMounted(async () => {
  if (!isAuthPage.value && !profile.value) {
    try {
      profile.value = await api.getMe()
    } catch {
      api.logout()
      await navigateTo('/login')
    }
  }
})

const confirmSignOut = async () => {
  api.logout()
  profile.value = null
  showSignOut.value = false
  await navigateTo('/login')
}
</script>
