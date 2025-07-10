import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import ProfileView from '@/views/ProfileView.vue'
import DiaryView from '@/views/DiaryView.vue'
import QuotesView from '@/views/QuotesView.vue'
import FavouritesView from '@/views/FavouritesView.vue'
import AnalyticsView from '@/views/AnalyticsView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView
    },
    {
      path: '/register',
      name: 'register',
      component: RegisterView
    },
    {
      path: '/profile',
      name: 'profile',
      component: ProfileView
    },
    {
      path: '/diary',
      name: 'diary',
      component: DiaryView
    },
    {
      path: '/quotes',
      name: 'Quotes',
      component: QuotesView
    },
    {
      path: '/favourites',
      name: 'favourites',
      component: FavouritesView
    },
    {
      path: '/analytics',
      name: 'analytics',
      component: AnalyticsView
    }
  ]
})

export default router