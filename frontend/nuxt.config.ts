export default defineNuxtConfig({
  compatibilityDate: '2025-01-01',
  experimental: {
    appManifest: false
  },
  css: ['~/assets/css/main.css', 'leaflet/dist/leaflet.css'],
  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || 'http://localhost:5155'
    }
  },
  app: {
    head: {
      title: 'MyTripStoria',
      meta: [
        { name: 'viewport', content: 'width=device-width, initial-scale=1' },
        { name: 'description', content: 'Plan trips, preserve travel memories, and explore them on a map.' }
      ],
      link: [{ rel: 'icon', type: 'image/png', href: '/icon.png' }]
    }
  }
})
