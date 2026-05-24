<template>
  <div class="auth-page">
    <section class="auth-panel">
      <NuxtLink class="brand" to="/login">
        <img src="/icon.png" alt="MyTripStoria" />
        <span>MyTripStoria</span>
      </NuxtLink>
      <div>
        <p class="eyebrow">Yeni hesap</p>
        <h1 class="title">MyTripStoria hesabını oluştur.</h1>
        <p class="subtitle">Kayıttan sonra profilin oluşturulur ve tatil kayıtların hesabına bağlı saklanır.</p>
      </div>
      <form class="form" @submit.prevent="submit">
        <div class="grid-2">
          <label class="field">
            <span>Kullanıcı adı</span>
            <input v-model="form.username" class="input" required />
          </label>
          <label class="field">
            <span>E-posta</span>
            <input v-model="form.email" class="input" type="email" required />
          </label>
        </div>
        <div class="grid-2">
          <label class="field">
            <span>Şifre</span>
            <input v-model="form.password" class="input" type="password" required />
          </label>
          <label class="field">
            <span>Telefon</span>
            <input v-model="form.phoneNumber" class="input" />
          </label>
        </div>
        <div class="grid-2">
          <label class="field">
            <span>Yaş</span>
            <input v-model.number="form.age" class="input" type="number" min="13" required />
          </label>
          <label class="field">
            <span>Bütçe</span>
            <input v-model.number="form.budget" class="input" type="number" min="0" />
          </label>
        </div>
        <div class="grid-2">
          <label class="field">
            <span>Ülke</span>
            <select v-model.number="form.countryId" class="select" required>
              <option v-for="country in countries" :key="country.id" :value="country.id">{{ country.name }}</option>
            </select>
          </label>
          <label class="field">
            <span>Şehir</span>
            <select v-model.number="form.cityId" class="select" required>
              <option v-for="city in filteredCities" :key="city.id" :value="city.id">{{ city.name }}</option>
            </select>
          </label>
        </div>
        <label class="field">
          <span>Adres</span>
          <input v-model="form.address" class="input" />
        </label>
        <label class="field" style="display:flex; grid-template-columns: auto 1fr; align-items:center; gap:10px;">
          <input v-model="form.isPremium" type="checkbox" />
          <span>Premium planı dene</span>
        </label>
        <p v-if="error" class="error">{{ error }}</p>
        <button class="btn btn-primary" type="submit" :disabled="loading">
          {{ loading ? 'Hesap oluşturuluyor' : 'Kayıt ol' }}
        </button>
      </form>
      <p class="subtitle">Zaten hesabın var mı? <NuxtLink style="color: var(--primary); font-weight: 800;" to="/login">Giriş yap</NuxtLink></p>
    </section>
    <section class="auth-visual" />
  </div>
</template>

<script setup lang="ts">
import type { City, Country } from '~/types'

const api = useApi()
const loading = ref(false)
const error = ref('')
const countries = ref<Country[]>([])
const cities = ref<City[]>([])
const form = reactive({
  username: '',
  email: '',
  password: '',
  phoneNumber: '',
  age: 18,
  countryId: 1,
  cityId: 1,
  address: '',
  budget: 0,
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
    error.value = 'Ülke ve şehir listesi alınamadı. Backend çalışıyor mu kontrol et.'
  }
})

const submit = async () => {
  loading.value = true
  error.value = ''
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
      phoneNumber: form.phoneNumber,
      age: form.age,
      countryId: form.countryId,
      cityId: form.cityId,
      address: form.address,
      budget: form.budget,
      isPremium: form.isPremium
    })
    await navigateTo('/')
  } catch (err) {
    const message = getApiErrorMessage(err)
    error.value = message
      ? `Kayit tamamlanamadi: ${message}`
      : 'Kayit tamamlanamadi. E-posta, sifre, kullanici adi veya ulke/sehir bilgisini kontrol et.'
  } finally {
    loading.value = false
  }
}
</script>
