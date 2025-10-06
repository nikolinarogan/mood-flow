
import { createApp } from 'vue'
import { createPinia } from 'pinia' //state management library, centralized place to store and manage data that multiple components in your app might need to access or modify

import App from './App.vue'
import router from './router'

const app = createApp(App)

app.use(createPinia())
app.use(router)

app.mount('#app')
