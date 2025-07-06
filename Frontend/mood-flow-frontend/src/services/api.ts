import axios from 'axios'

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

export const authAPI = {
    login: (data: { email: string; password: string }) => api.post('/auth/login', data),
    register: (data: { email: string; password: string; username: string }) => api.post('/auth/register', data),
    changeUsername: (data: { newUsername: string; password: string }) => api.post('/auth/change-username', data)
}

export default api