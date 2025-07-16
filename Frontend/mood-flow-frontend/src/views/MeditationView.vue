<template>
  <div class="meditation-container">
    <div class="meditation-header">
      <h1>Meditation & Breathing</h1>
      <p class="subtitle">Find peace and calm with guided exercises</p>
    </div>

    <div v-if="loading" class="loading-container">
      <div class="loading-spinner"></div>
      <p>Loading meditation exercises...</p>
    </div>

    <div v-else class="exercises-content">
      <div class="exercise-grid">
        <div v-for="exercise in exercises" :key="exercise.id" class="exercise-card">
          <h3 class="exercise-title">{{ exercise.title }}</h3>
          <div class="video-wrapper">
            <iframe
              :src="exercise.videoUrl"
              frameborder="0"
              allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
              allowfullscreen
              title="Meditation Exercise"
            ></iframe>
          </div>
          <p class="exercise-description">{{ exercise.description }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getMeditationExercises } from '@/services/api'

interface MeditationExercise {
  id: number
  title: string
  videoUrl: string
  description: string
}

const loading = ref(true)
const exercises = ref<MeditationExercise[]>([])

const fetchExercises = async () => {
  try {
    const response = await getMeditationExercises()
    exercises.value = response.data
    console.log('Meditation exercises:', exercises.value)
  } catch (error) {
    console.error('Error fetching exercises:', error)
    exercises.value = []
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  await fetchExercises()
})
</script>

<style scoped>
.meditation-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
  min-height: calc(100vh - 70px);
  background: linear-gradient(135deg, #667eea 0%, #764ba2 50%, #f093fb 100%);
}

.meditation-header {
  text-align: center;
  margin-bottom: 40px;
  color: white;
}

.meditation-header h1 {
  font-size: 2.5rem;
  font-weight: 700;
  margin-bottom: 10px;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.subtitle {
  font-size: 1.1rem;
  opacity: 0.9;
}

.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 400px;
  color: white;
}

.loading-spinner {
  width: 50px;
  height: 50px;
  border: 4px solid rgba(255, 255, 255, 0.3);
  border-top: 4px solid white;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 20px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.exercises-content {
  margin-top: 20px;
}

.exercise-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: 30px;
}

.exercise-card {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(20px);
  border-radius: 20px;
  padding: 24px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease;
}

.exercise-card:hover {
  transform: translateY(-5px);
}

.exercise-title {
  color: #2c3e50;
  font-size: 1.4rem;
  font-weight: 600;
  margin-bottom: 16px;
  text-align: center;
}

.video-wrapper {
  position: relative;
  width: 100%;
  padding-bottom: 56.25%; 
  margin-bottom: 16px;
  border-radius: 12px;
  overflow: hidden;
}

.video-wrapper iframe {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  border-radius: 12px;
}

.exercise-description {
  color: #666;
  font-size: 1rem;
  line-height: 1.6;
  margin: 0;
  text-align: center;
}

@media (max-width: 768px) {
  .meditation-container {
    padding: 10px;
  }
  
  .meditation-header h1 {
    font-size: 2rem;
  }
  
  .exercise-grid {
    grid-template-columns: 1fr;
    gap: 20px;
  }
  
  .exercise-card {
    padding: 20px;
  }
  
  .exercise-title {
    font-size: 1.2rem;
  }
}

@media (max-width: 480px) {
  .exercise-card {
    padding: 16px;
  }
}
</style> 