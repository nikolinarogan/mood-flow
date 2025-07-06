export interface User {
  id: number
  username: string
  email: string
  isEmailVerified: boolean
  createdDate: string
  lastLoginAt?: string
}

export interface AuthResponse {
  token: string
  username: string
  email: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  username: string
  email: string
  password: string
} 