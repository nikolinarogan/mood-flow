<template>
  <div class="quotes-container">
    <div class="quotes-header">
      <h1>Quote of the Day</h1>
      <router-link to="/favourites" class="btn btn-primary">
        <span class="btn-icon">❤️</span>
        My Favourites
      </router-link>
    </div>
    <div v-if="todayQuote" class="today-quote-card">
      <blockquote>
        "{{ todayQuote.quoteText }}"
        <footer>
          - {{ todayQuote.author }}
          <span class="date">{{ formatDate(todayQuote.date) }}</span>
          <span
            class="heart"
            :class="{ favourite: isFavourite(todayQuote.id) }"
            @click="toggleFavourite(todayQuote)"
            :title="isFavourite(todayQuote.id) ? 'Remove from favourites' : 'Mark as favourite'"
          >
            {{ isFavourite(todayQuote.id) ? '❤️' : '🤍' }}
          </span>
        </footer>
      </blockquote>
    </div>
    <div v-if="archiveQuotes.length" class="archive-list">
      <div v-for="quote in archiveQuotes" :key="quote.id" class="archive-quote-card accent-bg">
        <blockquote>
          "{{ quote.quoteText }}"
          <footer>
            - {{ quote.author }}
            <span class="date">{{ formatDate(quote.date) }}</span>
            <span
              class="heart"
              :class="{ favourite: isFavourite(quote.id) }"
              @click="toggleFavourite(quote)"
              :title="isFavourite(quote.id) ? 'Remove from favourites' : 'Mark as favourite'"
            >
              {{ isFavourite(quote.id) ? '❤️' : '🤍' }}
            </span>
          </footer>
        </blockquote>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import api from '@/services/api'
import { getUserFavourites } from '@/services/api'

const todayQuote = ref<any>(null)
const allQuotes = ref<any[]>([])
const favouriteIds = ref<number[]>([])

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleDateString()
}

const archiveQuotes = computed(() => {
  if (!todayQuote.value) return allQuotes.value
  return allQuotes.value.filter(q => q.id !== todayQuote.value.id)
})

function isFavourite(quoteId: number) {
  return favouriteIds.value.includes(quoteId)
}

async function toggleFavourite(quote: any) {
  try {
    await api.post(`/quote/favourite/${quote.id}`)
    // Refresh favourites
    const res = await getUserFavourites()
    favouriteIds.value = res.data.map((q: any) => q.id)
  } catch (error) {
    console.error('Failed to toggle favourite:', error)
  }
}

onMounted(async () => {
  try {
    const [todayRes, allRes, favRes] = await Promise.all([
      api.get('/quote/today'),
      api.get('/quote/all'),
      getUserFavourites()
    ])
    todayQuote.value = todayRes.data
    allQuotes.value = allRes.data
    favouriteIds.value = favRes.data.map((q: any) => q.id)
  } catch (error) {
    console.error('Failed to fetch quotes:', error)
  }
})
</script>

<style scoped>
.quotes-container {
  max-width: 800px;
  margin: 0 auto;
  padding: 40px 20px;
  width: 100%;
  box-sizing: border-box;
}

.quotes-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 40px;
}

.quotes-header h1 {
  color: #333;
  font-size: 2.5rem;
  margin-bottom: 8px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
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

.quotes-list {
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
  position: relative;
  display: flex;
  flex-direction: column;
  height: 100%;
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

.date {
  font-size: 0.9rem;
  opacity: 0.8;
}

.heart {
  cursor: pointer;
  font-size: 1.4em;
  margin-left: 10px;
  transition: transform 0.2s, color 0.2s;
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

.today-quote-card {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-radius: 20px;
  padding: 32px 24px;
  box-shadow: 0 6px 24px rgba(102, 126, 234, 0.18);
  margin-bottom: 36px;
  text-align: center;
  font-size: 1.3rem;
  position: relative;
  width: 100%;
}
.today-quote-card blockquote {
  font-size: 1.4rem;
  font-style: italic;
  margin: 0;
  line-height: 1.7;
}
.today-quote-card footer {
  margin-top: 18px;
  font-size: 1.1rem;
  opacity: 0.92;
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 18px;
}
.archive-list {
  display: flex;
  flex-direction: column;
  gap: 18px;
  margin-top: 0;
}
.archive-quote-card {
  border-radius: 16px;
  padding: 22px 18px;
  box-shadow: 0 2px 12px rgba(102, 126, 234, 0.08);
  color: #333;
  font-size: 1.1rem;
  margin-bottom: 0;
}
.archive-quote-card blockquote {
  font-size: 1.1rem;
  font-style: italic;
  margin: 0;
  line-height: 1.6;
}
.archive-quote-card footer {
  margin-top: 14px;
  font-size: 1rem;
  opacity: 0.85;
  display: flex;
  justify-content: flex-end;
  align-items: center;
  gap: 12px;
}
.accent-bg {
  background: linear-gradient(135deg, #ede9fe 0%, #c7d2fe 100%);
}
@media (max-width: 900px) {
  .quotes-list {
    grid-template-columns: 1fr;
    gap: 18px;
  }
  .quotes-container {
    max-width: 98vw;
    padding: 20px 10px;
  }
  .quotes-header h1 {
    font-size: 2rem;
  }
  .quote-card blockquote {
    font-size: 1.1rem;
  }
}
@media (max-width: 700px) {
  .today-quote-card {
    padding: 18px 8px;
    font-size: 1.1rem;
  }
  .archive-quote-card {
    padding: 12px 6px;
    font-size: 1rem;
  }
}
</style>