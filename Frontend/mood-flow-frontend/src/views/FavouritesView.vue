<template>
  <div class="favourites-container">
    <div class="favourites-header">
      <h1>My Favourite Quotes</h1>
      <p class="subtitle">Your personal collection of inspiring quotes</p>
    </div>
    
    <div v-if="loading" class="loading">
      <div class="loading-spinner"></div>
      <p>Loading your favourites...</p>
    </div>
    
    <div v-else-if="favouriteQuotes.length === 0" class="empty-state">
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
              @click="toggleFavourite(quote)"
              title="Remove from favourites"
            >
              ❤️
            </span>
          </footer>
        </blockquote>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import api from '@/services/api'

const favouriteQuotes = ref<any[]>([])
const loading = ref(true)

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleDateString()
}

onMounted(async () => {
  try {
    loading.value = true
    const response = await api.get('/quote/favourites')
    favouriteQuotes.value = response.data
  } catch (error) {
    console.error('Failed to load favourite quotes:', error)
  } finally {
    loading.value = false
  }
})

async function toggleFavourite(quote: any) {
  try {
    const res = await api.post(`/quote/favourite/${quote.id}`)
    if (!res.data.isFavourite) {
      // Remove from the list if unfavourited
      favouriteQuotes.value = favouriteQuotes.value.filter(q => q.id !== quote.id)
    }
  } catch (error) {
    console.error('Failed to toggle favourite:', error)
  }
}
</script>

<style scoped>
.favourites-container {
  max-width: 800px;
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

.empty-state {
  text-align: center;
  padding: 80px 20px;
  background: rgba(255, 255, 255, 0.8);
  border-radius: 16px;
  border: 2px dashed #ddd;
}

.empty-icon {
  font-size: 4rem;
  margin-bottom: 20px;
  opacity: 0.6;
}

.empty-state h3 {
  color: #333;
  margin-bottom: 12px;
  font-size: 1.5rem;
}

.empty-state p {
  color: #666;
  margin-bottom: 30px;
  font-size: 1.1rem;
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
}
</style> 