import { defineStore } from 'pinia'
import api from '@/services/api'
import type { User, AuthResponse, LoginRequest, RegisterRequest } from '@/types/auth'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null as User | null,
    token: localStorage.getItem('token') || null,
    isAuthenticated: !!localStorage.getItem('token')
  }),
  
  actions: {
    async login(email: string, password: string) {
 
      try {
        const response = await api.post<AuthResponse>('/auth/login', { email, password })
        this.token = response.data.token
        

        this.user = {
          id: 0, 
          username: response.data.username,
          email: response.data.email,
          isEmailVerified: true, 
          createdDate: new Date().toISOString(),
          lastLoginAt: new Date().toISOString()
        }
        
        this.isAuthenticated = true
        
        if (this.token) {
          localStorage.setItem('token', this.token)
        }
        
        return response.data
      } catch (error) {
        throw error
      }
    },
    
    async register(username: string, email: string, password: string) {
      try {
        const response = await api.post<AuthResponse>('/auth/register', { username, email, password })
        return response.data
      } catch (error) {
        throw error
      }
    },
    
    logout() {
      this.user = null
      this.token = null
      this.isAuthenticated = false
      localStorage.removeItem('token')
    },

    async initializeAuth() {
      const token = localStorage.getItem('token')
      if (token) {
        this.token = token
        this.isAuthenticated = true
      }
    }
  }
})
