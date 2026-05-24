<template>
  <div v-if="isAuthPage">
    <slot />
  </div>
  <div v-else class="app-shell">
    <aside class="sidebar">
      <NuxtLink class="brand" to="/">
        <img src="/icon.png" alt="MyTripStoria" />
        <span>MyTripStoria</span>
      </NuxtLink>
      <nav class="nav">
        <NuxtLink to="/">Dashboard</NuxtLink>
        <NuxtLink to="/trips/new">Yeni tatil</NuxtLink>
        <button type="button" @click="signOut">Çıkış yap</button>
      </nav>
      <div class="premium-note" v-if="profile">
        {{ profile.isPremium ? 'Premium hesap: fotoğraf yükleme açık.' : 'Normal hesap: fotoğraf yükleme premium ile açılır.' }}
      </div>
    </aside>
    <main class="main">
      <slot />
    </main>
  </div>
</template>

<script setup lang="ts">
import type { UserProfile } from '~/types'

const route = useRoute()
const api = useApi()
const profile = useState<UserProfile | null>('profile', () => null)
const isAuthPage = computed(() => ['/login', '/register'].includes(route.path))

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

const signOut = async () => {
  api.logout()
  profile.value = null
  await navigateTo('/login')
}
</script>
