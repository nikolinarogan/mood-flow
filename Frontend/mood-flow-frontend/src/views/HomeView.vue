<script setup lang="ts">
// Component logic here
</script>

<template>
  <div class="home-container">
    <div class="welcome-card">
      <h1>Welcome to Mood Flow</h1>
      <p class="subtitle">Track your mood and emotions</p>
      
      <div class="auth-buttons">
        <router-link to="/login" class="btn btn-primary">
          Sign In
        </router-link>
        <router-link to="/register" class="btn btn-secondary">
          Create Account
        </router-link>
      </div>
    </div>
  </div>
</template>

<style scoped>
.home-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
}

.welcome-card {
  background: white;
  padding: 40px;
  border-radius: 12px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.1);
  text-align: center;
  max-width: 500px;
  width: 100%;
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

.auth-buttons {
  display: flex;
  gap: 16px;
  justify-content: center;
  flex-wrap: wrap;
}

.btn {
  padding: 12px 24px;
  border-radius: 8px;
  text-decoration: none;
  font-weight: 600;
  font-size: 16px;
  transition: transform 0.2s ease;
  display: inline-block;
}

.btn:hover {
  transform: translateY(-2px);
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
</style>
