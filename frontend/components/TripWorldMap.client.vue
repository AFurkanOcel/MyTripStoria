<template>
  <div ref="mapElement" class="map" />
</template>

<script setup lang="ts">
import type { TripMarker } from '~/types'

const props = defineProps<{ markers: TripMarker[] }>()
const mapElement = ref<HTMLElement | null>(null)
let map: any
let layerGroup: any

const paintMarkers = async () => {
  if (!map) return
  const L = await import('leaflet')
  layerGroup.clearLayers()
  const bounds: any[] = []

  props.markers.forEach((marker) => {
    const icon = L.divIcon({
      className: '',
      html: `<span style="display:block;width:18px;height:18px;border-radius:999px;background:${marker.markerColor};border:3px solid white;box-shadow:0 8px 20px rgba(15,23,42,.28)"></span>`,
      iconSize: [18, 18],
      iconAnchor: [9, 9]
    })

    const point = [Number(marker.latitude), Number(marker.longitude)] as [number, number]
    bounds.push(point)

    L.marker(point, { icon })
      .bindPopup(`<strong>${marker.title}</strong><br>${marker.cityName || ''} ${marker.countryName || ''}<br>${marker.status}`)
      .addTo(layerGroup)
  })

  if (bounds.length === 1) {
    map.setView(bounds[0], 5)
  } else if (bounds.length > 1) {
    map.fitBounds(bounds, { padding: [36, 36], maxZoom: 6 })
  }

  setTimeout(() => map.invalidateSize(), 0)
}

onMounted(async () => {
  if (!mapElement.value) return
  const L = await import('leaflet')
  map = L.map(mapElement.value, { zoomControl: true }).setView([39, 35], 3)
  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; OpenStreetMap contributors',
    maxZoom: 18
  }).addTo(map)
  layerGroup = L.layerGroup().addTo(map)
  setTimeout(() => map.invalidateSize(), 0)
  await paintMarkers()
})

watch(() => props.markers, paintMarkers, { deep: true })
</script>
