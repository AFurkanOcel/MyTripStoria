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
const route = useRoute()
const mapElement = ref<HTMLElement | null>(null)
let map: any

const resizeMap = () => {
  if (!map) return
  map.invalidateSize()
}

const mountMap = async () => {
  if (!mapElement.value || map) return

  const L = await import('leaflet')
  map = L.map(mapElement.value, {
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

  await nextTick()
  resizeMap()
  setTimeout(resizeMap, 80)
  setTimeout(resizeMap, 300)
}

onMounted(mountMap)

watch(() => route.path, async () => {
  await nextTick()
  setTimeout(resizeMap, 0)
})
</script>
