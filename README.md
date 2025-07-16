# 🌊 Mood Flow

A comprehensive mental wellness application that helps users track their daily moods, receive personalized quotes, practice meditation, and gain insights through analytics.

## ✨ Features

### 🧠 **Mood Tracking**
- Daily mood diary with emotion selection
- 1-5 star rating system for daily experiences
- Optional notes with sentiment analysis
- Calendar view with mood indicators
- Filter by emotions and ratings
- Edit and delete diary entries

### 📊 **Analytics & Insights**
- Monthly mood trends visualization
- Emotion distribution charts
- Rating statistics and patterns
- Interactive charts and graphs
- Progress tracking over time

### 💭 **Daily Quotes**
- Personalized daily inspirational quotes
- Favorite quotes system
- Quote history and browsing
- Heart button to save favorites

### 🧘 **Meditation & Breathing**
- Guided meditation exercises
- YouTube video integration
- Favorite meditation system
- Breathing exercises and relaxation techniques

### 🔐 **User Management**
- Secure authentication system
- User registration and login
- Profile management
- Password strength validation
- Email verification system

### ⏰ **Notifications**
- Customizable daily reminder times
- Email notification system
- Gentle reminders for mood tracking

### 💖 **Favorites System**
- Save favorite quotes and meditation exercises
- Organized favorites page with two sections
- Easy access to saved content
- Remove favorites with one click

## 🛠️ Tech Stack

### **Backend**
- **.NET 8** with C#
- **Entity Framework Core** for data access
- **PostgreSQL** database
- **JWT Authentication**
- **Swagger/OpenAPI** documentation
- **Dependency Injection** pattern
- **Service Layer** architecture

### **Frontend**
- **Vue 3** with Composition API
- **TypeScript** for type safety
- **Pinia** for state management
- **Vue Router** for navigation
- **Vite** for fast development
- **Tailwind CSS** for styling
- **Responsive design** for all devices

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- Node.js 18+ and npm
- PostgreSQL database
- Git

### Backend Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd mood-flow/Backend/MoodFlow/MoodFlow
   ```

2. **Configure database connection**
   - Update `appsettings.json` with your PostgreSQL connection string
   - Ensure PostgreSQL is running

3. **Run database migrations**
   ```bash
   dotnet ef database update
   ```

4. **Start the backend server**
   ```bash
   dotnet run
   ```
   The API will be available at `https://localhost:7000`

### Frontend Setup

1. **Navigate to frontend directory**
   ```bash
   cd ../../Frontend/mood-flow-frontend
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Start the development server**
   ```bash
   npm run dev
   ```
   The app will be available at `http://localhost:5173`

## 📁 Project Structure

```
mood-flow/
├── Backend/
│   └── MoodFlow/
│       └── MoodFlow/
│           ├── Controllers/          # API endpoints
│           ├── Services/             # Business logic
│           ├── Models/               # Data models
│           ├── DTOs/                 # Data transfer objects
│           ├── Data/                 # Database context
│           └── Migrations/           # Database migrations
└── Frontend/
    └── mood-flow-frontend/
        ├── src/
        │   ├── components/           # Vue components
        │   ├── views/                # Page components
        │   ├── stores/               # Pinia stores
        │   ├── services/             # API services
        │   ├── types/                # TypeScript types
        │   └── router/               # Vue Router config
        └── public/                   # Static assets
```


**Built with ❤️ for mental wellness and mindfulness**
