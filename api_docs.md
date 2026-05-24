# API Документация - Crucian Game Server

## Базовая информация

**Base URL:** `http://localhost:6767`

**Content-Type:** `application/json`

**Аутентификация:** Token-based (токен возвращается при регистрации/логине)

**Доступные endpoints:**
- `POST /api/auth/register` - Регистрация нового пользователя
- `POST /api/auth/login` - Вход в систему
- `POST /api/auth/loadUserData` - Загрузка данных пользователя
- `POST /api/auth/saveUserData` - Сохранение данных пользователя
- `POST /api/game/leaderboard` - Загрузка топ-20 игроков

---

## Endpoints

### 1. Регистрация пользователя

Создает нового пользователя в системе.

**Endpoint:** `POST /api/auth/register`

**Request Body:**
```json
{
  "username": "string",
  "password": "string"
}
```

**Поля запроса:**
| Поле | Тип | Обязательное | Описание |
|------|-----|--------------|----------|
| username | string | Да | Уникальное имя пользователя (3-50 символов) |
| password | string | Да | Пароль (минимум 6 символов) |

**Успешный ответ (201 Created):**
```json
{
  "success": true,
  "message": "User registered successfully",
  "token": "cGxheWVyMToxNzM3NTg5MjM0NTY3ODkwMTIz"
}
```

**Поля ответа:**
| Поле | Тип | Описание |
|------|-----|----------|
| success | boolean | Статус операции |
| message | string | Сообщение о результате |
| token | string | Токен доступа (действителен 24 часа) |

**Возможные ошибки:**

**400 Bad Request** - Неверный формат запроса:
```json
{
  "success": false,
  "message": "Invalid request format"
}
```

**400 Bad Request** - Пустые поля:
```json
{
  "success": false,
  "message": "Username and password are required"
}
```

**409 Conflict** - Пользователь уже существует:
```json
{
  "success": false,
  "message": "Username already exists"
}
```

**500 Internal Server Error** - Ошибка сервера:
```json
{
  "success": false,
  "message": "Failed to create user"
}
```

**Пример запроса (curl):**
```bash
curl -X POST http://localhost:6767/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "player1",
    "password": "mypassword123"
  }'
```

**Пример запроса (JavaScript/Fetch):**
```javascript
const response = await fetch('http://localhost:6767/api/auth/register', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({
    username: 'player1',
    password: 'mypassword123'
  })
});

const data = await response.json();
console.log(data.token); // Сохраните токен для дальнейших запросов
```

---

### 2. Вход в систему

Аутентифицирует существующего пользователя.

**Endpoint:** `POST /api/auth/login`

**Request Body:**
```json
{
  "username": "string",
  "password": "string"
}
```

**Поля запроса:**
| Поле | Тип | Обязательное | Описание |
|------|-----|--------------|----------|
| username | string | Да | Имя пользователя |
| password | string | Да | Пароль |

**Успешный ответ (200 OK):**
```json
{
  "success": true,
  "message": "Login successful",
  "token": "cGxheWVyMToxNzM3NTg5MjM0NTY3ODkwMTIz"
}
```

**Поля ответа:**
| Поле | Тип | Описание |
|------|-----|----------|
| success | boolean | Статус операции |
| message | string | Сообщение о результате |
| token | string | Новый токен доступа (действителен 24 часа) |

**Возможные ошибки:**

**400 Bad Request** - Неверный формат:
```json
{
  "success": false,
  "message": "Invalid request format"
}
```

**400 Bad Request** - Пустые поля:
```json
{
  "success": false,
  "message": "Username and password are required"
}
```

**401 Unauthorized** - Неверные учетные данные:
```json
{
  "success": false,
  "message": "Invalid username or password"
}
```

**Пример запроса (curl):**
```bash
curl -X POST http://localhost:6767/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "player1",
    "password": "mypassword123"
  }'
```

**Пример запроса (JavaScript/Fetch):**
```javascript
const response = await fetch('http://localhost:6767/api/auth/login', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({
    username: 'player1',
    password: 'mypassword123'
  })
});

const data = await response.json();
if (data.success) {
  localStorage.setItem('authToken', data.token);
}
```

---

### 3. Загрузка данных пользователя

Получает игровые данные аутентифицированного пользователя.

**Endpoint:** `POST /api/auth/loadUserData`

**Request Body:**
```json
{
  "token": "string"
}
```

**Поля запроса:**
| Поле | Тип | Обязательное | Описание |
|------|-----|--------------|----------|
| token | string | Да | Токен доступа, полученный при регистрации/логине |

**Успешный ответ (200 OK):**
```json
{
  "success": true,
  "message": "User data loaded successfully",
  "username": "player1",
  "currency": 1500,
  "currentRankIndex": 2,
  "totalClicks": 42,
  "purchasedHatIndices": [0, 1, 3],
  "equippedHatIndex": 1
}
```

**Поля ответа:**
| Поле | Тип | Описание |
|------|-----|----------|
| success | boolean | Статус операции |
| message | string | Сообщение о результате |
| username | string | Имя пользователя |
| currency | number | Игровая валюта пользователя |
| currentRankIndex | number | Текущий индекс ранга (уровня) |
| totalClicks | number | Общее количество кликов |
| purchasedHatIndices | number[] | Массив индексов купленных шляп |
| equippedHatIndex | number \| null | Индекс экипированной шляпы (null если не экипирована) |

**Возможные ошибки:**

**400 Bad Request** - Неверный формат:
```json
{
  "success": false,
  "message": "Invalid request format"
}
```

**400 Bad Request** - Токен не предоставлен:
```json
{
  "success": false,
  "message": "Token is required"
}
```

**401 Unauthorized** - Недействительный или истекший токен:
```json
{
  "success": false,
  "message": "Invalid or expired token"
}
```

**500 Internal Server Error** - Ошибка загрузки данных:
```json
{
  "success": false,
  "message": "Failed to load user data"
}
```

**Пример запроса (curl):**
```bash
curl -X POST http://localhost:6767/api/auth/loadUserData \
  -H "Content-Type: application/json" \
  -d '{
    "token": "cGxheWVyMToxNzM3NTg5MjM0NTY3ODkwMTIz"
  }'
```

**Пример запроса (JavaScript/Fetch):**
```javascript
const token = localStorage.getItem('authToken');

const response = await fetch('http://localhost:6767/api/auth/loadUserData', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({ token })
});

const userData = await response.json();
if (userData.success) {
  console.log('User data:', userData);
  // Используйте данные для отображения в UI
}
```

---

### 4. Сохранение данных пользователя

Сохраняет игровые данные аутентифицированного пользователя на сервер.

**Endpoint:** `POST /api/auth/saveUserData`

**Request Body:**
```json
{
  "token": "string",
  "currency": number,
  "currentRankIndex": number,
  "totalClicks": number,
  "purchasedHatIndices": number[],
  "equippedHatIndex": number | null
}
```

**Поля запроса:**
| Поле | Тип | Обязательное | Описание |
|------|-----|--------------|----------|
| token | string | Да | Токен доступа |
| currency | number | Да | Игровая валюта (>= 0) |
| currentRankIndex | number | Да | Текущий индекс ранга (>= 0) |
| totalClicks | number | Да | Общее количество кликов (>= 0) |
| purchasedHatIndices | number[] | Да | Массив индексов купленных шляп |
| equippedHatIndex | number \| null | Да | Индекс экипированной шляпы или null |

**Успешный ответ (200 OK):**
```json
{
  "success": true,
  "message": "User data saved successfully"
}
```

**Возможные ошибки:**

**400 Bad Request** - Неверный формат:
```json
{
  "success": false,
  "message": "Invalid request format"
}
```

**400 Bad Request** - Токен не предоставлен:
```json
{
  "success": false,
  "message": "Token is required"
}
```

**400 Bad Request** - Некорректные данные:
```json
{
  "success": false,
  "message": "Invalid data: negative values not allowed"
}
```

**401 Unauthorized** - Недействительный токен:
```json
{
  "success": false,
  "message": "Invalid or expired token"
}
```

**500 Internal Server Error** - Ошибка сохранения:
```json
{
  "success": false,
  "message": "Failed to save user data"
}
```

**Пример запроса (curl):**
```bash
curl -X POST http://localhost:6767/api/auth/saveUserData \
  -H "Content-Type: application/json" \
  -d '{
    "token": "cGxheWVyMToxNzM3NTg5MjM0NTY3ODkwMTIz",
    "currency": 2500,
    "currentRankIndex": 5,
    "totalClicks": 150,
    "purchasedHatIndices": [0, 1, 2, 5],
    "equippedHatIndex": 2
  }'
```

**Пример запроса (JavaScript/Fetch):**
```javascript
const token = localStorage.getItem('authToken');

// Данные игрока для сохранения
const gameData = {
  token: token,
  currency: 2500,
  currentRankIndex: 5,
  totalClicks: 150,
  purchasedHatIndices: [0, 1, 2, 5],
  equippedHatIndex: 2
};

const response = await fetch('http://localhost:6767/api/auth/saveUserData', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify(gameData)
});

const result = await response.json();
if (result.success) {
  console.log('Data saved successfully!');
}
```

**Важно:**
- Этот endpoint также автоматически обновляет таблицу лидеров (leaderboard)
- Все значения (currency, currentRankIndex, totalClicks) должны быть >= 0
- Массив purchasedHatIndices может быть пустым []
- equippedHatIndex может быть null если шляпа не экипирована

---

### 5. Загрузка таблицы лидеров

Получает топ-20 игроков по количеству валюты.

**Endpoint:** `POST /api/game/leaderboard`

**Request Body:**
```json
{}
```

**Поля запроса:**
Пустой объект (токен не требуется - публичный endpoint)

**Успешный ответ (200 OK):**
```json
{
  "success": true,
  "message": "Leaderboard loaded successfully",
  "leaderboard": [
    {
      "rank": 1,
      "username": "player1",
      "currency": 15000,
      "currentRankIndex": 10
    },
    {
      "rank": 2,
      "username": "player2",
      "currency": 12500,
      "currentRankIndex": 8
    },
    {
      "rank": 3,
      "username": "player3",
      "currency": 10000,
      "currentRankIndex": 7
    }
    // ... до 20 игроков
  ]
}
```

**Поля ответа:**
| Поле | Тип | Описание |
|------|-----|----------|
| success | boolean | Статус операции |
| message | string | Сообщение о результате |
| leaderboard | array | Массив игроков (до 20) |
| leaderboard[].rank | number | Позиция в топе (1-20) |
| leaderboard[].username | string | Имя игрока |
| leaderboard[].currency | number | Количество валюты |
| leaderboard[].currentRankIndex | number | Индекс ранга игрока |

**Возможные ошибки:**

**500 Internal Server Error** - Ошибка загрузки:
```json
{
  "success": false,
  "message": "Failed to load leaderboard"
}
```

**Пример запроса (curl):**
```bash
curl -X POST http://localhost:6767/api/game/leaderboard \
  -H "Content-Type: application/json" \
  -d '{}'
```

**Пример запроса (JavaScript/Fetch):**
```javascript
const response = await fetch('http://localhost:6767/api/game/leaderboard', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({})
});

const data = await response.json();
if (data.success) {
  console.log('Top players:', data.leaderboard);
  
  // Отобразить в UI
  data.leaderboard.forEach(player => {
    console.log(`${player.rank}. ${player.username} - ${player.currency} coins`);
  });
}
```

**Важно:**
- Endpoint публичный - токен не требуется
- Возвращает максимум 20 игроков
- Сортировка по currency (по убыванию)
- Если игроков меньше 20, вернется столько, сколько есть
- Пустой массив если нет игроков

---

## Модели данных

### UserData (игровые данные пользователя)

```typescript
interface UserData {
  username: string;           // Имя пользователя
  currency: number;           // Игровая валюта (>= 0)
  currentRankIndex: number;   // Текущий ранг (>= 0)
  totalClicks: number;        // Общее количество кликов (>= 0)
  purchasedHatIndices: number[]; // Массив индексов купленных шляп
  equippedHatIndex: number | null; // Индекс экипированной шляпы или null
}
```

### AuthResponse (ответ при регистрации/логине)

```typescript
interface AuthResponse {
  success: boolean;
  message: string;
  token: string;
}
```

### ErrorResponse (ответ при ошибке)

```typescript
interface ErrorResponse {
  success: boolean;
  message: string;
}
```

---

## Типичные сценарии использования

### Сценарий 1: Регистрация нового пользователя

```javascript
// 1. Регистрация
const registerResponse = await fetch('http://localhost:6767/api/auth/register', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    username: 'newplayer',
    password: 'securepass123'
  })
});

const registerData = await registerResponse.json();

if (registerData.success) {
  // 2. Сохранить токен
  localStorage.setItem('authToken', registerData.token);
  
  // 3. Загрузить данные пользователя
  const userDataResponse = await fetch('http://localhost:6767/api/auth/loadUserData', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ token: registerData.token })
  });
  
  const userData = await userDataResponse.json();
  console.log('User data:', userData);
  // Начальные значения:
  // currency: 0
  // currentRankIndex: 0
  // totalClicks: 0
  // purchasedHatIndices: []
  // equippedHatIndex: null
}
```

### Сценарий 2: Вход существующего пользователя

```javascript
// 1. Логин
const loginResponse = await fetch('http://localhost:6767/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    username: 'existingplayer',
    password: 'mypassword'
  })
});

const loginData = await loginResponse.json();

if (loginData.success) {
  // 2. Сохранить новый токен
  localStorage.setItem('authToken', loginData.token);
  
  // 3. Загрузить сохраненные данные
  const userDataResponse = await fetch('http://localhost:6767/api/auth/loadUserData', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ token: loginData.token })
  });
  
  const userData = await userDataResponse.json();
  // Данные пользователя с предыдущей сессии
}
```

### Сценарий 3: Игровой процесс с автосохранением

```javascript
// Игровое состояние
let gameState = {
  currency: 1500,
  currentRankIndex: 3,
  totalClicks: 75,
  purchasedHatIndices: [0, 1, 3],
  equippedHatIndex: 1
};

// Функция для сохранения прогресса
async function saveProgress() {
  const token = localStorage.getItem('authToken');
  
  const response = await fetch('http://localhost:6767/api/auth/saveUserData', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      token: token,
      ...gameState
    })
  });
  
  const result = await response.json();
  if (result.success) {
    console.log('Progress saved!');
  } else {
    console.error('Failed to save:', result.message);
  }
}

// Автосохранение каждые 30 секунд
setInterval(saveProgress, 30000);

// Сохранение при клике
function handleClick() {
  gameState.totalClicks++;
  gameState.currency += 10;
  
  // Сохранить после каждого 10-го клика
  if (gameState.totalClicks % 10 === 0) {
    saveProgress();
  }
}

// Сохранение при покупке шляпы
async function purchaseHat(hatIndex, cost) {
  if (gameState.currency >= cost) {
    gameState.currency -= cost;
    gameState.purchasedHatIndices.push(hatIndex);
    
    // Сразу сохранить после покупки
    await saveProgress();
  }
}

// Сохранение перед закрытием страницы
window.addEventListener('beforeunload', (e) => {
  saveProgress();
});
```

### Сценарий 4: Проверка токена при загрузке приложения

```javascript
// При загрузке приложения проверяем наличие токена
const token = localStorage.getItem('authToken');

if (token) {
  try {
    const response = await fetch('http://localhost:6767/api/auth/loadUserData', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ token })
    });
    
    const data = await response.json();
    
    if (data.success) {
      // Токен валиден, пользователь авторизован
      console.log('User authenticated:', data.username);
    } else {
      // Токен истек или невалиден
      localStorage.removeItem('authToken');
      // Перенаправить на страницу логина
    }
  } catch (error) {
    console.error('Auth check failed:', error);
    localStorage.removeItem('authToken');
  }
} else {
  // Токена нет, показать форму логина/регистрации
}
```

---

## Обработка ошибок

### Рекомендуемый подход

```javascript
async function apiRequest(endpoint, data) {
  try {
    const response = await fetch(`http://localhost:6767${endpoint}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data)
    });
    
    const result = await response.json();
    
    if (!result.success) {
      // Обработка ошибок API
      switch (response.status) {
        case 400:
          console.error('Validation error:', result.message);
          break;
        case 401:
          console.error('Authentication error:', result.message);
          localStorage.removeItem('authToken');
          // Перенаправить на логин
          break;
        case 409:
          console.error('Conflict:', result.message);
          break;
        case 500:
          console.error('Server error:', result.message);
          break;
        default:
          console.error('Unknown error:', result.message);
      }
    }
    
    return result;
  } catch (error) {
    console.error('Network error:', error);
    throw error;
  }
}

// Использование
const result = await apiRequest('/api/auth/login', {
  username: 'player1',
  password: 'pass123'
});

if (result.success) {
  console.log('Login successful!');
}
```

---

## Важные замечания

### Безопасность

1. **Токены истекают через 24 часа** - после истечения нужно повторно войти в систему
2. **Пароли хешируются** на сервере с использованием bcrypt
3. **Токены уникальны** - каждый логин генерирует новый токен
4. **HTTPS рекомендуется** для production окружения

### Валидация на клиенте

Рекомендуется добавить валидацию перед отправкой запросов:

```javascript
function validateUsername(username) {
  if (!username || username.length < 3 || username.length > 50) {
    return 'Username must be 3-50 characters';
  }
  return null;
}

function validatePassword(password) {
  if (!password || password.length < 6) {
    return 'Password must be at least 6 characters';
  }
  return null;
}
```

### CORS

Если фронтенд работает на другом домене/порту, убедитесь что сервер настроен для CORS.

---

## Примеры интеграции

### React Example

```jsx
import { useState, useEffect } from 'react';

function App() {
  const [userData, setUserData] = useState(null);
  const [token, setToken] = useState(localStorage.getItem('authToken'));

  useEffect(() => {
    if (token) {
      loadUserData();
    }
  }, [token]);

  async function loadUserData() {
    const response = await fetch('http://localhost:6767/api/auth/loadUserData', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ token })
    });
    
    const data = await response.json();
    if (data.success) {
      setUserData(data);
    } else {
      setToken(null);
      localStorage.removeItem('authToken');
    }
  }

  async function saveUserData(gameData) {
    const response = await fetch('http://localhost:6767/api/auth/saveUserData', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        token,
        ...gameData
      })
    });
    
    const result = await response.json();
    if (result.success) {
      console.log('Data saved!');
    }
  }

  async function handleLogin(username, password) {
    const response = await fetch('http://localhost:6767/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password })
    });
    
    const data = await response.json();
    if (data.success) {
      setToken(data.token);
      localStorage.setItem('authToken', data.token);
    }
  }

  function handleClick() {
    const newData = {
      ...userData,
      totalClicks: userData.totalClicks + 1,
      currency: userData.currency + 10
    };
    setUserData(newData);
    
    // Автосохранение каждые 10 кликов
    if (newData.totalClicks % 10 === 0) {
      saveUserData(newData);
    }
  }

  return (
    <div>
      {userData ? (
        <div>
          <h1>Welcome, {userData.username}!</h1>
          <p>Currency: {userData.currency}</p>
          <p>Rank: {userData.currentRankIndex}</p>
          <p>Total Clicks: {userData.totalClicks}</p>
          <button onClick={handleClick}>Click!</button>
          <button onClick={() => saveUserData(userData)}>Save Progress</button>
        </div>
      ) : (
        <LoginForm onLogin={handleLogin} />
      )}
    </div>
  );
}
```
          <p>Currency: {userData.currency}</p>
          <p>Rank: {userData.currentRankIndex}</p>
          <p>Total Clicks: {userData.totalClicks}</p>
        </div>
      ) : (
        <LoginForm onLogin={handleLogin} />
      )}
    </div>
  );
}
```

### Vue Example

```vue
<template>
  <div>
    <div v-if="userData">
      <h1>Welcome, {{ userData.username }}!</h1>
      <p>Currency: {{ userData.currency }}</p>
      <p>Rank: {{ userData.currentRankIndex }}</p>
      <p>Total Clicks: {{ userData.totalClicks }}</p>
      <button @click="handleClick">Click!</button>
      <button @click="saveProgress">Save Progress</button>
    </div>
    <LoginForm v-else @login="handleLogin" />
  </div>
</template>

<script>
export default {
  data() {
    return {
      userData: null,
      token: localStorage.getItem('authToken')
    };
  },
  mounted() {
    if (this.token) {
      this.loadUserData();
    }
  },
  methods: {
    async loadUserData() {
      const response = await fetch('http://localhost:6767/api/auth/loadUserData', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ token: this.token })
      });
      
      const data = await response.json();
      if (data.success) {
        this.userData = data;
      }
    },
    async saveProgress() {
      const response = await fetch('http://localhost:6767/api/auth/saveUserData', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          token: this.token,
          currency: this.userData.currency,
          currentRankIndex: this.userData.currentRankIndex,
          totalClicks: this.userData.totalClicks,
          purchasedHatIndices: this.userData.purchasedHatIndices,
          equippedHatIndex: this.userData.equippedHatIndex
        })
      });
      
      const result = await response.json();
      if (result.success) {
        console.log('Progress saved!');
      }
    },
    async handleLogin(username, password) {
      const response = await fetch('http://localhost:6767/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password })
      });
      
      const data = await response.json();
      if (data.success) {
        this.token = data.token;
        localStorage.setItem('authToken', data.token);
        this.loadUserData();
      }
    },
    handleClick() {
      this.userData.totalClicks++;
      this.userData.currency += 10;
      
      // Автосохранение каждые 10 кликов
      if (this.userData.totalClicks % 10 === 0) {
        this.saveProgress();
      }
    }
  }
};
</script>
```

---

## Тестирование API

Для тестирования API можно использовать:

1. **curl** (примеры выше)
2. **Postman** - импортируйте коллекцию запросов
3. **Thunder Client** (VS Code extension)
4. **Insomnia**

### Postman Collection

Создайте коллекцию с тремя запросами:

1. **Register**
   - Method: POST
   - URL: `http://localhost:6767/api/auth/register`
   - Body (raw JSON):
     ```json
     {
       "username": "testuser",
       "password": "testpass123"
     }
     ```

2. **Login**
   - Method: POST
   - URL: `http://localhost:6767/api/auth/login`
   - Body (raw JSON):
     ```json
     {
       "username": "testuser",
       "password": "testpass123"
     }
     ```

3. **Load User Data**
   - Method: POST
   - URL: `http://localhost:6767/api/auth/loadUserData`
   - Body (raw JSON):
     ```json
     {
       "token": "{{token}}"
     }
     ```

---

## Поддержка

При возникновении проблем проверьте:

1. Сервер запущен на порту 6767
2. PostgreSQL доступна
3. Миграции применены
4. Правильный формат JSON в запросах
5. Content-Type заголовок установлен

Для отладки смотрите логи сервера в консоли.
