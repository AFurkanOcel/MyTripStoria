<template>
  <div class="auth-page">
    <section class="auth-panel">
      <NuxtLink class="brand" to="/login">
        <img src="/icon.png" alt="MyTripStoria" />
        <span>MyTripStoria</span>
      </NuxtLink>

      <div>
        <p class="eyebrow">Password recovery</p>
        <h1 class="title">Reset access to your travel archive.</h1>
        <p class="subtitle">Enter your account email and we will prepare reset instructions for you.</p>
      </div>

      <form class="form" @submit.prevent="submit">
        <label class="field">
          <span>Email</span>
          <input v-model.trim="email" class="input" type="email" autocomplete="email" required />
        </label>

        <p v-if="message" class="success">{{ message }}</p>
        <p v-if="error" class="error">{{ error }}</p>

        <button class="btn btn-primary" type="submit" :disabled="loading">
          {{ loading ? 'Sending instructions' : 'Send reset instructions' }}
        </button>
      </form>

      <p class="subtitle">
        Already have a reset code?
        <NuxtLink class="text-link strong" :to="{ path: '/reset-password', query: email ? { email } : {} }">Reset password</NuxtLink>
      </p>
      <p class="subtitle"><NuxtLink class="text-link strong" to="/login">Back to sign in</NuxtLink></p>
    </section>

    <AuthWorldMap />
  </div>
</template>

<script setup lang="ts">
const api = useApi()
const email = ref('')
const loading = ref(false)
const message = ref('')
const error = ref('')

const submit = async () => {
  loading.value = true
  message.value = ''
  error.value = ''

  try {
    await api.forgotPassword(email.value)
    message.value = 'If an account exists, reset instructions have been sent.'
  } catch {
    error.value = 'Reset instructions could not be requested. Please try again.'
  } finally {
    loading.value = false
  }
}
</script>
