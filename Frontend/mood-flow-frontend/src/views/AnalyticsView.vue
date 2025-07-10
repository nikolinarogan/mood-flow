<template>
  <div class="analytics-container">
    <div class="analytics-header">
      <h1>Mood Analytics</h1>
      <p class="subtitle">Track your emotional journey and discover patterns</p>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="loading-container">
      <div class="loading-spinner"></div>
      <p>Loading your analytics...</p>
    </div>

    <!-- Analytics Content -->
    <div v-else class="analytics-content">
      <!-- Summary Cards -->
      <div class="summary-cards">
        <div class="summary-card">
          <div class="card-icon">📊</div>
          <div class="card-content">
            <h3>Total Entries</h3>
            <p class="card-value">{{ analyticsData?.totalEntries || 0 }}</p>
          </div>
        </div>
        
        <div class="summary-card">
          <div class="card-icon">😊</div>
          <div class="card-content">
            <h3>Average Mood</h3>
            <p class="card-value">{{ analyticsData?.averageMood?.toFixed(1) || '0.0' }}/10</p>
          </div>
        </div>
        
        <div class="summary-card">
          <div class="card-icon">📈</div>
          <div class="card-content">
            <h3>Best Day</h3>
            <p class="card-value">{{ analyticsData?.bestDay || 'N/A' }}</p>
          </div>
        </div>
        
        <div class="summary-card">
          <div class="card-icon">🎯</div>
          <div class="card-content">
            <h3>Most Common Emotion</h3>
            <p class="card-value">{{ analyticsData?.mostCommonEmotion || 'N/A' }}</p>
          </div>
        </div>
      </div>

            <!-- Analytics Sections -->
      <div class="analytics-sections">
        <!-- Weekly Mood Heatmap -->
        <div class="analytics-container">
          <h2>Weekly Mood Pattern (Last 4 Weeks)</h2>
          <div class="weekly-heatmap">
            <div class="heatmap-header">
              <span v-for="day in weekDays" :key="day" class="day-label">{{ day }}</span>
            </div>
            <div class="heatmap-grid">
              <div 
                v-for="(week, weekIndex) in analyticsData?.weeklyMoodData || []" 
                :key="weekIndex" 
                class="week-row"
              >
                <div class="week-label">Week {{ 4 - weekIndex }}</div>
                <div 
                  v-for="(mood, dayIndex) in week" 
                  :key="dayIndex" 
                  class="mood-cell"
                  :class="getMoodClass(mood)"
                  :title="`${weekDays[dayIndex]} Week ${4 - weekIndex}: ${mood || 'No data'}`"
                >
                  {{ mood || '' }}
                </div>
              </div>
            </div>
            <div class="heatmap-legend">
              <div class="legend-item">
                <div class="legend-color excellent"></div>
                <span>Excellent (8-10)</span>
              </div>
              <div class="legend-item">
                <div class="legend-color good"></div>
                <span>Good (6-7)</span>
              </div>
              <div class="legend-item">
                <div class="legend-color average"></div>
                <span>Average (4-5)</span>
              </div>
              <div class="legend-item">
                <div class="legend-color poor"></div>
                <span>Poor (1-3)</span>
              </div>
              <div class="legend-item">
                <div class="legend-color no-data"></div>
                <span>No Data</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Monthly Statistics -->
        <div class="analytics-container">
          <h2>Monthly Statistics</h2>
          <div class="monthly-stats">
            <div class="stat-item">
              <h4>Entries This Month</h4>
              <p>{{ analyticsData?.monthlyStats?.entries || 0 }}</p>
            </div>
            <div class="stat-item">
              <h4>Average Mood</h4>
              <p>{{ analyticsData?.monthlyStats?.averageMood?.toFixed(1) || '0.0' }}/10</p>
            </div>
            <div class="stat-item">
              <h4>Best Day</h4>
              <p>{{ analyticsData?.monthlyStats?.bestDay || 'N/A' }}</p>
            </div>
            <div class="stat-item">
              <h4>Mood Range</h4>
              <p>{{ analyticsData?.monthlyStats?.lowestMood || 0 }} - {{ analyticsData?.monthlyStats?.highestMood || 0 }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Insights Section -->
      <div class="insights-section">
        <h2>Mood Insights</h2>
        <div class="insights-grid">
          <div class="insight-card" v-for="insight in analyticsData?.insights || []" :key="insight.title">
            <div class="insight-icon">{{ insight.icon }}</div>
            <h4>{{ insight.title }}</h4>
            <p>{{ insight.description }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getAnalyticsSummary } from '@/services/api'

interface AnalyticsData {
  totalEntries: number
  averageMood: number
  bestDay: string
  mostCommonEmotion: string
  monthlyStats: {
    entries: number
    averageMood: number
    bestDay: string
    lowestMood: number
    highestMood: number
  }
  weeklyMoodData: (number | null)[][]
  insights: Array<{
    icon: string
    title: string
    description: string
  }>
}

const loading = ref(true)
const analyticsData = ref<AnalyticsData | null>(null)

const weekDays = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']

const getMoodClass = (mood: number | null) => {
  if (mood === null) return 'no-data'
  if (mood >= 8) return 'excellent'
  if (mood >= 6) return 'good'
  if (mood >= 4) return 'average'
  return 'poor'
}

const fetchAnalyticsData = async () => {
  try {
    const response = await getAnalyticsSummary()
    analyticsData.value = response.data
    console.log('Analytics data:', analyticsData.value)
  } catch (error) {
    console.error('Error fetching analytics data:', error)
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  await fetchAnalyticsData()
})
</script>

<style scoped>
.analytics-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

.analytics-header {
  text-align: center;
  margin-bottom: 40px;
}

.analytics-header h1 {
  font-size: 2.5rem;
  color: #2c3e50;
  margin-bottom: 10px;
}

.subtitle {
  color: #7f8c8d;
  font-size: 1.1rem;
}

.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 400px;
}

.loading-spinner {
  width: 50px;
  height: 50px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #3498db;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 20px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.summary-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
  margin-bottom: 40px;
}

.summary-card {
  background: white;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  display: flex;
  align-items: center;
  transition: transform 0.2s;
}

.summary-card:hover {
  transform: translateY(-2px);
}

.card-icon {
  font-size: 2rem;
  margin-right: 16px;
}

.card-content h3 {
  margin: 0 0 8px 0;
  color: #2c3e50;
  font-size: 0.9rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.card-value {
  margin: 0;
  font-size: 1.8rem;
  font-weight: bold;
  color: #3498db;
}

.analytics-sections {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(500px, 1fr));
  gap: 30px;
  margin-bottom: 40px;
}

.analytics-container {
  background: white;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.analytics-container h2 {
  margin: 0 0 20px 0;
  color: #2c3e50;
  font-size: 1.3rem;
}

.weekly-heatmap {
  margin-top: 20px;
}

.heatmap-header {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 4px;
  margin-bottom: 8px;
}

.day-label {
  text-align: center;
  font-size: 0.8rem;
  color: #7f8c8d;
  font-weight: 600;
}

.heatmap-grid {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.week-row {
  display: grid;
  grid-template-columns: auto repeat(7, 1fr);
  gap: 4px;
  align-items: center;
}

.week-label {
  font-size: 0.8rem;
  color: #7f8c8d;
  font-weight: 600;
  text-align: right;
  padding-right: 8px;
}

.mood-cell {
  aspect-ratio: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
  font-size: 0.8rem;
  font-weight: 600;
  color: white;
}

.mood-cell.excellent {
  background-color: #27ae60;
}

.mood-cell.good {
  background-color: #2ecc71;
}

.mood-cell.average {
  background-color: #f39c12;
}

.mood-cell.poor {
  background-color: #e74c3c;
}

.mood-cell.no-data {
  background-color: #ecf0f1;
  color: #bdc3c7;
}

.heatmap-legend {
  display: flex;
  justify-content: center;
  gap: 16px;
  margin-top: 16px;
  flex-wrap: wrap;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: 0.8rem;
  color: #7f8c8d;
}

.legend-color {
  width: 16px;
  height: 16px;
  border-radius: 2px;
}

.legend-color.excellent {
  background-color: #27ae60;
}

.legend-color.good {
  background-color: #2ecc71;
}

.legend-color.average {
  background-color: #f39c12;
}

.legend-color.poor {
  background-color: #e74c3c;
}

.legend-color.no-data {
  background-color: #ecf0f1;
}

.monthly-stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
}

.stat-item {
  text-align: center;
  padding: 16px;
  background: #f8f9fa;
  border-radius: 8px;
}

.stat-item h4 {
  margin: 0 0 8px 0;
  color: #2c3e50;
  font-size: 0.9rem;
  font-weight: 600;
}

.stat-item p {
  margin: 0;
  font-size: 1.5rem;
  font-weight: bold;
  color: #3498db;
}

.insights-section {
  background: white;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.insights-section h2 {
  margin: 0 0 20px 0;
  color: #2c3e50;
  font-size: 1.3rem;
}

.insights-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 20px;
}

.insight-card {
  padding: 20px;
  background: #f8f9fa;
  border-radius: 8px;
  border-left: 4px solid #3498db;
}

.insight-icon {
  font-size: 2rem;
  margin-bottom: 12px;
}

.insight-card h4 {
  margin: 0 0 8px 0;
  color: #2c3e50;
  font-size: 1.1rem;
}

.insight-card p {
  margin: 0;
  color: #7f8c8d;
  line-height: 1.5;
}

@media (max-width: 768px) {
  .analytics-container {
    padding: 10px;
  }
  
  .analytics-header h1 {
    font-size: 2rem;
  }
  
  .charts-section {
    grid-template-columns: 1fr;
  }
  
  .summary-cards {
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  }
  
  .monthly-stats {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .insights-grid {
    grid-template-columns: 1fr;
  }
}
</style> 