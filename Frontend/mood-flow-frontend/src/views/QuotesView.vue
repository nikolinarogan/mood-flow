<template>
  <div class="quotes-container">
    <div class="quotes-header">
      <h1>Daily Quotes</h1>
      <router-link to="/favourites" class="favourites-link">
        <span class="favourites-icon">❤️</span>
        <span class="favourites-text">My Favourites</span>
      </router-link>
    </div>
    <div v-if="allQuotes.length" class="quotes-list">
      <div v-for="quote in allQuotes" :key="quote.id" 
           :class="['quote-card', { 'today-quote': quote.isToday }]">
        <div v-if="quote.isToday" class="today-badge">Today's Quote</div>
        <blockquote>
          "{{ quote.quoteText }}"
          <footer>
            - {{ quote.author }}
            <span class="date">{{ formatDate(quote.date) }}</span>
            <span
              class="heart"
              :class="{ favourite: quote.isFavourite }"
              @click="toggleFavourite(quote)"
              :title="quote.isFavourite ? 'Remove from favourites' : 'Mark as favourite'"
            >
              {{ quote.isFavourite ? '❤️' : '🤍' }}
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
import { getUserFavourites } from '@/services/api'

const allQuotes = ref<any[]>([])
const favouriteIds = ref<number[]>([])

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleDateString()
}

onMounted(async () => {
  try {
    // Fetch user's favourite quote IDs
    const favouritesRes = await getUserFavourites()
    favouriteIds.value = favouritesRes.data.map((q: any) => q.id)

    // Fetch all quotes
    const allRes = await api.get('/quote/all')
    const today = new Date().toISOString().split('T')[0] // Get YYYY-MM-DD format
    
    // Process all quotes and mark today's quote
    allQuotes.value = allRes.data.map((quote: any) => ({
      ...quote,
      isToday: quote.date.split('T')[0] === today,
      isFavourite: favouriteIds.value.includes(quote.id)
    }))
    
    // Sort: today's quote first, then by date descending
    allQuotes.value.sort((a, b) => {
      if (a.isToday && !b.isToday) return -1
      if (!a.isToday && b.isToday) return 1
      return new Date(b.date).getTime() - new Date(a.date).getTime()
    })
  } catch (error) {
    console.error('Failed to load quotes:', error)
  }
})

async function toggleFavourite(quote: any) {
  try {
    const res = await api.post(`/quote/favourite/${quote.id}`)
    quote.isFavourite = res.data.isFavourite
  } catch (error) {
    console.error('Failed to toggle favourite:', error)
  }
}
</script>

<style scoped>
.heart {
  cursor: pointer;
  font-size: 1.3em;
  margin-left: 10px;
  transition: transform 0.1s;
  user-select: none;
}
.heart.favourite {
  color: #e25555;
  transform: scale(1.2);
}
.heart:hover {
  transform: scale(1.2);
}
.quotes-container {
  max-width: 700px;
  margin: 0 auto;
  padding: 40px 20px;
}

.quotes-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

.quotes-header h1 {
  margin: 0;
  color: #333;
  font-size: 2.5rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.favourites-link {
  display: flex;
  align-items: center;
  padding: 12px 20px;
  background: linear-gradient(135deg, #ff6b6b 0%, #ee5a52 100%);
  color: white;
  text-decoration: none;
  border-radius: 12px;
  font-weight: 600;
  font-size: 14px;
  transition: all 0.3s ease;
  box-shadow: 0 4px 12px rgba(255, 107, 107, 0.2);
}

.favourites-link:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(255, 107, 107, 0.3);
}

.favourites-icon {
  font-size: 16px;
  margin-right: 8px;
}

.favourites-text {
  font-size: 14px;
}
.quotes-list {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.quote-card {
  background: #f7f7fa;
  border-radius: 16px;
  padding: 24px;
  box-shadow: 0 4px 20px rgba(102, 126, 234, 0.08);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  position: relative;
}

.quote-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 30px rgba(102, 126, 234, 0.15);
}

.quote-card.today-quote {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  box-shadow: 0 8px 30px rgba(102, 126, 234, 0.25);
  transform: scale(1.02);
}

.today-badge {
  position: absolute;
  top: -10px;
  left: 20px;
  background: linear-gradient(135deg, #ffd700 0%, #ffed4e 100%);
  color: #333;
  padding: 6px 16px;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 600;
  box-shadow: 0 4px 12px rgba(255, 215, 0, 0.3);
}

.quote-card blockquote {
  font-size: 1.2rem;
  font-style: italic;
  margin: 0;
  line-height: 1.6;
}

.quote-card.today-quote blockquote {
  font-size: 1.4rem;
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

.quote-card.today-quote footer {
  font-size: 1.1rem;
}

.date {
  font-size: 0.9rem;
  opacity: 0.8;
}

.quote-card.today-quote .date {
  opacity: 0.9;
}
</style>