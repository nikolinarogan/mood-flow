<template>
  <nav class="navbar">
    <div class="nav-container">
      <div class="nav-brand">
        <router-link to="/" class="brand-link">
          <span class="brand-icon">🌊</span>
          <span class="brand-text">Mood Flow</span>
        </router-link>
      </div>
      
      <div class="nav-menu">
        <router-link to="/" class="nav-link" active-class="active">
          <span class="nav-icon">🏠</span>
          <span class="nav-text">Home</span>
        </router-link>
        
        <router-link v-if="isAuthenticated" to="/diary" class="nav-link" active-class="active">
          <span class="nav-icon">📖</span>
          <span class="nav-text">Diary</span>
        </router-link>
        
        <router-link v-if="isAuthenticated" to="/profile" class="nav-link" active-class="active">
          <span class="nav-icon">👤</span>
          <span class="nav-text">Profile</span>
        </router-link>
        
        <router-link v-if="!isAuthenticated" to="/login" class="nav-link" active-class="active">
          <span class="nav-icon">🔐</span>
          <span class="nav-text">Login</span>
        </router-link>
        
        <router-link v-if="!isAuthenticated" to="/register" class="nav-link" active-class="active">
          <span class="nav-icon">✨</span>
          <span class="nav-text">Register</span>
        </router-link>
        <router-link to="/quotes" class="nav-link quotes-link" active-class="active">
          <span class="nav-icon">💬</span>
          <span class="nav-text">Quotes</span>
        </router-link>
      </div>
      
      <div v-if="isAuthenticated" class="nav-actions">
        <button @click="handleLogout" class="logout-btn">
          <span class="logout-icon">🚪</span>
          <span class="logout-text">Logout</span>
        </button>
      </div>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const isAuthenticated = computed(() => authStore.isAuthenticated)

const handleLogout = () => {
  authStore.logout()
  router.push('/')
}
</script>

<style scoped>
.navbar {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(20px);
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
  position: sticky;
  top: 0;
  z-index: 1000;
  box-shadow: 0 2px 20px rgba(0, 0, 0, 0.1);
}

.nav-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 70px;
}

.nav-brand {
  display: flex;
  align-items: center;
}

.brand-link {
  display: flex;
  align-items: center;
  text-decoration: none;
  color: #333;
  font-weight: 700;
  font-size: 24px;
  transition: transform 0.2s ease;
}

.brand-link:hover {
  transform: scale(1.05);
}

.brand-icon {
  font-size: 28px;
  margin-right: 8px;
  animation: wave 2s ease-in-out infinite;
}

@keyframes wave {
  0%, 100% { transform: rotate(0deg); }
  25% { transform: rotate(10deg); }
  75% { transform: rotate(-10deg); }
}

.brand-text {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.nav-menu {
  display: flex;
  align-items: center;
  gap: 8px;
}

.nav-link {
  display: flex;
  align-items: center;
  padding: 12px 16px;
  text-decoration: none;
  color: #666;
  border-radius: 12px;
  transition: all 0.3s ease;
  font-weight: 500;
  position: relative;
  overflow: hidden;
}

.nav-link::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  transition: left 0.3s ease;
  z-index: -1;
}

.nav-link:hover::before,
.nav-link.active::before {
  left: 0;
}

.nav-link:hover,
.nav-link.active {
  color: white;
  transform: translateY(-2px);
}

.nav-icon {
  font-size: 18px;
  margin-right: 8px;
}

.nav-text {
  font-size: 14px;
}

.nav-actions {
  display: flex;
  align-items: center;
}

.logout-btn {
  display: flex;
  align-items: center;
  padding: 10px 16px;
  background: linear-gradient(135deg, #ff6b6b 0%, #ee5a52 100%);
  color: white;
  border: none;
  border-radius: 12px;
  cursor: pointer;
  font-weight: 500;
  font-size: 14px;
  transition: all 0.3s ease;
}

.logout-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(255, 107, 107, 0.3);
}

.logout-icon {
  font-size: 16px;
  margin-right: 6px;
}

.logout-text {
  font-size: 14px;
}

.quotes-link.active {
  background: linear-gradient(135deg, #ffd700 0%, #43e97b 100%);
  color: #333 !important;
  box-shadow: 0 2px 12px rgba(255, 215, 0, 0.18);
  transform: scale(1.08);
}

@media (max-width: 768px) {
  .nav-container {
    padding: 0 16px;
  }
  
  .nav-text,
  .logout-text {
    display: none;
  }
  
  .nav-link {
    padding: 12px;
  }
  
  .logout-btn {
    padding: 10px 12px;
  }
}
</style> 