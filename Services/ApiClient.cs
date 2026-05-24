using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CrucianGame.Services
{
    /// <summary>
    /// HTTP-клиент для общения с сервером авторизации и сохранения
    /// </summary>
    public class ApiClient
    {
        private static readonly string BaseUrl = "http://176.109.110.146:6767";
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Токен доступа, полученный при регистрации/логине
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// Имя текущего пользователя
        /// </summary>
        public string? CurrentUser { get; set; }

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        public async Task<AuthResult> Register(string username, string password)
        {
            try
            {
                var body = new { username, password };
                string json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(
                    $"{BaseUrl}/api/auth/register", content);

                string responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AuthResult>(responseBody, JsonOptions);

                if (result != null && result.Success)
                {
                    Token = result.Token;
                    CurrentUser = username;
                }

                return result ?? new AuthResult { Success = false, Message = "Ошибка ответа сервера" };
            }
            catch (Exception ex)
            {
                return new AuthResult { Success = false, Message = $"Ошибка соединения: {ex.Message}" };
            }
        }

        /// <summary>
        /// Вход в систему
        /// </summary>
        public async Task<AuthResult> Login(string username, string password)
        {
            try
            {
                var body = new { username, password };
                string json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(
                    $"{BaseUrl}/api/auth/login", content);

                string responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AuthResult>(responseBody, JsonOptions);

                if (result != null && result.Success)
                {
                    Token = result.Token;
                    CurrentUser = username;
                }

                return result ?? new AuthResult { Success = false, Message = "Ошибка ответа сервера" };
            }
            catch (Exception ex)
            {
                return new AuthResult { Success = false, Message = $"Ошибка соединения: {ex.Message}" };
            }
        }

        /// <summary>
        /// Загрузить игровые данные пользователя с сервера
        /// </summary>
        public async Task<UserDataResult> LoadUserData()
        {
            try
            {
                var body = new { token = Token };
                string json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(
                    $"{BaseUrl}/api/auth/loadUserData", content);

                string responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<UserDataResult>(responseBody, JsonOptions);

                return result ?? new UserDataResult { Success = false, Message = "Ошибка ответа сервера" };
            }
            catch (Exception ex)
            {
                return new UserDataResult { Success = false, Message = $"Ошибка соединения: {ex.Message}" };
            }
        }

        /// <summary>
        /// Сохранить игровые данные пользователя на сервер
        /// </summary>
        public async Task<bool> SaveUserData(long currency, int currentRankIndex, long totalClicks,
            System.Collections.Generic.List<int> purchasedHatIndices, int equippedHatIndex)
        {
            try
            {
                var body = new
                {
                    token = Token,
                    currency,
                    currentRankIndex,
                    totalClicks,
                    purchasedHatIndices,
                    equippedHatIndex
                };
                string json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(
                    $"{BaseUrl}/api/auth/saveUserData", content);

                string responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AuthResult>(responseBody, JsonOptions);

                return result?.Success ?? false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Загрузить таблицу лидеров (топ-20 игроков)
        /// </summary>
        public async Task<LeaderboardResult> GetLeaderboard()
        {
            try
            {
                var body = new { };
                string json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(
                    $"{BaseUrl}/api/game/leaderboard", content);

                string responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<LeaderboardResult>(responseBody, JsonOptions);

                return result ?? new LeaderboardResult { Success = false, Message = "Ошибка ответа сервера" };
            }
            catch (Exception ex)
            {
                return new LeaderboardResult { Success = false, Message = $"Ошибка соединения: {ex.Message}" };
            }
        }
    }

    /// <summary>
    /// Результат регистрации или входа
    /// </summary>
    public class AuthResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
    }

    /// <summary>
    /// Результат загрузки данных пользователя
    /// </summary>
    public class UserDataResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Username { get; set; }
        public long Currency { get; set; }
        public int CurrentRankIndex { get; set; }
        public long TotalClicks { get; set; }
        public System.Collections.Generic.List<int>? PurchasedHatIndices { get; set; }
        public int EquippedHatIndex { get; set; } = -1;
    }

    /// <summary>
    /// Результат загрузки таблицы лидеров
    /// </summary>
    public class LeaderboardResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public System.Collections.Generic.List<LeaderboardEntry>? Leaderboard { get; set; }
    }

    /// <summary>
    /// Запись в таблице лидеров
    /// </summary>
    public class LeaderboardEntry
    {
        public int Rank { get; set; }
        public string? Username { get; set; }
        public long Currency { get; set; }
        public int CurrentRankIndex { get; set; }
    }
}
