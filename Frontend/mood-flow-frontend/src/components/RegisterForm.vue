<template>
  <div class="register-container">
    <div class="register-card">
      <div class="card-header">
        <div class="logo">
          <span class="logo-icon">✨</span>
        </div>
        <h2>Join Mood Flow</h2>
        <p class="subtitle">Create your account and start tracking</p>
      </div>
      
      <form @submit.prevent="handleRegister" class="register-form">
        <div class="form-group">
          <label for="username">Username</label>
          <div class="input-wrapper">
            <span class="input-icon">👤</span>
            <input 
              type="text" 
              id="username" 
              v-model="username" 
              required
              placeholder="Choose a username"
              :disabled="loading"
            />
          </div>
        </div>
        
        <div class="form-group">
          <label for="email">Email Address</label>
          <div class="input-wrapper">
            <span class="input-icon">📧</span>
            <input 
              type="email" 
              id="email" 
              v-model="email" 
              required
              placeholder="Enter your email"
              :disabled="loading"
            />
          </div>
        </div>
        
        <div class="form-group">
          <label for="password">Password</label>
          <div class="input-wrapper">
            <span class="input-icon">🔒</span>
            <input 
              type="password" 
              id="password" 
              v-model="password" 
              required
              placeholder="Create a strong password"
              :disabled="loading"
            />
          </div>
          <div v-if="password" class="password-strength" :class="passwordStrength.toLowerCase()">
            Strength: {{ passwordStrength }}
          </div>
        </div>
        
        <button type="submit" :disabled="loading" class="register-btn">
          <span v-if="loading" class="loading-spinner"></span>
          <span v-else class="btn-text">Create Account</span>
        </button>
      </form>
      
      <div v-if="error" class="error-message">
        <span class="error-icon">⚠️</span>
        {{ error }}
      </div>
      
      <div v-if="success" class="success-message">
        <span class="success-icon">✅</span>
        {{ success }}
      </div>
      
      <div class="login-link">
        <span>Already have an account?</span>
        <router-link to="/login" class="link">Sign in here</router-link>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const username = ref('')
const email = ref('')
const password = ref('')
const loading = ref(false)
const error = ref('')
const success = ref('')

const passwordStrength = computed(() => {
  if (!password.value) return ''
  if (password.value.length < 6) return 'Weak'
  if (password.value.match(/[A-Z]/) && password.value.match(/[0-9]/) && password.value.length >= 8) return 'Strong'
  return 'Medium'
})

const handleRegister = async () => {
  try {
    loading.value = true
    error.value = ''
    success.value = ''
    
    await authStore.register(username.value, email.value, password.value)
    success.value = 'Account created successfully! Please check your email for verification.'
    
    username.value = ''
    email.value = ''
    password.value = ''
    
    setTimeout(() => {
      router.push('/')
    }, 3000)
  } catch (err: any) {
    error.value = err.response?.data?.message || 'Registration failed. Please try again.'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.register-container {
  min-height: calc(100vh - 72px);
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 50%, #f093fb 100%);
  padding: 0;
  position: relative;
  overflow: hidden;
}

.register-container::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"><defs><pattern id="grain" width="100" height="100" patternUnits="userSpaceOnUse"><circle cx="50" cy="50" r="1" fill="white" opacity="0.1"/></pattern></defs><rect width="100" height="100" fill="url(%23grain)"/></svg>');
  pointer-events: none;
}

.register-card {
  background: rgba(255, 255, 255, 0.90);
  backdrop-filter: blur(24px) saturate(160%);
  padding: 20px 16px;
  border-radius: 28px;
  box-shadow: 0 8px 32px 0 rgba(76, 70, 109, 0.15), 0 1.5px 8px 0 rgba(102, 126, 234, 0.08);
  max-width: 600px;
  margin: 0 auto;
  width: 40%;
  position: relative;
  z-index: 1;
  animation: slideUp 0.6s ease-out;
  border: 1.5px solid #e0e7ff;
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.card-header {
  text-align: center;
  margin-bottom: 18px;
}

.logo {
  margin-bottom: 20px;
}

.logo-icon {
  font-size: 44px;
  display: block;
}

h2 {
  color: #2d3748;
  margin-bottom: 8px;
  font-size: 28px;
  font-weight: 700;
}

.subtitle {
  color: #718096;
  font-size: 16px;
  margin: 0;
}

.register-form {
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

label {
  font-weight: 600;
  color: #4a5568;
  font-size: 14px;
}

.input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.input-icon {
  position: absolute;
  left: 16px;
  font-size: 18px;
  color: #a0aec0;
  z-index: 1;
}

input {
  width: 100%;
  padding: 16px 16px 16px 48px;
  border: 2px solid #e2e8f0;
  border-radius: 12px;
  font-size: 16px;
  transition: all 0.3s ease;
  background: white;
}

input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

input:disabled {
  background-color: #f7fafc;
  cursor: not-allowed;
}

.register-btn {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 16px;
  border: none;
  border-radius: 12px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}

.register-btn::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.2), transparent);
  transition: left 0.5s;
}

.register-btn:hover:not(:disabled)::before {
  left: 100%;
}

.register-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 10px 25px rgba(102, 126, 234, 0.4);
}

.register-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.loading-spinner {
  width: 20px;
  height: 20px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top: 2px solid white;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.btn-text {
  font-weight: 600;
}

.error-message {
  background: #fed7d7;
  color: #c53030;
  padding: 12px 16px;
  border-radius: 8px;
  margin-top: 16px;
  text-align: center;
  font-size: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  animation: shake 0.5s ease-in-out;
}

@keyframes shake {
  0%, 100% { transform: translateX(0); }
  25% { transform: translateX(-5px); }
  75% { transform: translateX(5px); }
}

.error-icon {
  font-size: 16px;
}

.success-message {
  background: #c6f6d5;
  color: #22543d;
  padding: 12px 16px;
  border-radius: 8px;
  margin-top: 16px;
  text-align: center;
  font-size: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}

.success-icon {
  font-size: 16px;
}

.login-link {
  text-align: center;
  margin-top: 24px;
  color: #718096;
  font-size: 14px;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.link {
  color: #667eea;
  text-decoration: none;
  font-weight: 600;
  transition: color 0.3s ease;
}

.link:hover {
  color: #5a67d8;
  text-decoration: underline;
}

.password-strength {
  font-size: 13px;
  margin-top: 4px;
}
.password-strength.weak { color: #e53e3e; }
.password-strength.medium { color: #d69e2e; }
.password-strength.strong { color: #38a169; }

@media (max-width: 480px) {
  .register-card {
    padding: 30px 20px;
  }
  
  h2 {
    font-size: 24px;
  }
  
  .logo-icon {
    font-size: 40px;
  }
}
</style>