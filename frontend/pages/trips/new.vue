<template>
  <div>
    <header class="topbar">
      <div>
        <p class="eyebrow">Yeni tatil</p>
        <h1 class="section-title">Plan veya geçmiş tatil kaydı oluştur.</h1>
        <p class="subtitle">Konum bilgisi girersen kayıt haritada renkli marker olarak görünür.</p>
      </div>
      <NuxtLink class="btn btn-ghost" to="/">Dashboard</NuxtLink>
    </header>

    <section class="panel">
      <form class="form" @submit.prevent="submit">
        <div class="grid-2">
          <label class="field">
            <span>Başlık</span>
            <input v-model="form.title" class="input" required />
          </label>
          <label class="field">
            <span>Tip</span>
            <input v-model="form.tripType" class="input" placeholder="Balayı, aile tatili, iş seyahati" required />
          </label>
        </div>

        <label class="field">
          <span>Açıklama</span>
          <textarea v-model="form.description" class="textarea" required />
        </label>

        <div class="grid-2">
          <label class="field">
            <span>Durum</span>
            <select v-model="form.status" class="select">
              <option value="Planned">Planlandı</option>
              <option value="Ongoing">Devam ediyor</option>
              <option value="Completed">Geçmiş</option>
              <option value="Cancelled">İptal</option>
            </select>
          </label>
          <label class="field">
            <span>Görünürlük</span>
            <select v-model="form.visibility" class="select">
              <option value="Private">Private</option>
              <option value="Public">Public</option>
            </select>
          </label>
        </div>

        <div class="grid-2">
          <label class="field">
            <span>Ülke ID</span>
            <input v-model.number="form.countryId" class="input" type="number" min="1" required />
          </label>
          <label class="field">
            <span>Şehir ID</span>
            <input v-model.number="form.cityId" class="input" type="number" min="1" required />
          </label>
        </div>

        <div class="grid-2">
          <label class="field">
            <span>Yer adı</span>
            <input v-model="form.placeName" class="input" placeholder="Kapadokya, Prag Old Town..." />
          </label>
          <label class="field">
            <span>Adres</span>
            <input v-model="form.address" class="input" />
          </label>
        </div>

        <div class="grid-2">
          <label class="field">
            <span>Enlem</span>
            <input v-model.number="form.latitude" class="input" type="number" step="0.000001" />
          </label>
          <label class="field">
            <span>Boylam</span>
            <input v-model.number="form.longitude" class="input" type="number" step="0.000001" />
          </label>
        </div>

        <div class="grid-2">
          <label class="field">
            <span>Başlangıç</span>
            <input v-model="form.startDate" class="input" type="date" required />
          </label>
          <label class="field">
            <span>Bitiş</span>
            <input v-model="form.endDate" class="input" type="date" required />
          </label>
        </div>

        <div class="grid-2">
          <label class="field">
            <span>Planlanan bütçe</span>
            <input v-model.number="form.plannedBudget" class="input" type="number" min="0" />
          </label>
          <label class="field">
            <span>Gerçek maliyet</span>
            <input v-model.number="form.actualCost" class="input" type="number" min="0" />
          </label>
        </div>

        <label class="field">
          <span>Notlar</span>
          <textarea v-model="form.notes" class="textarea" />
        </label>

        <p v-if="error" class="error">{{ error }}</p>
        <button class="btn btn-primary" type="submit" :disabled="loading">
          {{ loading ? 'Kaydediliyor' : 'Tatili kaydet' }}
        </button>
      </form>
    </section>
  </div>
</template>

<script setup lang="ts">
const api = useApi()
const profile = useState<any>('profile')
const loading = ref(false)
const error = ref('')
const today = new Date().toISOString().slice(0, 10)
const form = reactive({
  title: '',
  tripType: '',
  description: '',
  status: 'Planned',
  visibility: 'Private',
  countryId: 1,
  cityId: 1,
  placeName: '',
  address: '',
  latitude: undefined as number | undefined,
  longitude: undefined as number | undefined,
  startDate: today,
  endDate: today,
  plannedBudget: 0,
  actualCost: undefined as number | undefined,
  notes: ''
})

onMounted(async () => {
  if (!profile.value) profile.value = await api.getMe()
})

const submit = async () => {
  loading.value = true
  error.value = ''
  try {
    const trip = await api.createTrip({
      userId: profile.value.id,
      isCompleted: form.status === 'Completed',
      ...form
    })
    await navigateTo(`/trips/${trip.tripID}`)
  } catch {
    error.value = 'Tatil kaydı oluşturulamadı. Tarih, ülke/şehir veya konum bilgilerini kontrol et.'
  } finally {
    loading.value = false
  }
}
</script>
