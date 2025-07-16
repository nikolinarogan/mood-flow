<template>
  <div class="profile-container">
    <div class="profile-card">
      <div class="avatar-section">
        <div class="avatar">{{ avatarInitials }}</div>
      </div>
      <h2 class="profile-title">Welcome, {{ user?.username || 'User' }}!</h2>
      <div class="profile-info centered-info">
        <div class="info-row centered-row">
          <span class="info-label"><span class="info-icon">👤</span> Username</span>
        </div>
        <div class="info-value username-value">{{ user?.username }}</div>
        <div class="info-row centered-row">
          <span class="info-label"><span class="info-icon">📧</span> Email</span>
        </div>
        <div class="info-value email-value">{{ user?.email }}</div>
      </div>

      <div class="change-actions-row">
        <button @click="toggleUsernameForm" class="toggle-btn">
          {{ showUsernameForm ? 'Cancel' : 'Change Username' }}
        </button>
        <button @click="togglePasswordForm" class="toggle-btn">
          {{ showPasswordForm ? 'Cancel' : 'Change Password' }}
        </button>
      </div>
      <div class="change-section" v-if="showUsernameForm">
        <form @submit.prevent="handleChangeUsername" class="change-form">
          <input v-model="newUsername" placeholder="New username" required />
          <button type="submit">Update Username</button>
          <div v-if="usernameChangeMsg" class="change-msg">{{ usernameChangeMsg }}</div>
        </form>
      </div>
      <div class="change-section" v-if="showPasswordForm">
        <form @submit.prevent="handleChangePassword" class="change-form">
          <input v-model="currentPassword" type="password" placeholder="Current password" required />
          <input v-model="newPassword" type="password" placeholder="New password" required />
          <button type="submit">Update Password</button>
          <div v-if="passwordChangeMsg" class="change-msg">{{ passwordChangeMsg }}</div>
        </form>
      </div>

      <!-- Notification Time Section -->
      <div class="change-section notification-row">
        <div class="notification-info-label">
          <span class="notification-info-icon">⏰</span>
          <span class="notification-info-text">Select the time you want to receive your daily email reminder:</span>
        </div>
        <div class="notification-row-inner">
          <label for="notification-time">Notification Time:</label>
          <input id="notification-time" type="time" v-model="notificationTime" />
          <button @click="saveTime" :disabled="loading" class="notification-save-btn">Save</button>
        </div>
        <div v-if="notificationMsg" class="change-msg">{{ notificationMsg }}</div>
      </div>
          </div>
    </div>
  </template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
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
    showUsernameForm.value = false
  } catch (err: any) {
    usernameChangeMsg.value = err.response?.data?.message || 'Failed to update username.'
  }
}

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
    showPasswordForm.value = false
  } catch (err: any) {
    passwordChangeMsg.value = err.response?.data?.message || 'Failed to update password.'
  }
}

const notificationTime = ref('09:00')
const loading = ref(false)
const notificationMsg = ref('')

function localToUtcTimeString(localTimeStr: string) {
  const [hour, minute] = localTimeStr.split(':').map(Number)
  const now = new Date()
  now.setHours(hour, minute, 0, 0)
  const utcHour = now.getUTCHours().toString().padStart(2, '0')
  const utcMinute = now.getUTCMinutes().toString().padStart(2, '0')
  return `${utcHour}:${utcMinute}:00`
}


function utcToLocalTimeString(utcTimeStr: string) {
  const [utcHour, utcMinute] = utcTimeStr.split(':').map(Number)
  const now = new Date()
  now.setUTCHours(utcHour, utcMinute, 0, 0)
  const localHour = now.getHours().toString().padStart(2, '0')
  const localMinute = now.getMinutes().toString().padStart(2, '0')
  return `${localHour}:${localMinute}`
}

onMounted(async () => {
  loading.value = true
  try {
    const res = await api.get('/auth/notification-time')
    if (res.data && res.data.notificationTime) {
      notificationTime.value = utcToLocalTimeString(res.data.notificationTime)
    }
  } catch (err: any) {
    notificationMsg.value = 'Failed to load notification time.'
  }
  loading.value = false
})

const saveTime = async () => {
  loading.value = true
  notificationMsg.value = ''
  try {
    await api.post('/auth/notification-time', {
      notificationTime: localToUtcTimeString(notificationTime.value)
    })
    notificationMsg.value = 'Notification time updated!'
  } catch (err: any) {
    notificationMsg.value = err.response?.data?.message || 'Failed to update notification time.'
  }
  loading.value = false
}
</script>

<style scoped>
.profile-container {
  min-height: calc(100vh - 80px);
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 50%, #f093fb 100%);
}

.profile-card {
  max-width: 480px;
  width: 48%;
  margin-bottom: 2px;
  margin-top: 10px;
  background: rgba(255, 255, 255, 0.92);
  backdrop-filter: blur(28px) saturate(180%);
  border-radius: 20px;
  box-shadow: 0 8px 32px 0 rgba(76, 70, 109, 0.18), 0 1.5px 8px 0 rgba(102, 126, 234, 0.10);
  padding: 25px 32px 8px 32px;
  text-align: center;
  position: relative;
  animation: fadeInCard 0.8s cubic-bezier(0.4,0,0.2,1);
  transition: box-shadow 0.3s, transform 0.3s;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  height: auto;
  max-height: none;
}

@keyframes fadeInCard {
  from {
    opacity: 0;
    transform: translateY(30px) scale(0.98);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

.avatar-section {
  display: flex;
  justify-content: center;
}

.avatar {
  width: 80px;
  height: 80px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: #fff;
  font-size: 2.2rem;
  font-weight: 700;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 18px rgba(102, 126, 234, 0.18);
  margin-bottom: 2px;
  letter-spacing: 1px;
  border: 2px solid #fff;
  outline: 2px solid #a18cd1;
  outline-offset: 1px;
  position: relative;
}

.profile-title {
  font-size: 2rem;
  font-weight: 900;
  color: #4f46e5;
  margin-bottom: 10px;
  letter-spacing: 0.5px;
}

.profile-info {
  margin-bottom: 5px;
  background: linear-gradient(135deg, #ede9fe 0%, #f3e8ff 100%);
  border-radius: 12px;
  padding: 14px 0 10px 0;
  box-shadow: 0 2px 12px rgba(102, 126, 234, 0.07);
  border: 1.5px solid #f1f5fa;
}

.info-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
  padding: 0 10px;
  font-size: 1.2rem;
}

.info-label {
  color: #764ba2;
  font-weight: 700;
  font-size: 1.1rem;
}

.info-value {
  color: #333;
  font-size: 1.1rem;
  font-weight: 500;
}

.change-section {
  margin-top: 18px;
  background: rgba(255,255,255,0.82);
  border-radius: 12px;
  padding: 14px 12px 10px 12px;
  box-shadow: 0 2px 12px rgba(60,60,60,0.08);
  margin-bottom: 12px;
}

.toggle-btn {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: #fff;
  border: none;
  border-radius: 10px;
  padding: 12px 0;
  font-weight: 700;
  cursor: pointer;
  width: 100%;
  margin-bottom: 8px;
  font-size: 1.1rem;
  transition: background 0.2s, box-shadow 0.2s;
  letter-spacing: 0.2px;
}

.change-form {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-top: 8px;
}

.change-form input {
  padding: 14px;
  border: 2px solid #e2e8f0;
  border-radius: 8px;
  font-size: 1rem;
  transition: border-color 0.2s, box-shadow 0.2s;
}
.change-form input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.change-form button {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: #fff;
  border: none;
  border-radius: 8px;
  padding: 12px 0;
  font-weight: 700;
  cursor: pointer;
  margin-top: 4px;
  font-size: 1rem;
  transition: background 0.2s, box-shadow 0.2s;
  letter-spacing: 0.2px;
}
.change-form button:hover {
  background: linear-gradient(135deg, #5a67d8 0%, #6b47b6 100%);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.15);
}

.change-msg {
  margin-top: 6px;
  font-size: 0.88rem;
  color: #38a169;
}

.change-section label,
.change-form input,
.change-form button {
  font-size: 1.1rem;
}

.change-section label {
  display: block;
  margin-bottom: 6px;
  font-weight: 600;
  color: #764ba2;
  font-size: 0.9rem;
}

.change-section small {
  display: block;
  margin-bottom: 8px;
  color: #666;
  font-size: 0.78rem;
}

#notification-time {
  padding: 10px;
  border: 2px solid #e2e8f0;
  border-radius: 8px;
  font-size: 0.9rem;
  margin-bottom: 8px;
  width: 100%;
  box-sizing: border-box;
}

.change-section button:not(.toggle-btn) {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: #fff;
  border: none;
  border-radius: 8px;
  padding: 10px 0;
  font-weight: 700;
  cursor: pointer;
  width: 100%;
  font-size: 0.9rem;
  transition: background 0.2s, box-shadow 0.2s;
  letter-spacing: 0.2px;
}

.change-section button:not(.toggle-btn):hover {
  background: linear-gradient(135deg, #5a67d8 0%, #6b47b6 100%);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.15);
}

.change-section button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.change-actions-row {
  display: flex;
  flex-direction: row;
  gap: 12px;
  justify-content: center;
  margin-top: 18px;
  margin-bottom: 0;
}

.notification-row {
  margin-top: 18px;
  background: rgba(255,255,255,0.82);
  border-radius: 12px;
  padding: 14px 12px 10px 12px;
  box-shadow: 0 2px 12px rgba(60,60,60,0.08);
  margin-bottom: 5px;
}

.notification-row-inner {
  display: flex;
  flex-direction: row;
  align-items: center;
  gap: 5px;
  justify-content: center;
  margin-bottom: 1px;
}

.notification-row label {
  margin-bottom: 0;
  font-weight: 600;
  color: #764ba2;
  font-size: 1.1rem;
}

.notification-row input[type="time"] {
  min-width: 48px;
  max-width: 48px;
  width: 48px;
  padding: 2px 2px;
  font-size: 0.9rem;
  border-radius: 6px;
}

.notification-row button {
  padding: 4px 12px;
  font-size: 0.9rem;
  border-radius: 6px;
}

.notification-row small {
  display: block;
  margin-top: 2px;
  color: #666;
  font-size: 0.82rem;
}

.centered-info {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  border-radius: 20px;
  padding: 32px 0 18px 0;
  margin-bottom: 36px;
  box-shadow: 0 2px 12px rgba(102, 126, 234, 0.08);
}
.centered-row {
  justify-content: center;
  margin-bottom: 0;
  padding: 0;
}
.info-label {
  display: flex;
  align-items: center;
  font-size: 1.1rem;
  color: #764ba2;
  font-weight: 700;
  margin-bottom: 2px;
}
.info-icon {
  font-size: 1.3rem;
  margin-right: 8px;
}
.username-value {
  font-size: 1.4rem;
  color: #333;
  margin-bottom: 18px;
  text-align: center;
  word-break: break-all;
}
.email-value {
  font-size: 1.1rem;
  color: #333;
  margin-bottom: 18px;
  text-align: center;
  word-break: break-all;
}

.notification-info-label {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  background: linear-gradient(90deg, #e0c3fc 0%, #8ec5fc 100%);
  color: #4f46e5;
  border-radius: 8px;
  padding: 8px 14px;
  margin-bottom: 10px;
  font-size: 1rem;
  font-weight: 600;
  box-shadow: 0 2px 8px rgba(102, 126, 234, 0.08);
}
.notification-info-icon {
  font-size: 1.3rem;
}

@media (max-width: 800px) {
  .profile-card {
    padding: 12px 2vw 8px 2vw;
    width: 98%;
    min-width: unset;
    max-width: 98vw;
    max-height: 90vh;
  }
  .profile-title {
    font-size: 1.2rem;
  }
  .avatar {
    width: 48px;
    height: 48px;
    font-size: 1.2rem;
  }
  .change-section {
    padding: 8px 2vw;
  }
}

.notification-save-btn {
  padding: 2px 10px !important;
  font-size: 0.8rem !important;
  border-radius: 5px !important;
  width: auto !important;
  min-width: unset !important;
  max-width: unset !important;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: #fff;
  border: none;
  font-weight: 700;
  transition: background 0.2s, box-shadow 0.2s;
  box-shadow: 0 2px 8px rgba(102, 126, 234, 0.10);
}
.notification-save-btn:hover {
  background: linear-gradient(135deg, #5a67d8 0%, #6b47b6 100%);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.15);
}
</style>