<template>
  <div class="auth-page">
    <section class="auth-panel">
      <NuxtLink class="brand" to="/login">
        <img src="/icon.png" alt="MyTripStoria" />
        <span>MyTripStoria</span>
      </NuxtLink>

      <div>
        <p class="eyebrow">New password</p>
        <h1 class="title">Choose a fresh password.</h1>
        <p class="subtitle">Use the reset code from your recovery instructions to secure your account.</p>
      </div>

      <form class="form" @submit.prevent="submit">
        <label class="field">
          <span>Email</span>
          <input v-model.trim="form.email" class="input" type="email" autocomplete="email" required />
        </label>

        <label class="field">
          <span>Reset code</span>
          <textarea v-model.trim="form.resetCode" class="textarea code-field" required />
        </label>

        <div class="grid-2">
          <label class="field">
            <span>New password</span>
            <input v-model="form.newPassword" class="input" type="password" autocomplete="new-password" required />
          </label>

          <label class="field">
            <span>Confirm password</span>
            <input v-model="form.confirmPassword" class="input" type="password" autocomplete="new-password" required />
          </label>
        </div>

        <p v-if="message" class="success">{{ message }}</p>
        <p v-if="error" class="error">{{ error }}</p>

        <button class="btn btn-primary" type="submit" :disabled="loading">
          {{ loading ? 'Updating password' : 'Update password' }}
        </button>
      </form>

      <p class="subtitle"><NuxtLink class="text-link strong" to="/login">Back to sign in</NuxtLink></p>
    </section>

    <AuthWorldMap />
  </div>
</template>

<script setup lang="ts">
const route = useRoute()
const api = useApi()
const loading = ref(false)
const message = ref('')
const error = ref('')

const form = reactive({
  email: typeof route.query.email === 'string' ? route.query.email : '',
  resetCode: '',
  newPassword: '',
  confirmPassword: ''
})

const submit = async () => {
  loading.value = true
  message.value = ''
  error.value = ''

  if (form.newPassword !== form.confirmPassword) {
    error.value = 'Passwords do not match.'
    loading.value = false
    return
  }

  try {
    await api.resetPassword(form.email, form.resetCode, form.newPassword)
    message.value = 'Your password has been updated. You can now sign in.'
  } catch {
    error.value = 'Password could not be reset. Please check your email, code, and new password.'
  } finally {
    loading.value = false
  }
}
</script>
