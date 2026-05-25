<template>
  <div>
    <header class="topbar">
      <div>
        <p class="eyebrow">Edit trip</p>
        <h1 class="section-title">Update your trip record.</h1>
        <p class="subtitle">Adjust the visible trip details while MyTripStoria keeps the map data in the background.</p>
      </div>
      <NuxtLink v-if="trip" class="btn btn-ghost" :to="`/trips/${trip.tripID}`">Trip details</NuxtLink>
    </header>

    <section class="panel">
      <form class="form" @submit.prevent="submit">
        <div class="grid-2">
          <label class="field">
            <span>Title</span>
            <input v-model.trim="form.title" class="input" required />
          </label>
          <label class="field">
            <span>Type</span>
            <input v-model.trim="form.tripType" class="input" placeholder="Honeymoon, family trip, business travel" required />
          </label>
        </div>

        <label class="field">
          <span>Description</span>
          <textarea v-model.trim="form.description" class="textarea" required />
        </label>

        <div class="grid-2">
          <label class="field">
            <span>Status</span>
            <select v-model="form.status" class="select">
              <option value="Planned">Planned</option>
              <option value="Ongoing">Ongoing</option>
              <option value="Completed">Completed</option>
              <option value="Cancelled">Cancelled</option>
            </select>
          </label>
          <label class="field">
            <span>Visibility</span>
            <select v-model="form.visibility" class="select">
              <option value="Private">Private</option>
              <option value="Public">Public</option>
            </select>
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

        <div class="grid-2">
          <label class="field">
            <span>Place name</span>
            <input v-model.trim="form.placeName" class="input" placeholder="Buda Castle, Old Town, beach house..." />
          </label>
          <label class="field">
            <span>Budget</span>
            <input v-model.number="form.budget" class="input" type="number" min="0" />
          </label>
        </div>

        <div class="grid-2">
          <label class="field">
            <span>Start date</span>
            <input v-model="form.startDate" class="input" type="date" required />
          </label>
          <label class="field">
            <span>End date</span>
            <input v-model="form.endDate" class="input" type="date" required />
          </label>
        </div>

        <label class="field">
          <span>Notes</span>
          <textarea v-model.trim="form.notes" class="textarea" />
        </label>

        <p v-if="error" class="error">{{ error }}</p>
        <button class="btn btn-primary" type="submit" :disabled="loading">
          {{ loading ? 'Saving changes' : 'Save changes' }}
        </button>
      </form>
    </section>
  </div>
</template>

<script setup lang="ts">
import type { City, Country, Trip, UserProfile } from '~/types'

const route = useRoute()
const api = useApi()
const profile = useState<UserProfile | null>('profile')
const trip = ref<Trip | null>(null)
const loading = ref(false)
const error = ref('')
const countries = ref<Country[]>([])
const cities = ref<City[]>([])

const form = reactive({
  title: '',
  tripType: '',
  description: '',
  status: 'Planned',
  visibility: 'Private',
  countryId: 1,
  cityId: 1,
  placeName: '',
  budget: 0,
  startDate: '',
  endDate: '',
  notes: ''
})

const filteredCities = computed(() => cities.value.filter((city) => city.countryId === form.countryId))
const selectedCity = computed(() => cities.value.find((city) => city.id === form.cityId))
const selectedCountry = computed(() => countries.value.find((country) => country.id === form.countryId))

watch(filteredCities, (items) => {
  if (items.length && !items.some((city) => city.id === form.cityId)) {
    form.cityId = items[0].id
  }
})

onMounted(async () => {
  const [profileData, tripData, countryData, cityData] = await Promise.all([
    profile.value ? Promise.resolve(profile.value) : api.getMe(),
    api.getTrip(route.params.id as string),
    api.getCountries(),
    api.getCities()
  ])

  profile.value = profileData
  trip.value = tripData
  countries.value = countryData
  cities.value = cityData
  hydrate(tripData)
})

const hydrate = (data: Trip) => {
  form.title = data.title
  form.tripType = data.tripType
  form.description = data.description
  form.status = data.status
  form.visibility = data.visibility
  form.countryId = data.countryId
  form.cityId = data.cityId
  form.placeName = data.placeName || ''
  form.budget = data.plannedBudget || 0
  form.startDate = data.startDate.slice(0, 10)
  form.endDate = data.endDate.slice(0, 10)
  form.notes = data.notes || ''
}

const submit = async () => {
  if (!profile.value || !trip.value) return

  loading.value = true
  error.value = ''

  try {
    const latitude = selectedCity.value?.latitude ?? selectedCountry.value?.latitude
    const longitude = selectedCity.value?.longitude ?? selectedCountry.value?.longitude

    const updated = await api.updateTrip(trip.value.tripID, {
      userId: profile.value.id,
      isCompleted: form.status === 'Completed',
      title: form.title,
      tripType: form.tripType,
      description: form.description,
      status: form.status,
      visibility: form.visibility,
      countryId: form.countryId,
      cityId: form.cityId,
      placeName: form.placeName,
      address: '',
      latitude,
      longitude,
      plannedBudget: form.budget,
      actualCost: undefined,
      startDate: form.startDate,
      endDate: form.endDate,
      notes: form.notes,
      photos: []
    })
    await navigateTo(`/trips/${updated.tripID}`)
  } catch (saveError: any) {
    error.value = getErrorMessage(saveError, 'The trip could not be updated. Please check the dates and location.')
  } finally {
    loading.value = false
  }
}

const getErrorMessage = (apiError: any, fallback: string) => {
  const data = apiError?.data
  if (typeof data === 'string') return data
  if (data?.title) return data.title
  if (data?.errors) return Object.values(data.errors).flat().join(' ')
  return fallback
}
</script>
