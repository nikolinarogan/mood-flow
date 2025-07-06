<template>
  <div class="profile-container">
    <div class="profile-card">
      <div class="avatar-section">
        <div class="avatar">{{ avatarInitials }}</div>
      </div>
      <h2 class="profile-title">Welcome, {{ user?.username || 'User' }}!</h2>
      <div class="profile-info">
        <div class="info-row">
          <span class="info-label">Username:</span>
          <span class="info-value">{{ user?.username }}</span>
        </div>
        <div class="info-row">
          <span class="info-label">Email:</span>
          <span class="info-value">{{ user?.email }}</span>
        </div>
      </div>
      <div class="profile-footer">
        <p>Glad to have you on Mood Flow! 🎉</p>
      </div>

      <!-- Change Username Section -->
      <div class="change-section">
        <button @click="toggleUsernameForm" class="toggle-btn">
          {{ showUsernameForm ? 'Cancel' : 'Change Username' }}
        </button>
        <form v-if="showUsernameForm" @submit.prevent="handleChangeUsername" class="change-form">
          <input v-model="newUsername" placeholder="New username" required />
          <button type="submit">Update Username</button>
          <div v-if="usernameChangeMsg" class="change-msg">{{ usernameChangeMsg }}</div>
        </form>
      </div>

      <!-- Change Password Section -->
      <div class="change-section">
        <button @click="togglePasswordForm" class="toggle-btn">
          {{ showPasswordForm ? 'Cancel' : 'Change Password' }}
        </button>
        <form v-if="showPasswordForm" @submit.prevent="handleChangePassword" class="change-form">
          <input v-model="currentPassword" type="password" placeholder="Current password" required />
          <input v-model="newPassword" type="password" placeholder="New password" required />
          <button type="submit">Update Password</button>
          <div v-if="passwordChangeMsg" class="change-msg">{{ passwordChangeMsg }}</div>
        </form>
      </div>
          </div>
    </div>
  </template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import api from '@/services/api'

const authStore = useAuthStore()
const user = computed(() => authStore.user)

const avatarInitials = computed(() => {
  if (user.value?.username) {
    return user.value.username.slice(0, 2).toUpperCase()
  }
  return 'MF'
})

// Form visibility states
const showUsernameForm = ref(false)
const showPasswordForm = ref(false)

const toggleUsernameForm = () => {
  showUsernameForm.value = !showUsernameForm.value
  if (!showUsernameForm.value) {
    newUsername.value = ''
    usernameChangeMsg.value = ''
  }
}

const togglePasswordForm = () => {
  showPasswordForm.value = !showPasswordForm.value
  if (!showPasswordForm.value) {
    currentPassword.value = ''
    newPassword.value = ''
    passwordChangeMsg.value = ''
  }
}

// Change Username
const newUsername = ref('')
const usernameChangeMsg = ref('')

const handleChangeUsername = async () => {
  usernameChangeMsg.value = ''
  
  if (!newUsername.value.trim()) {
    usernameChangeMsg.value = 'New username is required'
    return
  }

  try {
    await api.post('/auth/change-username', {
      newUsername: newUsername.value.trim()
    })
    usernameChangeMsg.value = 'Username updated successfully!'
    if (authStore.user) authStore.user.username = newUsername.value
    newUsername.value = ''
    // Hide the form after successful update
    showUsernameForm.value = false
  } catch (err: any) {
    usernameChangeMsg.value = err.response?.data?.message || 'Failed to update username.'
  }
}

// Change Password
const currentPassword = ref('')
const newPassword = ref('')
const passwordChangeMsg = ref('')

const handleChangePassword = async () => {
  passwordChangeMsg.value = ''
  
  if (!currentPassword.value.trim()) {
    passwordChangeMsg.value = 'Current password is required'
    return
  }
  
  if (!newPassword.value.trim()) {
    passwordChangeMsg.value = 'New password is required'
    return
  }

  try {
    await api.post('/auth/change-password', {
      currentPassword: currentPassword.value,
      newPassword: newPassword.value
    })
    passwordChangeMsg.value = 'Password updated successfully!'
    currentPassword.value = ''
    newPassword.value = ''
    // Hide the form after successful update
    showPasswordForm.value = false
  } catch (err: any) {
    passwordChangeMsg.value = err.response?.data?.message || 'Failed to update password.'
  }
}
</script>

<style scoped>
.profile-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 50%, #f093fb 100%);
  padding: 32px 8px;
}

.profile-card {
  background: #fff;
  border-radius: 18px;
  box-shadow: 0 8px 32px rgba(60, 60, 60, 0.12);
  padding: 40px 32px 32px 32px;
  max-width: 420px;
  width: 100%;
  text-align: center;
  position: relative;
  animation: fadeIn 0.7s cubic-bezier(.4,0,.2,1);
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(30px); }
  to { opacity: 1; transform: translateY(0); }
}

.avatar-section {
  display: flex;
  justify-content: center;
  margin-bottom: 18px;
}

.avatar {
  width: 72px;
  height: 72px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: #fff;
  font-size: 2.2rem;
  font-weight: 700;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 2px 12px rgba(102, 126, 234, 0.15);
  margin-bottom: 8px;
  letter-spacing: 1px;
}

.profile-title {
  font-size: 1.7rem;
  font-weight: 700;
  color: #333;
  margin-bottom: 18px;
}

.profile-info {
  margin-bottom: 24px;
}

.info-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
  padding: 0 8px;
}

.info-label {
  color: #7c3aed;
  font-weight: 600;
  font-size: 1.05rem;
}

.info-value {
  color: #22223b;
  font-size: 1.05rem;
  font-weight: 500;
}

.profile-footer {
  margin-top: 18px;
  color: #764ba2;
  font-size: 1.08rem;
  font-style: italic;
}

.change-section {
  margin-top: 32px;
  background: #f7f8fa;
  border-radius: 10px;
  padding: 20px 16px;
  box-shadow: 0 2px 12px rgba(60,60,60,0.06);
}

.toggle-btn {
  background: linear-gradient(90deg, #7c3aed 0%, #4f46e5 100%);
  color: #fff;
  border: none;
  border-radius: 6px;
  padding: 10px 0;
  font-weight: 600;
  cursor: pointer;
  width: 100%;
  margin-bottom: 12px;
}

.change-form {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.change-form input {
  padding: 10px;
  border: 1.5px solid #e0e3e8;
  border-radius: 6px;
  font-size: 1rem;
}

.change-form button {
  background: linear-gradient(90deg, #7c3aed 0%, #4f46e5 100%);
  color: #fff;
  border: none;
  border-radius: 6px;
  padding: 10px 0;
  font-weight: 600;
  cursor: pointer;
  margin-top: 4px;
}

.change-msg {
  margin-top: 6px;
  font-size: 0.98rem;
  color: #38a169;
}

@media (max-width: 600px) {
  .profile-card {
    padding: 24px 8px 18px 8px;
  }
  .profile-title {
    font-size: 1.2rem;
  }
  .avatar {
    width: 56px;
    height: 56px;
    font-size: 1.3rem;
  }
}
</style>