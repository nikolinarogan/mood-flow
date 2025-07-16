import { defineStore } from 'pinia'
import api from '@/services/api'
import { getCurrentUser } from '@/services/api'
import type { User, AuthResponse} from '@/types/auth'

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
          id: response.data.id,
          username: response.data.username,
          email: response.data.email,
          isEmailVerified: response.data.isEmailVerified,
          createdDate: response.data.createdDate,
          lastLoginAt: response.data.lastLoginAt
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
        try {
          const res = await getCurrentUser();
          console.log('getCurrentUser response:', res);
          if (res.data) {
            this.user = {
              id: res.data.id,
              username: res.data.username,
              email: res.data.email,
              isEmailVerified: res.data.isEmailVerified,
              createdDate: res.data.createdDate,
              lastLoginAt: res.data.lastLoginAt
            }
          }
        } catch (e) {
          // Only clear token if error is 401 or 403
          const err = e as any;
          if (err.response && (err.response.status === 401 || err.response.status === 403)) {
            this.user = null;
            this.isAuthenticated = false;
            this.token = null;
            localStorage.removeItem('token');
          } else {
            // For other errors, keep the token and just log the error
            console.error('Could not fetch user info, but token is kept:', e);
          }
        }
      }
    }
  }
})
