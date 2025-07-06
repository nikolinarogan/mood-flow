<script setup lang="ts">
import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const isAuthenticated = computed(() => authStore.isAuthenticated)
</script>

<template>
  <div class="home-container">
    <div class="welcome-card">
      <div class="welcome-header">
        <span class="welcome-icon">🌊</span>
        <h1>Welcome to Mood Flow</h1>
        <p class="subtitle">Track your mood and emotions with our beautiful diary</p>
      </div>
      
      <div v-if="!isAuthenticated" class="auth-buttons">
        <router-link to="/login" class="btn btn-primary">
          <span class="btn-icon">🔐</span>
          Sign In
        </router-link>
        <router-link to="/register" class="btn btn-secondary">
          <span class="btn-icon">✨</span>
          Create Account
        </router-link>
      </div>
      
      <div v-else class="welcome-actions">
        <div class="welcome-message">
          <h2>Welcome back, {{ authStore.user?.username || 'User' }}!</h2>
          <p>Ready to track your mood today?</p>
        </div>
        
        <div class="action-buttons">
          <router-link to="/diary" class="btn btn-primary">
            <span class="btn-icon">📖</span>
            Open Diary
          </router-link>
          <router-link to="/profile" class="btn btn-secondary">
            <span class="btn-icon">👤</span>
            View Profile
          </router-link>
        </div>
      </div>
      
      <div class="features">
        <div class="feature">
          <span class="feature-icon">📅</span>
          <h3>Daily Tracking</h3>
          <p>Record your mood every day with our intuitive calendar</p>
        </div>
        <div class="feature">
          <span class="feature-icon">📊</span>
          <h3>Mood Insights</h3>
          <p>Discover patterns and trends in your emotional well-being</p>
        </div>
        <div class="feature">
          <span class="feature-icon">🔒</span>
          <h3>Private & Secure</h3>
          <p>Your personal diary is protected with secure authentication</p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.home-container {
  min-height: calc(100vh - 70px);
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 50%, #f093fb 100%);
  padding: 20px;
}

.welcome-card {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(20px);
  padding: 40px;
  border-radius: 20px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
  text-align: center;
  max-width: 600px;
  width: 100%;
}

.welcome-header {
  margin-bottom: 40px;
}

.welcome-icon {
  font-size: 48px;
  display: block;
  margin-bottom: 16px;
  animation: float 3s ease-in-out infinite;
}

@keyframes float {
  0%, 100% { transform: translateY(0px); }
  50% { transform: translateY(-10px); }
}

h1 {
  color: #333;
  margin-bottom: 16px;
  font-size: 36px;
  font-weight: 700;
}

.subtitle {
  color: #666;
  margin-bottom: 32px;
  font-size: 18px;
}

.auth-buttons,
.action-buttons {
  display: flex;
  gap: 16px;
  justify-content: center;
  flex-wrap: wrap;
  margin-bottom: 40px;
}

.btn {
  padding: 16px 24px;
  border-radius: 12px;
  text-decoration: none;
  font-weight: 600;
  font-size: 16px;
  transition: all 0.3s ease;
  display: inline-flex;
  align-items: center;
  gap: 8px;
}

.btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.15);
}

.btn-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.btn-secondary {
  background: white;
  color: #667eea;
  border: 2px solid #667eea;
}

.btn-secondary:hover {
  background: #667eea;
  color: white;
}

.btn-icon {
  font-size: 18px;
}

.welcome-actions {
  margin-bottom: 40px;
}

.welcome-message h2 {
  color: #333;
  font-size: 24px;
  margin-bottom: 8px;
}

.welcome-message p {
  color: #666;
  font-size: 16px;
  margin-bottom: 24px;
}

.features {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 24px;
  margin-top: 40px;
}

.feature {
  padding: 20px;
  background: rgba(255, 255, 255, 0.8);
  border-radius: 16px;
  border: 1px solid rgba(102, 126, 234, 0.1);
}

.feature-icon {
  font-size: 32px;
  display: block;
  margin-bottom: 12px;
}

.feature h3 {
  color: #333;
  font-size: 18px;
  margin-bottom: 8px;
  font-weight: 600;
}

.feature p {
  color: #666;
  font-size: 14px;
  line-height: 1.5;
}

@media (max-width: 768px) {
  .welcome-card {
    padding: 30px 20px;
  }
  
  h1 {
    font-size: 28px;
  }
  
  .features {
    grid-template-columns: 1fr;
  }
}
</style>
