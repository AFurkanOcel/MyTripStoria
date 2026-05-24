export type TripStatus = 'Planned' | 'Ongoing' | 'Completed' | 'Cancelled'

export interface UserProfile {
  id: number
  username: string
  email: string
  phoneNumber: string
  age: number
  countryId: number
  cityId: number
  countryName?: string
  cityName?: string
  address?: string
  budget: number
  isPremium: boolean
}

export interface Country {
  id: number
  name: string
  latitude?: number
  longitude?: number
}

export interface City {
  id: number
  name: string
  countryId: number
  latitude?: number
  longitude?: number
}

export interface TripPhoto {
  id: number
  url: string
  originalFileName: string
  contentType: string
  sizeInBytes: number
  caption?: string
  isCover: boolean
  sortOrder: number
  takenAt?: string
}

export interface Trip {
  tripID: number
  userId: number
  title: string
  tripType: string
  isCompleted: boolean
  status: TripStatus
  visibility: string
  description: string
  countryId: number
  cityId: number
  countryName?: string
  cityName?: string
  placeName?: string
  address?: string
  latitude?: number
  longitude?: number
  coverImageUrl?: string
  rating?: number
  plannedBudget?: number
  actualCost?: number
  favoriteMoments?: string
  startDate: string
  endDate: string
  notes?: string
  photos: TripPhoto[]
}

export interface TripMarker {
  tripID: number
  title: string
  status: TripStatus
  markerType: string
  markerColor: string
  placeName?: string
  cityName?: string
  countryName?: string
  latitude: number
  longitude: number
  startDate: string
  endDate: string
}

export interface DashboardSummary {
  totalTrips: number
  plannedTrips: number
  ongoingTrips: number
  completedTrips: number
  cancelledTrips: number
  totalPlannedBudget: number
  totalActualCost: number
  nextTripStartDate?: string
  nextTripTitle?: string
  visitedCountryCount: number
  visitedCityCount: number
  totalTravelDays: number
}
