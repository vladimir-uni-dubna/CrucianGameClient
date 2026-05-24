using System;
using System.Threading.Tasks;
using System.Timers;
using CrucianGame.Save;
using CrucianGame.Services;
using Timer = System.Timers.Timer;

namespace CrucianGame.Models
{
    /// <summary>
    /// Главный класс, управляющий игровой логикой.
    /// Связывает модель (Crucian, Shop) с интерфейсом и сервером.
    /// </summary>
    public class GameManager
    {
        /// <summary>
        /// Текущий карась игрока
        /// </summary>
        public Crucian Crucian { get; private set; }

        /// <summary>
        /// Магазин шляп
        /// </summary>
        public Shop Shop { get; private set; }

        /// <summary>
        /// Клиент для общения с сервером
        /// </summary>
        public ApiClient ApiClient { get; private set; }

        /// <summary>
        /// Менеджер сохранений
        /// </summary>
        private SaveManager _saveManager;

        /// <summary>
        /// Таймер автосохранения (30 секунд)
        /// </summary>
        private Timer _autoSaveTimer;

        /// <summary>
        /// Флаг — были ли изменения с последнего сохранения
        /// </summary>
        private bool _hasUnsavedChanges;

        /// <summary>
        /// Событие: обновилась валюта
        /// </summary>
        public event Action<long>? OnCurrencyChanged;

        /// <summary>
        /// Событие: повысилось звание
        /// </summary>
        public event Action<Rank>? OnRankUp;

        /// <summary>
        /// Событие: изменилась шляпа (купили/надели/сняли)
        /// </summary>
        public event Action? OnHatChanged;

        /// <summary>
        /// Событие: изменилось количество кликов
        /// </summary>
        public event Action<long>? OnTotalClicksChanged;

        /// <summary>
        /// Событие: ошибка сохранения
        /// </summary>
        public event Action<string>? OnSaveError;

        public GameManager(ApiClient apiClient)
        {
            ApiClient = apiClient;
            Shop = new Shop();
            Crucian = new Crucian();
            _saveManager = new SaveManager(apiClient);

            // Запускаем таймер автосохранения каждые 30 секунд
            _autoSaveTimer = new Timer(30000);
            _autoSaveTimer.Elapsed += async (sender, args) =>
            {
                try { await AutoSave(); }
                catch { /* игнорируем ошибки автосохранения */ }
            };
            _autoSaveTimer.AutoReset = true;
            _autoSaveTimer.Start();
        }

        /// <summary>
        /// Загрузить сохранение или начать новую игру
        /// </summary>
        public async Task StartNewOrLoadGame()
        {
            bool loaded = await _saveManager.Load(Crucian);

            if (!loaded)
            {
                Crucian = new Crucian();
            }

            OnCurrencyChanged?.Invoke(Crucian.Currency);
            OnTotalClicksChanged?.Invoke(Crucian.TotalClicks);
            OnRankUp?.Invoke(Crucian.CurrentRank);
            OnHatChanged?.Invoke();

            _hasUnsavedChanges = false;
        }

        /// <summary>
        /// Обработать клик по карасю (синхронно, без ожидания сохранения)
        /// </summary>
        public void HandleClick()
        {
            Crucian.Click();

            if (Crucian.TryRankUp())
            {
                OnRankUp?.Invoke(Crucian.CurrentRank);
                _ = SaveGame(); // fire-and-forget
            }

            OnCurrencyChanged?.Invoke(Crucian.Currency);
            OnTotalClicksChanged?.Invoke(Crucian.TotalClicks);
            _hasUnsavedChanges = true;
        }

        /// <summary>
        /// Купить шляпу (синхронно, без ожидания сохранения)
        /// </summary>
        public bool BuyHat(int hatIndex)
        {
            Hat? hat = Shop.GetHatByIndex(hatIndex);
            if (hat == null) return false;

            bool success = Shop.TryBuy(hat, Crucian);
            if (success)
            {
                OnCurrencyChanged?.Invoke(Crucian.Currency);
                OnHatChanged?.Invoke();
                _ = SaveGame(); // fire-and-forget
            }
            return success;
        }

        /// <summary>
        /// Надеть шляпу (синхронно, без ожидания сохранения)
        /// </summary>
        public void EquipHat(int hatIndex)
        {
            Hat? hat = Shop.GetHatByIndex(hatIndex);
            if (hat == null) return;

            if (Crucian.EquippedHat == hat)
            {
                Crucian.EquippedHat = null;
            }
            else
            {
                Shop.Equip(hat, Crucian);
            }

            OnHatChanged?.Invoke();
            _ = SaveGame(); // fire-and-forget
        }

        /// <summary>
        /// Сохранить игру на сервер
        /// </summary>
        public async Task SaveGame()
        {
            bool success = await _saveManager.Save(Crucian);
            if (success)
            {
                _hasUnsavedChanges = false;
            }
            else
            {
                OnSaveError?.Invoke("Не удалось сохранить прогресс");
            }
        }

        /// <summary>
        /// Автосохранение по таймеру
        /// </summary>
        private async Task AutoSave()
        {
            if (_hasUnsavedChanges)
            {
                await SaveGame();
            }
        }

        /// <summary>
        /// Сохранить при закрытии игры
        /// </summary>
        public async Task SaveOnExit()
        {
            _autoSaveTimer.Stop();
            await SaveGame();
        }
    }
}
