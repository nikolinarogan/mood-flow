import axios from 'axios'
import type { ChangeUsernameRequest, ChangePasswordRequest } from '@/types/auth';

const api = axios.create({
    baseURL: 'https://localhost:7096/api',
    headers:{
        'Content-Type': 'application/json'
    }
})

api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token')
    if(token){
        config.headers.Authorization = `Bearer ${token}`
    }
    return config
})
export function toggleFavouriteQuote(id: number) {
  return api.post(`/quote/favourite/${id}`);
}
export const authAPI = {
    login: (data: { email: string; password: string }) => api.post('/auth/login', data),
    register: (data: { email: string; password: string; username: string }) => api.post('/auth/register', data),
    changeUsername: (data: ChangeUsernameRequest) => api.post('/auth/change-username', data),
    changePassword: (data: ChangePasswordRequest) => api.post('/auth/change-password', data)
}
export async function analyzeSentiment(note: string) {
  const res = await fetch('/api/sentiment/analyze', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ text: note })
  })
  if (!res.ok) return null
  return await res.json()
}

export function updateDiaryEntry(id: number, data: { emotion: string; grade: number; note: string; }) {
  return api.put(`/diaryitem/update/${id}`, data);
}

export function deleteDiaryEntry(id: number) {
  return api.delete(`/diaryitem/delete/${id}`);
}

export function getUserFavourites() {
  return api.get('/quote/favourites');
}

export function getAnalyticsSummary() {
  return api.get('/analytics/summary');
}

export default api