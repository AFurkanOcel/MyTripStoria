<template>
  <main class="landing-page">
    <nav class="landing-nav">
      <NuxtLink class="brand landing-brand" to="/">
        <img src="/icon.png" alt="MyTripStoria" />
        <span>MyTripStoria</span>
      </NuxtLink>

      <div class="actions">
        <NuxtLink v-if="hasToken" class="btn btn-ghost" to="/dashboard">Open dashboard</NuxtLink>
        <template v-else>
          <NuxtLink class="btn btn-ghost" to="/login">Log in</NuxtLink>
          <NuxtLink class="btn btn-primary" to="/register">Create account</NuxtLink>
        </template>
      </div>
    </nav>

    <section class="landing-hero">
      <div class="landing-copy">
        <p class="eyebrow">Travel planning and memory keeping</p>
        <h1>Plan future escapes and preserve every trip you have lived.</h1>
        <p>
          MyTripStoria turns your vacations into a beautiful map-based archive, with planned routes,
          completed memories, travel notes, and premium photo collections in one private workspace.
        </p>
        <div class="actions">
          <NuxtLink v-if="hasToken" class="btn btn-primary" to="/dashboard">Open dashboard</NuxtLink>
          <template v-else>
            <NuxtLink class="btn btn-primary" to="/register">Create your travel story</NuxtLink>
            <NuxtLink class="btn btn-ghost" to="/login">Log in</NuxtLink>
          </template>
        </div>
      </div>

      <div class="landing-map">
        <AuthWorldMap />
        <div class="landing-map-card">
          <span class="badge badge-completed">Completed</span>
          <strong>Budapest weekend</strong>
          <small>Saved with notes, dates, and photos</small>
        </div>
        <div class="landing-route-card">
          <span class="badge badge-planned">Planned</span>
          <strong>Tokyo spring route</strong>
          <small>Upcoming itinerary on your map</small>
        </div>
      </div>
    </section>

    <section class="landing-features">
      <article>
        <span>01</span>
        <h2>Plan what is next</h2>
        <p>Create future trips with dates, locations, budgets, and route context before you travel.</p>
      </article>
      <article>
        <span>02</span>
        <h2>Remember where you have been</h2>
        <p>Keep completed vacations visible on a world map, separated from planned adventures.</p>
      </article>
      <article>
        <span>03</span>
        <h2>Add premium memories</h2>
        <p>Premium accounts can upload photos so each trip record feels personal and complete.</p>
      </article>
    </section>

    <section class="landing-pricing">
      <div>
        <p class="eyebrow">Plans</p>
        <h2>Start simple, upgrade when your archive grows.</h2>
      </div>
      <div class="pricing-grid">
        <article>
          <h3>Standard</h3>
          <p>Trip planning, completed travel records, and map markers for your personal archive.</p>
        </article>
        <article class="pricing-premium">
          <h3>Premium</h3>
          <p>Everything in Standard, plus photo uploads for richer visual memories.</p>
        </article>
      </div>
    </section>
  </main>
</template>

<script setup lang="ts">
const api = useApi()
const hasToken = ref(false)

onMounted(() => {
  hasToken.value = Boolean(api.loadToken())
})
</script>
