<template>
  <div class="favourites-container">
    <div class="favourites-header">
      <h1>My Favourites</h1>
      <p class="subtitle">Your personal collection of inspiring quotes and meditation exercises</p>
    </div>
    
    <div v-if="loading" class="loading">
      <div class="loading-spinner"></div>
      <p>Loading your favourites...</p>
    </div>
    
    <div v-else class="favourites-content">
      <!-- Quotes Section -->
      <div class="section">
        <h2 class="section-title">💭 Favourite Quotes</h2>
        
        <div v-if="favouriteQuotes.length === 0" class="empty-state">
          <div class="empty-icon">💭</div>
          <h3>No Favourite Quotes Yet</h3>
          <p>Start exploring quotes and mark your favourites with a heart!</p>
          <router-link to="/quotes" class="btn btn-primary">
            <span class="btn-icon">💫</span>
            Explore Quotes
          </router-link>
        </div>
        
        <div v-else class="favourites-list">
          <div v-for="quote in favouriteQuotes" :key="quote.id" class="quote-card">
            <blockquote>
              "{{ quote.quoteText }}"
              <footer>
                - {{ quote.author }}
                <span class="date">{{ formatDate(quote.date) }}</span>
                <span
                  class="heart favourite"
                  @click="toggleQuoteFavourite(quote)"
                  title="Remove from favourites"
                >
                  ❤️
                </span>
              </footer>
            </blockquote>
          </div>
        </div>
      </div>

      <!-- Meditation Exercises Section -->
      <div class="section">
        <h2 class="section-title">🧘 Favourite Meditation Exercises</h2>
        
        <div v-if="favouriteExercises.length === 0" class="empty-state">
          <div class="empty-icon">🧘</div>
          <h3>No Favourite Exercises Yet</h3>
          <p>Start exploring meditation exercises and mark your favourites!</p>
          <router-link to="/meditation" class="btn btn-primary">
            <span class="btn-icon">🧘</span>
            Explore Meditation
          </router-link>
        </div>
        
        <div v-else class="exercises-grid">
          <div v-for="exercise in favouriteExercises" :key="exercise.id" class="exercise-card">
            <div class="exercise-header">
              <h3 class="exercise-title">{{ exercise.title }}</h3>
              <button 
                @click="toggleExerciseFavourite(exercise)"
                class="favourite-btn favourite"
                title="Remove from favourites"
              >
                ❤️
              </button>
            </div>
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
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import api from '@/services/api'
import { getMeditationFavourites, toggleMeditationFavourite } from '@/services/api'

interface MeditationExercise {
  id: number
  title: string
  videoUrl: string
  description: string
}

const favouriteQuotes = ref<any[]>([])
const favouriteExercises = ref<MeditationExercise[]>([])
const loading = ref(true)

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleDateString()
}

onMounted(async () => {
  try {
    loading.value = true
    await Promise.all([
      loadFavouriteQuotes(),
      loadFavouriteExercises()
    ])
  } catch (error) {
    console.error('Failed to load favourites:', error)
  } finally {
    loading.value = false
  }
})

async function loadFavouriteQuotes() {
  try {
    const response = await api.get('/quote/favourites')
    favouriteQuotes.value = response.data
  } catch (error) {
    console.error('Failed to load favourite quotes:', error)
  }
}

async function loadFavouriteExercises() {
  try {
    const response = await getMeditationFavourites()
    favouriteExercises.value = response.data
  } catch (error) {
    console.error('Failed to load favourite exercises:', error)
  }
}

async function toggleQuoteFavourite(quote: any) {
  try {
    const res = await api.post(`/quote/favourite/${quote.id}`)
    if (!res.data.isFavourite) {
      // Remove from the list if unfavourited
      favouriteQuotes.value = favouriteQuotes.value.filter(q => q.id !== quote.id)
    }
  } catch (error) {
    console.error('Failed to toggle quote favourite:', error)
  }
}

async function toggleExerciseFavourite(exercise: MeditationExercise) {
  try {
    const response = await toggleMeditationFavourite(exercise.id)
    if (!response.data.isFavourite) {
      // Remove from the list if unfavourited
      favouriteExercises.value = favouriteExercises.value.filter(e => e.id !== exercise.id)
    }
  } catch (error) {
    console.error('Failed to toggle exercise favourite:', error)
  }
}
</script>

<style scoped>
.favourites-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 40px 20px;
}

.favourites-header {
  text-align: center;
  margin-bottom: 40px;
}

.favourites-header h1 {
  color: #333;
  font-size: 2.5rem;
  margin-bottom: 8px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.subtitle {
  color: #666;
  font-size: 1.1rem;
}

.loading {
  text-align: center;
  padding: 60px 20px;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #667eea;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 20px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.favourites-content {
  display: flex;
  flex-direction: column;
  gap: 40px;
}

.section {
  background: rgba(255, 255, 255, 0.8);
  border-radius: 16px;
  padding: 24px;
  border: 2px solid rgba(102, 126, 234, 0.1);
}

.section-title {
  color: #333;
  font-size: 1.8rem;
  margin-bottom: 24px;
  text-align: center;
  font-weight: 600;
}

.empty-state {
  text-align: center;
  padding: 60px 20px;
  background: rgba(255, 255, 255, 0.8);
  border-radius: 16px;
  border: 2px dashed #ddd;
}

.empty-icon {
  font-size: 3rem;
  margin-bottom: 20px;
  opacity: 0.6;
}

.empty-state h3 {
  color: #333;
  margin-bottom: 12px;
  font-size: 1.3rem;
}

.empty-state p {
  color: #666;
  margin-bottom: 30px;
  font-size: 1rem;
}

.btn {
  padding: 12px 24px;
  border-radius: 8px;
  text-decoration: none;
  font-weight: 600;
  font-size: 16px;
  transition: all 0.3s ease;
  display: inline-flex;
  align-items: center;
  gap: 8px;
}

.btn-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.15);
}

.btn-icon {
  font-size: 18px;
}

.favourites-list {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.quote-card {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-radius: 16px;
  padding: 24px;
  box-shadow: 0 4px 20px rgba(102, 126, 234, 0.2);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.quote-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 30px rgba(102, 126, 234, 0.3);
}

.quote-card blockquote {
  font-size: 1.3rem;
  font-style: italic;
  margin: 0;
  line-height: 1.6;
}

.quote-card footer {
  margin-top: 16px;
  font-size: 1rem;
  text-align: right;
  opacity: 0.9;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.date {
  font-size: 0.9rem;
  opacity: 0.8;
}

.heart {
  cursor: pointer;
  font-size: 1.4em;
  transition: transform 0.2s ease;
  user-select: none;
}

.heart:hover {
  transform: scale(1.2);
}

.heart.favourite {
  color: #ff6b6b;
  animation: pulse 0.3s ease;
}

@keyframes pulse {
  0% { transform: scale(1); }
  50% { transform: scale(1.3); }
  100% { transform: scale(1); }
}

.exercises-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  gap: 24px;
}

.exercise-card {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(20px);
  border-radius: 16px;
  padding: 20px;
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease;
}

.exercise-card:hover {
  transform: translateY(-4px);
}

.exercise-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.exercise-title {
  color: #2c3e50;
  font-size: 1.2rem;
  font-weight: 600;
  margin: 0;
  flex: 1;
}

.favourite-btn {
  background: none;
  border: none;
  font-size: 1.3rem;
  cursor: pointer;
  transition: transform 0.2s ease;
  padding: 6px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.favourite-btn:hover {
  transform: scale(1.2);
}

.favourite-btn.favourite {
  animation: pulse 0.3s ease;
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
  font-size: 0.9rem;
  line-height: 1.5;
  margin: 0;
  text-align: center;
}

@media (max-width: 768px) {
  .favourites-container {
    padding: 20px 15px;
  }
  
  .favourites-header h1 {
    font-size: 2rem;
  }
  
  .quote-card blockquote {
    font-size: 1.1rem;
  }
  
  .exercises-grid {
    grid-template-columns: 1fr;
  }
  
  .section-title {
    font-size: 1.5rem;
  }
}
</style> 