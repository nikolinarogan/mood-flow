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
        <p class="subtitle">
          Your private space to track emotions, reflect on your journey, and discover insights for a happier, healthier you.
        </p>
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
          <h3>Effortless Mood Tracking</h3>
          <p>Log your feelings daily and watch your emotional story unfold.</p>
        </div>
        <div class="feature">
          <span class="feature-icon">📊</span>
          <h3>Insightful Analytics</h3>
          <p>Spot trends and triggers with beautiful, easy-to-read charts.</p>
        </div>
        <div class="feature">
          <span class="feature-icon">💬</span>
          <h3>Personalized Inspiration</h3>
          <p>Receive daily quotes and tips tailored to your mood journey.</p>
        </div>
        <div class="feature">
          <span class="feature-icon">🔒</span>
          <h3>Private & Secure</h3>
          <p>Your diary is encrypted and always under your control.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

.home-container {
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 50%, #f093fb 100%);
}

.welcome-card {
  background: rgba(255, 255, 255, 0.85);
  backdrop-filter: blur(24px) saturate(160%);
  padding: 40px;
  border-radius: 28px;
  box-shadow: 0 8px 32px 0 rgba(76, 70, 109, 0.18), 0 1.5px 8px 0 rgba(102, 126, 234, 0.10);
  text-align: center;
  max-width: 600px;
  width: 100%;
  transition: box-shadow 0.3s, transform 0.3s;
  animation: fadeInCard 0.8s cubic-bezier(0.4,0,0.2,1);
}


@keyframes fadeInCard {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.welcome-header {
  margin-bottom: 25px;
}

.welcome-icon {
  font-size: 48px;
  display: block;
  margin-bottom: 16px;
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

.btn-primary,
.btn-secondary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
}

.btn-primary:hover,
.btn-secondary:hover {
  background: linear-gradient(135deg, #5a67d8 0%, #6b47b6 100%);
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
  margin-bottom: 16px;
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
