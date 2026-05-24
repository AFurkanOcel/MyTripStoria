<template>
  <div class="auth-page">
    <section class="auth-panel">
      <NuxtLink class="brand" to="/login">
        <img src="/icon.png" alt="MyTripStoria" />
        <span>MyTripStoria</span>
      </NuxtLink>
      <div>
        <p class="eyebrow">Seyahatlerini tek haritada topla</p>
        <h1 class="title">Tatil planların ve anıların için profesyonel alan.</h1>
        <p class="subtitle">Giriş yap, geçmiş tatillerini gör, yeni rotalar planla ve premium ile fotoğraf arşivini büyüt.</p>
      </div>
      <form class="form" @submit.prevent="submit">
        <label class="field">
          <span>E-posta</span>
          <input v-model="email" class="input" type="email" autocomplete="email" required />
        </label>
        <label class="field">
          <span>Şifre</span>
          <input v-model="password" class="input" type="password" autocomplete="current-password" required />
        </label>
        <p v-if="error" class="error">{{ error }}</p>
        <button class="btn btn-primary" type="submit" :disabled="loading">
          {{ loading ? 'Giriş yapılıyor' : 'Giriş yap' }}
        </button>
      </form>
      <p class="subtitle">Hesabın yok mu? <NuxtLink style="color: var(--primary); font-weight: 800;" to="/register">Kayıt ol</NuxtLink></p>
    </section>
    <section class="auth-visual" />
  </div>
</template>

<script setup lang="ts">
const api = useApi()
const email = ref('')
const password = ref('')
const loading = ref(false)
const error = ref('')

const submit = async () => {
  loading.value = true
  error.value = ''
  try {
    await api.login(email.value, password.value)
    await navigateTo('/')
  } catch {
    error.value = 'E-posta veya şifre hatalı.'
  } finally {
    loading.value = false
  }
}
</script>
