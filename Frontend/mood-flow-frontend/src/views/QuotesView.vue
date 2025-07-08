<template>
  <div class="quotes-container">
    <h1>Daily Quotes</h1>
    <div v-if="todayQuote" class="today-quote">
      <h2>Today's Quote</h2>
      <blockquote>
        "{{ todayQuote.quoteText }}"
        <footer>- {{ todayQuote.author }}</footer>
      </blockquote>
    </div>
    <div v-if="previousQuotes.length" class="previous-quotes">
      <h3>Previous Quotes</h3>
      <ul>
        <li v-for="quote in previousQuotes" :key="quote.id">
          <blockquote>
            "{{ quote.quoteText }}"
            <footer>- {{ quote.author }} ({{ formatDate(quote.date) }})</footer>
          </blockquote>
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import api from '@/services/api'

const todayQuote = ref<any>(null)
const previousQuotes = ref<any[]>([])

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleDateString()
}

onMounted(async () => {
  // Fetch today's quote
  const todayRes = await api.get('/quote/today')
  todayQuote.value = todayRes.data

  // Fetch all quotes (excluding today)
  const allRes = await api.get('/quote/all')
  previousQuotes.value = allRes.data.filter((q: any) => q.date !== todayQuote.value.date)
})
</script>

<style scoped>
.quotes-container {
  max-width: 700px;
  margin: 0 auto;
  padding: 40px 20px;
}
.today-quote {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: #fff;
  border-radius: 16px;
  padding: 32px 24px;
  margin-bottom: 32px;
  box-shadow: 0 4px 24px rgba(102, 126, 234, 0.12);
}
.today-quote blockquote {
  font-size: 1.4rem;
  font-style: italic;
  margin: 0;
}
.today-quote footer {
  margin-top: 16px;
  font-size: 1.1rem;
  text-align: right;
  opacity: 0.9;
}
.previous-quotes h3 {
  margin-bottom: 16px;
}
.previous-quotes ul {
  list-style: none;
  padding: 0;
}
.previous-quotes li {
  background: #f7f7fa;
  border-radius: 10px;
  margin-bottom: 18px;
  padding: 18px 16px;
  box-shadow: 0 2px 8px rgba(102, 126, 234, 0.06);
}
.previous-quotes blockquote {
  margin: 0;
  font-size: 1.1rem;
}
.previous-quotes footer {
  margin-top: 8px;
  font-size: 0.95rem;
  text-align: right;
  color: #555;
}
</style>