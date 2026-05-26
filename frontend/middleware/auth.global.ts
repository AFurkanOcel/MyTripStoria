export default defineNuxtRouteMiddleware((to) => {
  if (process.server) return

  const publicRoutes = ['/', '/login', '/register', '/forgot-password', '/reset-password']
  const authRoutes = ['/login', '/register', '/forgot-password', '/reset-password']
  const api = useApi()
  const token = api.loadToken()

  if (!token && !publicRoutes.includes(to.path)) return navigateTo('/login')
  if (token && authRoutes.includes(to.path)) return navigateTo('/dashboard')
})
