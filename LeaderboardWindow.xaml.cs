using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CrucianGame.Models;
using CrucianGame.Services;

namespace CrucianGame
{
    /// <summary>
    /// Окно с таблицей лидеров
    /// </summary>
    public partial class LeaderboardWindow : Window
    {
        private readonly ApiClient _apiClient;
        private readonly Rank[] _ranks;

        public LeaderboardWindow(ApiClient apiClient)
        {
            InitializeComponent();
            _apiClient = apiClient;
            _ranks = Rank.GetAllRanks().ToArray();
            Loaded += async (s, e) => await LoadLeaderboard();
        }

        /// <summary>
        /// Загрузить и отобразить таблицу лидеров
        /// </summary>
        private async System.Threading.Tasks.Task LoadLeaderboard()
        {
            LeaderboardList.ItemsSource = null;

            var result = await _apiClient.GetLeaderboard();

            if (result.Success && result.Leaderboard != null)
            {
                var items = result.Leaderboard.Select(entry => new LeaderboardItem
                {
                    Rank = entry.Rank,
                    Username = entry.Username ?? "???",
                    RankName = entry.CurrentRankIndex >= 0 && entry.CurrentRankIndex < _ranks.Length
                        ? _ranks[entry.CurrentRankIndex].Name
                        : "Неизвестно",
                    CurrencyDisplay = $"{entry.Currency:N0}"
                }).ToList();

                LeaderboardList.ItemsSource = new ObservableCollection<LeaderboardItem>(items);
            }
            else
            {
                MessageBox.Show(this, result.Message ?? "Не удалось загрузить таблицу лидеров",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        /// <summary>
        /// Закрыть окно
        /// </summary>
        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Модель для отображения записи в таблице лидеров
        /// </summary>
        public class LeaderboardItem
        {
            public int Rank { get; set; }
            public string Username { get; set; } = "";
            public string RankName { get; set; } = "";
            public string CurrencyDisplay { get; set; } = "";
        }
    }
}
