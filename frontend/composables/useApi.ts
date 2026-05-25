import type { City, Country, DashboardSummary, Trip, TripMarker, UserProfile } from '~/types'

interface LoginResult {
  tokenType: string
  accessToken: string
  expiresIn: number
  refreshToken?: string
}

const tokenKey = 'mytripstoria.accessToken'

export const useApi = () => {
  const config = useRuntimeConfig()
  const apiBase = config.public.apiBase as string
  const token = useState<string | null>('auth-token', () => null)

  const loadToken = () => {
    if (process.client && !token.value) {
      token.value = localStorage.getItem(tokenKey)
    }
    return token.value
  }

  const setToken = (value: string | null) => {
    token.value = value
    if (process.client) {
      if (value) localStorage.setItem(tokenKey, value)
      else localStorage.removeItem(tokenKey)
    }
  }

  const request = async <T>(path: string, options: any = {}) => {
    const headers = new Headers(options.headers || {})
    const currentToken = loadToken()

    if (currentToken) headers.set('Authorization', `Bearer ${currentToken}`)
    if (options.body && !(options.body instanceof FormData)) headers.set('Content-Type', 'application/json')

    return await $fetch<T>(`${apiBase}${path}`, {
      ...options,
      headers,
      body: options.body && !(options.body instanceof FormData) ? JSON.stringify(options.body) : options.body
    })
  }

  const login = async (email: string, password: string) => {
    const result = await $fetch<LoginResult>(`${apiBase}/login?useCookies=false`, {
      method: 'POST',
      body: { email, password }
    })

    setToken(result.accessToken)
    return result
  }

  const registerIdentity = (email: string, password: string) => {
    return $fetch(`${apiBase}/register`, {
      method: 'POST',
      body: { email, password }
    })
  }

  const forgotPassword = (email: string) => {
    return $fetch(`${apiBase}/forgotPassword`, {
      method: 'POST',
      body: { email }
    })
  }

  const resetPassword = (email: string, resetCode: string, newPassword: string) => {
    return $fetch(`${apiBase}/resetPassword`, {
      method: 'POST',
      body: { email, resetCode, newPassword }
    })
  }

  const logout = () => setToken(null)
  const getMe = () => request<UserProfile>('/api/users/me')
  const createProfile = (profile: Partial<UserProfile>) => request<UserProfile>('/api/users', { method: 'POST', body: profile })
  const updateProfile = (profile: UserProfile) => request<UserProfile>(`/api/users/${profile.id}`, { method: 'PUT', body: profile })
  const uploadProfilePhoto = (file: File) => {
    const formData = new FormData()
    formData.append('file', file)
    return request<UserProfile>('/api/users/me/photo', { method: 'POST', body: formData })
  }
  const getCountries = () => request<Country[]>('/api/countries')
  const getCities = () => request<City[]>('/api/cities')
  const getTrips = () => request<Trip[]>('/api/trips/me')
  const getTrip = (id: string | number) => request<Trip>(`/api/trips/${id}`)
  const getSummary = () => request<DashboardSummary>('/api/trips/me/summary')
  const getMarkers = () => request<TripMarker[]>('/api/trips/me/map-markers')
  const createTrip = (trip: Record<string, unknown>) => request<Trip>('/api/trips', { method: 'POST', body: trip })
  const updateTrip = (tripId: number, trip: Record<string, unknown>) => request<Trip>(`/api/trips/${tripId}`, { method: 'PUT', body: trip })
  const deleteTrip = (tripId: number) => request(`/api/trips/${tripId}`, { method: 'DELETE' })

  const uploadPhoto = (tripId: number, file: File, caption?: string) => {
    const formData = new FormData()
    formData.append('file', file)
    if (caption) formData.append('caption', caption)
    return request(`/api/trips/${tripId}/photos`, { method: 'POST', body: formData })
  }
  const deletePhoto = (tripId: number, photoId: number) => request(`/api/trips/${tripId}/photos/${photoId}`, { method: 'DELETE' })

  return {
    apiBase,
    token,
    loadToken,
    setToken,
    request,
    login,
    registerIdentity,
    forgotPassword,
    resetPassword,
    logout,
    getMe,
    createProfile,
    updateProfile,
    uploadProfilePhoto,
    getCountries,
    getCities,
    getTrips,
    getTrip,
    getSummary,
    getMarkers,
    createTrip,
    updateTrip,
    deleteTrip,
    uploadPhoto,
    deletePhoto
  }
}
