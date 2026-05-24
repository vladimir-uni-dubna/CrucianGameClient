using System.Windows;
using CrucianGame.Services;

namespace CrucianGame
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Показываем окно авторизации
            var authWindow = new AuthWindow();
            bool? authResult = authWindow.ShowDialog();

            if (authResult == true)
            {
                // Авторизация успешна — открываем главное окно
                var mainWindow = new MainWindow(authWindow.ApiClient);
                mainWindow.Show();
            }
            else
            {
                // Пользователь закрыл окно авторизации — выходим
                Shutdown();
            }
        }
    }
}
