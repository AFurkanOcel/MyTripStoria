<template>
  <div class="auth-page">
    <section class="auth-panel">
      <NuxtLink class="brand" to="/login">
        <img src="/icon.png" alt="MyTripStoria" />
        <span>MyTripStoria</span>
      </NuxtLink>

      <div>
        <p class="eyebrow">New account</p>
        <h1 class="title">Create your MyTripStoria account.</h1>
        <p class="subtitle">Your profile keeps every planned route and completed trip connected to your own travel story.</p>
      </div>

      <form class="form" @submit.prevent="submit">
        <div class="grid-2">
          <label class="field">
            <span>Username</span>
            <input v-model.trim="form.username" class="input" autocomplete="username" required />
          </label>

          <label class="field">
            <span>Email</span>
            <input v-model.trim="form.email" class="input" type="email" autocomplete="email" required />
          </label>
        </div>

        <div class="grid-2">
          <label class="field">
            <span>Password</span>
            <input v-model="form.password" class="input" type="password" autocomplete="new-password" required />
          </label>

          <label class="field">
            <span>Confirm password</span>
            <input v-model="form.confirmPassword" class="input" type="password" autocomplete="new-password" required />
          </label>
        </div>

        <div class="grid-2">
          <label class="field">
            <span>Age</span>
            <input v-model.number="form.age" class="input" type="number" min="13" required />
          </label>

          <label class="field">
            <span>Phone number</span>
            <div class="phone-input">
              <select v-model="form.phoneCountryCode" class="select phone-code" aria-label="Phone country code">
                <option v-for="code in phoneCountryCodes" :key="code" :value="code">{{ code }}</option>
              </select>
              <input
                v-model="form.localPhoneNumber"
                class="input"
                inputmode="numeric"
                maxlength="10"
                pattern="[0-9]{10}"
                placeholder="5551234567"
                required
                @input="sanitizePhone"
              />
            </div>
          </label>
        </div>

        <div class="grid-2">
          <label class="field">
            <span>Country</span>
            <select v-model.number="form.countryId" class="select" required>
              <option v-for="country in countries" :key="country.id" :value="country.id">{{ country.name }}</option>
            </select>
          </label>

          <label class="field">
            <span>City</span>
            <select v-model.number="form.cityId" class="select" required>
              <option v-for="city in filteredCities" :key="city.id" :value="city.id">{{ city.name }}</option>
            </select>
          </label>
        </div>

        <label class="field">
          <span>Address</span>
          <input v-model.trim="form.address" class="input" autocomplete="street-address" />
        </label>

        <label class="check-field">
          <input v-model="form.isPremium" type="checkbox" />
          <span>Start with the premium plan</span>
        </label>

        <p v-if="error" class="error">{{ error }}</p>

        <button class="btn btn-primary" type="submit" :disabled="loading">
          {{ loading ? 'Creating account' : 'Create account' }}
        </button>
      </form>

      <p class="subtitle">Already have an account? <NuxtLink class="text-link strong" to="/login">Sign in</NuxtLink></p>
    </section>

    <AuthWorldMap />
  </div>
</template>

<script setup lang="ts">
import type { City, Country } from '~/types'

const api = useApi()
const loading = ref(false)
const error = ref('')
const countries = ref<Country[]>([])
const cities = ref<City[]>([])
const phoneCountryCodes = ['+90', '+1', '+44', '+49', '+33', '+39', '+34', '+31', '+81', '+66']

const form = reactive({
  username: '',
  email: '',
  password: '',
  confirmPassword: '',
  phoneCountryCode: '+90',
  localPhoneNumber: '',
  age: 18,
  countryId: 1,
  cityId: 1,
  address: '',
  isPremium: false
})

const filteredCities = computed(() => cities.value.filter((city) => city.countryId === form.countryId))

const getApiErrorMessage = (err: unknown) => {
  const apiError = err as {
    statusCode?: number
    statusMessage?: string
    data?: {
      title?: string
      detail?: string
      errors?: Record<string, string[]>
    }
  }

  if (apiError.data?.errors) {
    const messages = Object.values(apiError.data.errors).flat()
    if (messages.length) return messages.join(' ')
  }

  return apiError.data?.detail || apiError.data?.title || apiError.statusMessage || ''
}

const sanitizePhone = () => {
  form.localPhoneNumber = form.localPhoneNumber.replace(/\D/g, '').slice(0, 10)
}

watch(filteredCities, (items) => {
  if (items.length && !items.some((city) => city.id === form.cityId)) form.cityId = items[0].id
})

onMounted(async () => {
  try {
    const [countryData, cityData] = await Promise.all([api.getCountries(), api.getCities()])
    countries.value = countryData
    cities.value = cityData
    if (countries.value.length) form.countryId = countries.value[0].id
    if (filteredCities.value.length) form.cityId = filteredCities.value[0].id
  } catch {
    error.value = 'Country and city options could not be loaded. Please check that the backend is running.'
  }
})

const submit = async () => {
  loading.value = true
  error.value = ''
  sanitizePhone()

  if (form.password !== form.confirmPassword) {
    error.value = 'Passwords do not match.'
    loading.value = false
    return
  }

  if (form.localPhoneNumber.length !== 10) {
    error.value = 'Phone number must contain exactly 10 digits.'
    loading.value = false
    return
  }

  try {
    try {
      await api.registerIdentity(form.email, form.password)
    } catch (registerError) {
      const message = getApiErrorMessage(registerError).toLowerCase()
      if (!message.includes('already') && !message.includes('taken')) {
        throw registerError
      }
    }

    await api.login(form.email, form.password)
    await api.createProfile({
      username: form.username,
      email: form.email,
      phoneNumber: `${form.phoneCountryCode}${form.localPhoneNumber}`,
      age: form.age,
      countryId: form.countryId,
      cityId: form.cityId,
      address: form.address,
      budget: 0,
      isPremium: form.isPremium
    })
    await navigateTo('/')
  } catch (err) {
    const message = getApiErrorMessage(err)
    error.value = message
      ? `Registration could not be completed: ${message}`
      : 'Registration could not be completed. Please check your email, password, username, and location.'
  } finally {
    loading.value = false
  }
}
</script>
