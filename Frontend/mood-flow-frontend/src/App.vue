<script setup lang="ts">
import { RouterView } from 'vue-router'
import { onMounted } from 'vue'
import { useAuthStore } from './stores/auth'
import NavBar from './components/NavBar.vue'

const authStore = useAuthStore()

onMounted(() => {
  authStore.initializeAuth()
})
</script>

<template>
  <div class="app">
    <NavBar />
    <main class="main-content">
      <RouterView />
    </main>
  </div>
</template>

<style>
html, body, #app {
  width: 100vw;
  height: 100vh;
  margin: 0;
  padding: 0;
  overflow-x: hidden;
}
*, *::before, *::after {
  box-sizing: border-box;
  max-width: 100vw !important;
}
.app {
  height: 100vh;
  display: flex;
  flex-direction: column;
}
.main-content {
  flex: 1 1 auto;
  overflow-y: auto;
  width: 100%;
}
</style>

