<template>
  <div class="diary-container">
    <div class="diary-header">
      <h1>My Mood Diary</h1>
      <p class="diary-subtitle">Track your daily moods and emotions</p>
    </div>
    <div class="filter-section">
  <button @click="showFilters = !showFilters" class="filter-toggle">
    {{ showFilters ? 'Hide Filters' : 'Show Filters' }}
  </button>
  
  <div v-if="showFilters" class="filter-controls">
    <div class="filter-group">
      <label>Filter by Emotion:</label>
      <select v-model="selectedEmotionFilter" class="filter-select">
        <option v-for="option in emotionFilterOptions" 
                :key="option.value" 
                :value="option.value">
          {{ option.emoji }} {{ option.label }}
        </option>
      </select>
    </div>
    
    <div class="filter-group">
      <label>Filter by Grade:</label>
      <select v-model="selectedGradeFilter" class="filter-select">
        <option v-for="option in gradeFilterOptions" 
                :key="option.value" 
                :value="option.value">
          {{ option.label }}
        </option>
      </select>
    </div>
    
    <button @click="clearFilters" class="clear-filters-btn">
      Clear Filters
    </button>
  </div>

</div>
    <div class="diary-content">
      <div class="calendar-section">
        <div class="calendar-header">
          <button @click="previousMonth" class="nav-btn">
            <span>◀</span>
          </button>
          <h2 class="current-month">{{ currentMonthYear }}</h2>
          <button @click="nextMonth" class="nav-btn">
            <span>▶</span>
          </button>
        </div>

        <div class="calendar">
          <div class="calendar-weekdays">
            <div v-for="day in weekdays" :key="day" class="weekday">{{ day }}</div>
          </div>
          
          <div class="calendar-days">
  <div 
    v-for="day in calendarDays" 
    :key="day.date"
    :class="[
      'calendar-day',
      { 'other-month': !day.isCurrentMonth },
      { 'today': day.isToday },
      { 'selected': day.isSelected },
      { 'filtered-out': !day.isVisible && (selectedEmotionFilter !== 'all' || selectedGradeFilter !== 'all') }
    ]"
    @click="selectDate(day)"
  >
    <span class="day-number">{{ day.dayNumber }}</span>
    <div v-if="day.mood && day.isVisible" class="mood-indicator" :class="`mood-${day.mood}`">
      {{ getMoodEmoji(day.mood) }}
    </div>
    <div v-if="day.rating > 0 && day.isVisible" class="rating-indicator">
      <span class="rating-stars">
        {{ '★'.repeat(day.rating) }}{{ '☆'.repeat(5 - day.rating) }}
      </span>
    </div>
  </div>
</div>
        </div>
      </div>
      
      <div class="mood-entry-section">
                 <div class="selected-date-info">
           <h3>{{ selectedDateFormatted }}</h3>
           <div v-if="selectedDateMood || selectedDateRating > 0" class="current-entry">
             <p v-if="selectedDateMood" class="current-mood">
               Current mood: {{ getMoodEmoji(selectedDateMood) }} {{ selectedDateMood }}
             </p>
             <p v-if="selectedDateRating > 0" class="current-rating">
               Day rating: 
               <span class="rating-stars">
                 {{ '★'.repeat(selectedDateRating) }}{{ '☆'.repeat(5 - selectedDateRating) }}
               </span>
               ({{ selectedDateRating }}/5)
             </p>
             <!-- Edit/Delete Buttons -->
             <div class="entry-actions" style="margin-top: 10px;">
               <button v-if="!isEditing && (selectedDateMood || selectedDateRating > 0)" @click="startEditEntry" class="edit-btn">Edit</button>
               <button v-if="!isEditing && (selectedDateMood || selectedDateRating > 0)" @click="deleteCurrentEntry" class="delete-btn">Delete</button>
             </div>
           </div>
         </div>
        
        <div class="mood-entry-form">
          <h4>How are you feeling today?</h4>
          <div class="mood-options">
            <button 
              v-for="mood in moodOptions" 
              :key="mood.value"
              :class="['mood-option', { active: selectedMood === mood.value }]"
              @click="selectMood(mood.value)"
            >
              <span class="mood-emoji">{{ mood.emoji }}</span>
              <span class="mood-label">{{ mood.label }}</span>
            </button>
          </div>
          
                     <div class="day-rating">
             <label>Rate your day (1-5)</label>
             <div class="rating-buttons">
               <button 
                 v-for="rating in 5" 
                 :key="rating"
                 :class="['rating-btn', { active: selectedRating === rating }]"
                 @click="selectedRating = rating"
               >
                 <span class="star">{{ rating <= selectedRating ? '★' : '☆' }}</span>
                 <span class="rating-number">{{ rating }}</span>
               </button>
             </div>
           </div>
           
           <div class="entry-notes">
             <label for="notes">Notes (optional)</label>
             <textarea 
               id="notes"
               v-model="entryNotes"
               placeholder="Write about your day, thoughts, or feelings..."
               rows="4"
             ></textarea>
             <MoodAlert :sentiment="sentiment" />
           </div>
           
           <button @click="saveEntry" class="save-btn" :disabled="!selectedMood || selectedRating === 0">
             <span v-if="saving" class="loading-spinner"></span>
             <span v-else>Save Entry</span>
           </button>
        </div>
      </div>
    </div>
  </div>
  <!-- Edit Form Modal -->
  <div v-if="isEditing" class="edit-modal">
    <form @submit.prevent="submitEditEntry" class="edit-form">
      <label>Emotion</label>
      <select v-model="editForm.emotion">
        <option v-for="mood in moodOptions" :key="mood.value" :value="mood.value">{{ mood.emoji }} {{ mood.label }}</option>
      </select>
      <label>Grade</label>
      <input v-model.number="editForm.grade" type="number" min="1" max="5" />
      <label>Note</label>
      <textarea v-model="editForm.note" rows="3"></textarea>
      <div style="margin-top:10px;">
        <button type="submit" class="save-btn">Save</button>
        <button type="button" @click="cancelEditEntry" class="cancel-btn">Cancel</button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import api from '@/services/api'
import MoodAlert from '@/components/MoodAlert.vue'
import { analyzeSentiment } from '@/services/api'
import { watch } from 'vue'
import { updateDiaryEntry, deleteDiaryEntry } from '@/services/api'

const authStore = useAuthStore()

// Calendar state
const currentDate = ref(new Date())
const selectedDate = ref(new Date())
const selectedMood = ref('')
const selectedRating = ref(0)
const entryNotes = ref('')
const saving = ref(false)
const sentiment = ref(null)
// Update moodEntries type to include id
const moodEntries = ref<Record<string, { id: number; mood: string; rating: number; notes: string }>>({})
const weekdays = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']

const selectedEmotionFilter = ref('all')
const selectedGradeFilter = ref('all')
const showFilters = ref(false)

const isEditing = ref(false)
// Update editForm to include id
const editForm = ref({
  id: 0,
  emotion: '',
  grade: 0,
  note: ''
})

const moodOptions = [
  { value: 'happy', label: 'Happy', emoji: '😊' },
  { value: 'excited', label: 'Excited', emoji: '🎉' },
  { value: 'calm', label: 'Calm', emoji: '😌' },
  { value: 'neutral', label: 'Neutral', emoji: '😐' },
  { value: 'sad', label: 'Sad', emoji: '😢' },
  { value: 'anxious', label: 'Anxious', emoji: '😰' },
  { value: 'angry', label: 'Angry', emoji: '😠' },
  { value: 'tired', label: 'Tired', emoji: '😴' }
]

const emotionFilterOptions = [
  {value: 'all', label: 'All emotions'},
  { value: 'happy', label: 'Happy', emoji: '😊' },
  { value: 'excited', label: 'Excited', emoji: '🎉' },
  { value: 'calm', label: 'Calm', emoji: '😌' },
  { value: 'neutral', label: 'Neutral', emoji: '😐' },
  { value: 'sad', label: 'Sad', emoji: '😢' },
  { value: 'anxious', label: 'Anxious', emoji: '😰' },
  { value: 'angry', label: 'Angry', emoji: '😠' },
  { value: 'tired', label: 'Tired', emoji: '😴' }
]

const gradeFilterOptions = [
  { value: 'all', label: 'All Grades' },
  { value: '1', label: 'Grade 1' },
  { value: '2', label: 'Grade 2' },
  { value: '3', label: 'Grade 3' },
  { value: '4', label: 'Grade 4' },
  { value: '5', label: 'Grade 5' }
]

const filteredEntries = computed(() => {
  const entries = Object.entries(moodEntries.value).map(([date, entry]) => ({
    date,
    ...entry
  }))

  return entries.filter(entry => {
    const emotionMatch = selectedEmotionFilter.value === 'all' || 
                        entry.mood === selectedEmotionFilter.value
    const gradeMatch = selectedGradeFilter.value === 'all' || 
                      entry.rating?.toString() === selectedGradeFilter.value
    
    return emotionMatch && gradeMatch
  })
})
const clearFilters = () => {
  selectedEmotionFilter.value = 'all'
  selectedGradeFilter.value = 'all'
}

const getFilteredEntriesCount = computed(() => {
  return filteredEntries.value.length
})
   watch(entryNotes, async (newNote) => {
     if (newNote.trim().length > 0) {
       sentiment.value = await analyzeSentiment(newNote)
     } else {
       sentiment.value = null
     }
   })
   
const currentMonthYear = computed(() => {
  return currentDate.value.toLocaleDateString('en-US', { 
    month: 'long', 
    year: 'numeric' 
  })
})

const selectedDateFormatted = computed(() => {
  return selectedDate.value.toLocaleDateString('en-US', { 
    weekday: 'long', 
    year: 'numeric', 
    month: 'long', 
    day: 'numeric' 
  })
})

const selectedDateMood = computed(() => {
  const dateKey = selectedDate.value.toISOString().split('T')[0]
  return moodEntries.value[dateKey]?.mood || null
})
   const selectedDateRating = computed(() => {
     const dateKey = selectedDate.value.toISOString().split('T')[0]
     return moodEntries.value[dateKey]?.rating || 0
   })
const calendarDays = computed(() => {
  const year = currentDate.value.getFullYear()
  const month = currentDate.value.getMonth()
  
  const firstDay = new Date(year, month, 1)
  const lastDay = new Date(year, month + 1, 0)
  const startDate = new Date(firstDay)
  startDate.setDate(startDate.getDate() - firstDay.getDay())
  
  const days = []
  const today = new Date()
  
  for (let i = 0; i < 42; i++) {
    const date = new Date(startDate)
    date.setDate(startDate.getDate() + i)
    
    const dateKey = date.toISOString().split('T')[0]
    const moodEntry = moodEntries.value[dateKey]
    
    // Check if this entry matches current filters
    const emotionMatch = selectedEmotionFilter.value === 'all' || 
                        !moodEntry || 
                        moodEntry.mood === selectedEmotionFilter.value
    
    const gradeMatch = selectedGradeFilter.value === 'all' || 
                      !moodEntry || 
                      moodEntry.rating?.toString() === selectedGradeFilter.value
    
    const isVisible = emotionMatch && gradeMatch
    
    days.push({
      date: dateKey,
      dayNumber: date.getDate(),
      isCurrentMonth: date.getMonth() === month,
      isToday: date.toDateString() === today.toDateString(),
      isSelected: date.toDateString() === selectedDate.value.toDateString(),
      mood: moodEntry?.mood || null,
      rating: moodEntry?.rating || 0,
      isVisible: isVisible
    })
  }
  
  return days
})

const getMoodEmoji = (mood: string) => {
  const moodOption = moodOptions.find(option => option.value === mood)
  return moodOption?.emoji || '❓'
}

const previousMonth = () => {
  currentDate.value = new Date(currentDate.value.getFullYear(), currentDate.value.getMonth() - 1, 1)
}

const nextMonth = () => {
  currentDate.value = new Date(currentDate.value.getFullYear(), currentDate.value.getMonth() + 1, 1)
}

const selectDate = (day: any) => {
  selectedDate.value = new Date(day.date)
  selectedMood.value = day.mood || ''
  
  const dateKey = day.date
  const entry = moodEntries.value[dateKey]
  entryNotes.value = entry?.notes || ''
}

const selectMood = (mood: string) => {
  selectedMood.value = mood
}

const saveEntry = async () => {
  if (!selectedMood.value || selectedRating.value === 0) return
  
  saving.value = true
  
  try {
    const response = await api.post('/diaryitem/create', {
      emotion: selectedMood.value,
      grade: selectedRating.value,
      note: entryNotes.value
    })
    
    // Update local state with the response data
    const dateKey = selectedDate.value.toISOString().split('T')[0]
    moodEntries.value[dateKey] = {
      id: response.data.data.id, // Store the new ID
      mood: response.data.data.emotion,
      rating: response.data.data.grade,
      notes: response.data.data.note || ''
    }
    
    // Reset form
    selectedMood.value = ''
    selectedRating.value = 0
    entryNotes.value = ''
    
    // Show success message
    console.log('Entry saved successfully!')
  } catch (error) {
    console.error('Failed to save entry:', error)
    // You can add error handling here (show error message to user)
  } finally {
    saving.value = false
  }
}

const loadEntries = async () => {
  try {
    const response = await api.get('/diaryitem/all')
    const entries = response.data.data

    entries.forEach((entry: any) => {
      const dateKey = new Date(entry.createdAt).toISOString().split('T')[0]
      moodEntries.value[dateKey] = {
        id: entry.id, // store id
        mood: entry.emotion,
        rating: entry.grade,
        notes: entry.note || ''
      }
    })
  } catch (error) {
    console.error('Failed to load entries:', error)
  }
}

function startEditEntry() {
  const dateKey = selectedDate.value.toISOString().split('T')[0]
  const entry = moodEntries.value[dateKey]
  if (entry) {
    editForm.value.id = entry.id
    editForm.value.emotion = entry.mood
    editForm.value.grade = entry.rating
    editForm.value.note = entry.notes
    isEditing.value = true
  }
}

function cancelEditEntry() {
  isEditing.value = false
}

async function submitEditEntry() {
  const dateKey = selectedDate.value.toISOString().split('T')[0]
  const entry = moodEntries.value[dateKey]
  if (!entry) return
  try {
    await updateDiaryEntry(entry.id, {
      emotion: editForm.value.emotion,
      grade: editForm.value.grade,
      note: editForm.value.note
    })
    // Update local state
    moodEntries.value[dateKey] = {
      id: entry.id,
      mood: editForm.value.emotion,
      rating: editForm.value.grade,
      notes: editForm.value.note
    }
    selectedMood.value = editForm.value.emotion;
    selectedRating.value = editForm.value.grade;
    entryNotes.value = editForm.value.note;
    isEditing.value = false
  } catch (error) {
    console.error('Failed to update entry:', error)
  }
}

async function deleteCurrentEntry() {
  const dateKey = selectedDate.value.toISOString().split('T')[0]
  const entry = moodEntries.value[dateKey]
  if (!entry) return
  if (confirm('Are you sure you want to delete this entry?')) {
    try {
      await deleteDiaryEntry(entry.id)
      // Remove from local state
      delete moodEntries.value[dateKey]
      selectedMood.value = ''
      selectedRating.value = 0
      entryNotes.value = ''
    } catch (error) {
      console.error('Failed to delete entry:', error)
    }
  }
}

onMounted(async () => {
 
  await loadEntries()
  
  selectDate({
    date: new Date().toISOString().split('T')[0],
    dayNumber: new Date().getDate(),
    isCurrentMonth: true,
    isToday: true,
    isSelected: true,
    mood: null
  })
})
</script>

<style scoped>
.diary-container {
  min-height: calc(100vh - 70px);
  background: linear-gradient(135deg, #667eea 0%, #764ba2 50%, #f093fb 100%);
  padding: 20px;
}

.diary-header {
  text-align: center;
  margin-bottom: 40px;
  color: white;
}

.diary-header h1 {
  font-size: 2.5rem;
  font-weight: 700;
  margin-bottom: 8px;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.diary-subtitle {
  font-size: 1.1rem;
  opacity: 0.9;
}

.diary-content {
  max-width: 1200px;
  margin: 0 auto;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 40px;
  align-items: start;
}

.calendar-section {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(20px);
  border-radius: 20px;
  padding: 30px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
}

.calendar-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 30px;
}

.current-month {
  font-size: 1.5rem;
  font-weight: 600;
  color: #333;
  margin: 0;
}

.nav-btn {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
  font-size: 16px;
}

.nav-btn:hover {
  transform: scale(1.1);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
}

.calendar {
  width: 100%;
}

.calendar-weekdays {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 8px;
  margin-bottom: 16px;
}

.weekday {
  text-align: center;
  font-weight: 600;
  color: #666;
  font-size: 14px;
  padding: 8px;
}

.calendar-days {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 8px;
}

.calendar-day {
  aspect-ratio: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
  background: rgba(255, 255, 255, 0.8);
  border: 2px solid transparent;
}

.calendar-day:hover {
  background: rgba(102, 126, 234, 0.1);
  transform: scale(1.05);
}

.calendar-day.other-month {
  opacity: 0.4;
}

.calendar-day.today {
  border-color: #667eea;
  background: rgba(102, 126, 234, 0.1);
}

.calendar-day.selected {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.day-number {
  font-weight: 600;
  font-size: 16px;
  margin-bottom: 4px;
}

.mood-indicator {
  font-size: 12px;
  margin-top: 2px;
}

.rating-indicator {
  margin-top: 2px;
}

.rating-stars {
  color: #ffd700;
  font-size: 10px;
  letter-spacing: 1px;
}

.mood-entry-section {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(20px);
  border-radius: 20px;
  padding: 30px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
}

.selected-date-info {
  margin-bottom: 30px;
  padding-bottom: 20px;
  border-bottom: 1px solid #eee;
}

.selected-date-info h3 {
  color: #333;
  font-size: 1.3rem;
  margin-bottom: 8px;
}

.current-mood,
.current-rating {
  color: #666;
  font-size: 1rem;
  margin: 0;
}

.current-rating {
  margin-top: 8px;
}

.current-rating .rating-stars {
  font-size: 14px;
  margin-left: 4px;
}

.mood-entry-form h4 {
  color: #333;
  font-size: 1.2rem;
  margin-bottom: 20px;
}

.mood-options {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 12px;
  margin-bottom: 30px;
}

.mood-option {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 16px;
  border: 2px solid #eee;
  border-radius: 12px;
  background: white;
  cursor: pointer;
  transition: all 0.3s ease;
}

.mood-option:hover {
  border-color: #667eea;
  transform: translateY(-2px);
}

.mood-option.active {
  border-color: #667eea;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.mood-emoji {
  font-size: 24px;
  margin-bottom: 8px;
}

.mood-label {
  font-size: 14px;
  font-weight: 500;
}

.day-rating {
  margin-bottom: 30px;
}

.day-rating label {
  display: block;
  font-weight: 600;
  color: #333;
  margin-bottom: 12px;
}

.rating-buttons {
  display: flex;
  gap: 8px;
  justify-content: center;
}

.rating-btn {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 12px 8px;
  border: 2px solid #eee;
  border-radius: 12px;
  background: white;
  cursor: pointer;
  transition: all 0.3s ease;
  min-width: 50px;
}

.rating-btn:hover {
  border-color: #ffd700;
  transform: translateY(-2px);
}

.rating-btn.active {
  border-color: #ffd700;
  background: linear-gradient(135deg, #ffd700 0%, #ffed4e 100%);
  color: #333;
}

.rating-btn .star {
  font-size: 20px;
  color: #ffd700;
  margin-bottom: 4px;
}

.rating-btn.active .star {
  color: #333;
}

.rating-btn .rating-number {
  font-size: 12px;
  font-weight: 600;
}

.entry-notes {
  margin-bottom: 30px;
}

.entry-notes label {
  display: block;
  font-weight: 600;
  color: #333;
  margin-bottom: 8px;
}

.entry-notes textarea {
  width: 100%;
  padding: 16px;
  border: 2px solid #eee;
  border-radius: 12px;
  font-family: inherit;
  font-size: 14px;
  resize: vertical;
  transition: border-color 0.3s ease;
}

.entry-notes textarea:focus {
  outline: none;
  border-color: #667eea;
}

.save-btn {
  width: 100%;
  padding: 16px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 12px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.save-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
}

.save-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.loading-spinner {
  width: 20px;
  height: 20px;
  border: 2px solid transparent;
  border-top: 2px solid white;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

@media (max-width: 768px) {
  .diary-content {
    grid-template-columns: 1fr;
    gap: 20px;
  }
  
  .mood-options {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .diary-header h1 {
    font-size: 2rem;
  }
}

.filter-section {
  margin-bottom: 20px;
}

.filter-toggle {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  margin-bottom: 15px;
}

.filter-controls {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(20px);
  border-radius: 12px;
  padding: 20px;
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
  display: flex;
  gap: 20px;
  align-items: end;
  flex-wrap: wrap;
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.filter-group label {
  font-weight: 600;
  color: #333;
  font-size: 14px;
}

.filter-select {
  padding: 8px 12px;
  border: 2px solid #eee;
  border-radius: 8px;
  font-size: 14px;
  background: white;
  cursor: pointer;
}

.filter-select:focus {
  outline: none;
  border-color: #667eea;
}

.clear-filters-btn {
  background: #ff6b6b;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  font-size: 14px;
}

.clear-filters-btn:hover {
  background: #ff5252;
}

.calendar-day.filtered-out {
  opacity: 0.3;
  background: rgba(200, 200, 200, 0.5);
  cursor: not-allowed;
}

.calendar-day.filtered-out:hover {
  transform: none;
  background: rgba(200, 200, 200, 0.5);
}

.calendar-day.filtered-out .mood-indicator,
.calendar-day.filtered-out .rating-indicator {
  display: none;
}

.edit-modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.edit-form {
  background: white;
  padding: 30px;
  border-radius: 15px;
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.2);
  width: 90%;
  max-width: 500px;
  position: relative;
}

.edit-form label {
  display: block;
  font-weight: 600;
  color: #333;
  margin-bottom: 10px;
  font-size: 14px;
}

.edit-form select,
.edit-form input {
  width: 100%;
  padding: 10px;
  border: 2px solid #eee;
  border-radius: 8px;
  font-size: 14px;
  margin-bottom: 20px;
  box-sizing: border-box;
}

.edit-form select:focus,
.edit-form input:focus {
  outline: none;
  border-color: #667eea;
}

.edit-form textarea {
  width: 100%;
  padding: 10px;
  border: 2px solid #eee;
  border-radius: 8px;
  font-size: 14px;
  margin-bottom: 20px;
  box-sizing: border-box;
  resize: vertical;
}

.edit-form textarea:focus {
  outline: none;
  border-color: #667eea;
}

.edit-form .save-btn,
.edit-form .cancel-btn {
  width: 48%;
  padding: 12px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: inline-block;
}

.edit-form .save-btn:hover:not(:disabled),
.edit-form .cancel-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
}

.edit-form .save-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.edit-form .cancel-btn {
  background: #ff6b6b;
  margin-left: 4%;
}

.edit-form .cancel-btn:hover {
  background: #ff5252;
}
.entry-actions {
  display: flex;
  gap: 12px;
  margin-top: 10px;
}

.edit-btn, .delete-btn {
  padding: 10px 22px;
  font-size: 1rem;
  font-weight: 600;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
  box-shadow: 0 2px 8px rgba(102, 126, 234, 0.08);
  outline: none;
}

.edit-btn {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: #fff;
}

.edit-btn:hover {
  background: linear-gradient(135deg, #5a67d8 0%, #6b46c1 100%);
  transform: translateY(-2px) scale(1.04);
  box-shadow: 0 4px 16px rgba(102, 126, 234, 0.18);
}

.delete-btn {
  background: linear-gradient(135deg, #ff6b6b 0%, #ff8e53 100%);
  color: #fff;
}

.delete-btn:hover {
  background: linear-gradient(135deg, #e53e3e 0%, #ff6b6b 100%);
  transform: translateY(-2px) scale(1.04);
  box-shadow: 0 4px 16px rgba(255, 107, 107, 0.18);
}
</style> 