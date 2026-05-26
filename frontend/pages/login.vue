<template>
  <div class="auth-page">
    <section class="auth-panel">
      <NuxtLink class="brand" to="/">
        <img src="/icon.png" alt="MyTripStoria" />
        <span>MyTripStoria</span>
      </NuxtLink>

      <div>
        <p class="eyebrow">Your trips on one map</p>
        <h1 class="title">Plan future escapes and keep every travel memory in one place</h1>
        <p class="subtitle">Sign in to revisit past trips, plan new routes, and grow your travel archive with premium photo memories.</p>
      </div>

      <form class="form" @submit.prevent="submit">
        <label class="field">
          <span>Email</span>
          <input v-model="email" class="input" type="email" autocomplete="email" required />
        </label>

        <label class="field">
          <span>Password</span>
          <input v-model="password" class="input" type="password" autocomplete="current-password" required />
        </label>

        <div class="form-row">
          <NuxtLink class="text-link" to="/forgot-password">Forgot password?</NuxtLink>
        </div>

        <p v-if="error" class="error">{{ error }}</p>

        <button class="btn btn-primary" type="submit" :disabled="loading">
          {{ loading ? 'Signing in' : 'Sign in' }}
        </button>
      </form>

      <p class="subtitle">Do not have an account? <NuxtLink class="text-link strong" to="/register">Create one</NuxtLink></p>
    </section>

    <AuthWorldMap />
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
    await navigateTo('/dashboard')
  } catch {
    error.value = 'The email or password is incorrect.'
  } finally {
    loading.value = false
  }
}
</script>
