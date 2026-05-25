<template>
  <div>
    <header class="topbar">
      <div>
        <p class="eyebrow">Settings</p>
        <h1 class="section-title">Workspace settings</h1>
        <p class="subtitle">Manage account security and application preferences.</p>
      </div>
    </header>

    <section class="profile-grid">
      <aside class="panel">
        <h2 style="margin-top:0;">Account security</h2>
        <p class="subtitle">Update your password regularly to keep your travel archive protected.</p>
      </aside>

      <section class="panel">
        <form class="form" @submit.prevent="changePassword">
          <label class="field">
            <span>Current password</span>
            <input v-model="form.currentPassword" class="input" type="password" autocomplete="current-password" required />
          </label>

          <div class="grid-2">
            <label class="field">
              <span>New password</span>
              <input v-model="form.newPassword" class="input" type="password" autocomplete="new-password" required />
            </label>

            <label class="field">
              <span>Confirm new password</span>
              <input v-model="form.confirmPassword" class="input" type="password" autocomplete="new-password" required />
            </label>
          </div>

          <p v-if="message" class="success">{{ message }}</p>
          <p v-if="error" class="error">{{ error }}</p>

          <button class="btn btn-primary" type="submit" :disabled="saving">
            {{ saving ? 'Updating password' : 'Update password' }}
          </button>
        </form>
      </section>
    </section>
  </div>
</template>

<script setup lang="ts">
const api = useApi()
const saving = ref(false)
const message = ref('')
const error = ref('')

const form = reactive({
  currentPassword: '',
  newPassword: '',
  confirmPassword: ''
})

const changePassword = async () => {
  message.value = ''
  error.value = ''

  if (form.newPassword !== form.confirmPassword) {
    error.value = 'New passwords do not match.'
    return
  }

  saving.value = true
  try {
    await api.changePassword(form.currentPassword, form.newPassword)
    form.currentPassword = ''
    form.newPassword = ''
    form.confirmPassword = ''
    message.value = 'Password updated.'
  } catch (changeError: any) {
    error.value = getErrorMessage(changeError, 'Password could not be updated.')
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
