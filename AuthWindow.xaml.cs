using System.Windows;
using System.Windows.Controls;
using CrucianGame.Services;

namespace CrucianGame
{
    /// <summary>
    /// Окно авторизации и регистрации
    /// </summary>
    public partial class AuthWindow : Window
    {
        /// <summary>
        /// Режим окна: true — регистрация, false — вход
        /// </summary>
        private bool _isRegisterMode;

        /// <summary>
        /// API-клиент для общения с сервером
        /// </summary>
        public ApiClient ApiClient { get; private set; }

        public AuthWindow()
        {
            InitializeComponent();
            ApiClient = new ApiClient();
            SetLoginMode();
        }

        /// <summary>
        /// Переключить в режим входа
        /// </summary>
        private void SetLoginMode()
        {
            _isRegisterMode = false;
            ActionButton.Content = "Войти";
            ToggleModeText.Text = "Нет аккаунта? Зарегистрироваться";
            ErrorText.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Переключить в режим регистрации
        /// </summary>
        private void SetRegisterMode()
        {
            _isRegisterMode = true;
            ActionButton.Content = "Зарегистрироваться";
            ToggleModeText.Text = "Уже есть аккаунт? Войти";
            ErrorText.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Переключение между входом и регистрацией
        /// </summary>
        private void OnToggleModeClick(object sender, RoutedEventArgs e)
        {
            if (_isRegisterMode)
                SetLoginMode();
            else
                SetRegisterMode();
        }

        /// <summary>
        /// Нажатие на кнопку действия (Войти / Зарегистрироваться)
        /// </summary>
        private async void OnActionButtonClick(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password;

            // Валидация
            string? validationError = Validate(username, password);
            if (validationError != null)
            {
                ShowError(validationError);
                return;
            }

            // Блокируем кнопку, чтобы не было двойного нажатия
            ActionButton.IsEnabled = false;
            ActionButton.Content = "Подождите...";
            ErrorText.Visibility = Visibility.Collapsed;

            AuthResult result;
            if (_isRegisterMode)
                result = await ApiClient.Register(username, password);
            else
                result = await ApiClient.Login(username, password);

            ActionButton.IsEnabled = true;

            if (result.Success)
            {
                // Успех — закрываем окно авторизации (DialogResult сам закроет окно)
                DialogResult = true;
            }
            else
            {
                ShowError(result.Message ?? "Неизвестная ошибка");
            }
        }

        /// <summary>
        /// Проверка полей на валидность
        /// </summary>
        private string? Validate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                return "Введите имя пользователя";
            if (username.Length < 3)
                return "Имя пользователя должно быть от 3 символов";
            if (username.Length > 50)
                return "Имя пользователя должно быть до 50 символов";
            if (string.IsNullOrWhiteSpace(password))
                return "Введите пароль";
            if (password.Length < 6)
                return "Пароль должен быть минимум 6 символов";
            return null;
        }

        /// <summary>
        /// Показать ошибку
        /// </summary>
        private void ShowError(string? message)
        {
            ErrorText.Text = message ?? "Неизвестная ошибка";
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}
