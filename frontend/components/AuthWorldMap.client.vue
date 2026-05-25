<template>
  <section class="auth-visual">
    <div class="auth-map-shell" aria-label="World map preview">
      <div ref="mapElement" class="auth-map" />
      <a class="auth-map-attribution" href="https://www.openstreetmap.org/copyright" target="_blank" rel="noreferrer">
        © OpenStreetMap
      </a>
    </div>
  </section>
</template>

<script setup lang="ts">
const mapElement = ref<HTMLElement | null>(null)

onMounted(async () => {
  if (!mapElement.value) return

  const L = await import('leaflet')
  const map = L.map(mapElement.value, {
    attributionControl: false,
    boxZoom: false,
    doubleClickZoom: false,
    dragging: false,
    keyboard: false,
    scrollWheelZoom: false,
    tap: false,
    touchZoom: false,
    zoomControl: false
  }).setView([20, 8], 2)

  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 18,
    minZoom: 2
  }).addTo(map)

  setTimeout(() => map.invalidateSize(), 0)
})
</script>
