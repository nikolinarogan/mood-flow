<template>
  <div v-if="sentiment" :class="['mood-alert', sentimentClass]">
    <span class="emoji">{{ emoji }}</span>
    <span class="label">{{ sentimentLabel }}</span>
    <div class="advice">{{ advice }}</div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  sentiment: {
    sentimentLabel: string
    detectedEmotion: string
    sentimentScore: number
  } | null
}>()

const sentimentClass = computed(() => {
  if (!props.sentiment) return ''
  switch (props.sentiment.sentimentLabel) {
    case 'positive': return 'positive'
    case 'negative': return 'negative'
    default: return 'neutral'
  }
})

const emoji = computed(() => {
  if (!props.sentiment) return ''
  switch (props.sentiment.detectedEmotion) {
    case 'happy': return '😊'
    case 'excited': return '🎉'
    case 'calm': return '😌'
    case 'neutral': return '😐'
    case 'sad': return '😢'
    case 'anxious': return '😰'
    case 'angry': return '😠'
    case 'tired': return '😴'
    default: return '🙂'
  }
})

const sentimentLabel = computed(() => {
  if (!props.sentiment) return ''
  return props.sentiment.sentimentLabel.charAt(0).toUpperCase() + props.sentiment.sentimentLabel.slice(1)
})

// Add advice/message based on sentiment
const advice = computed(() => {
  if (!props.sentiment) return ''
  switch (props.sentiment.sentimentLabel) {
    case 'positive':
      return "Great job! Keep up the positive vibes. 😊";
    case 'negative':
      switch (props.sentiment.detectedEmotion) {
        case 'sad':
          return "It's okay to feel sad. Consider talking to a friend or doing something you enjoy.";
        case 'anxious':
          return "Take a deep breath. Try some relaxation techniques or a short walk.";
        case 'angry':
          return "Try to relax and let go of anger. Maybe write down your feelings or talk to someone.";
        case 'tired':
          return "Rest is important. Make sure to take care of yourself and get enough sleep.";
        default:
          return "Remember, tough days happen. Take care of yourself!";
      }
    case 'neutral':
      return "A regular day is still a step forward. Reflect and see if there's something small to enjoy!";
    default:
      return "";
  }
})
</script>

<style scoped>
.mood-alert {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 0.5em 1em;
  border-radius: 8px;
  font-size: 1.2em;
  margin: 0.5em 0;
  font-weight: bold;
}
.mood-alert.positive { background: #e0ffe0; color: #2e7d32; }
.mood-alert.neutral { background: #f0f0f0; color: #616161; }
.mood-alert.negative { background: #ffe0e0; color: #c62828; }
.emoji { font-size: 2em; margin-right: 0.5em; }
.advice {
  font-size: 0.95em;
  font-weight: normal;
  margin-top: 0.5em;
  color: #333;
}
</style>