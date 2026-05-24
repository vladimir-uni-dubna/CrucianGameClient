using System;
using System.Collections.Generic;
using CrucianGame.Save;

namespace CrucianGame.Models
{
    /// <summary>
    /// Главный класс, управляющий игровой логикой.
    /// Связывает модель (Crucian, Shop) с интерфейсом.
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

        public GameManager()
        {
            Shop = new Shop();
            Crucian = new Crucian();
        }

        /// <summary>
        /// Загрузить сохранение или начать новую игру
        /// </summary>
        public void StartNewOrLoadGame()
        {
            SaveManager.SaveData? data = SaveManager.Load();

            if (data != null)
            {
                // Загружаем сохранённые данные
                Crucian.Currency = data.Currency;
                Crucian.CurrentRankIndex = data.CurrentRankIndex;
                Crucian.TotalClicks = data.TotalClicks;
                Crucian.PurchasedHatIndices = data.PurchasedHatIndices ?? new List<int>();

                // Восстанавливаем надетую шляпу
                if (data.EquippedHatIndex >= 0 && data.EquippedHatIndex < Shop.AvailableHats.Count)
                {
                    Crucian.EquippedHat = Shop.AvailableHats[data.EquippedHatIndex];
                }
            }
            // Если сохранения нет — начинаем с начальными значениями (уже установлены в конструкторе)

            // Оповещаем интерфейс
            OnCurrencyChanged?.Invoke(Crucian.Currency);
            OnTotalClicksChanged?.Invoke(Crucian.TotalClicks);
            OnRankUp?.Invoke(Crucian.CurrentRank);
            OnHatChanged?.Invoke();
        }

        /// <summary>
        /// Обработать клик по карасю
        /// </summary>
        public void HandleClick()
        {
            long earned = Crucian.Click();

            // Проверяем повышение звания
            bool rankedUp = Crucian.TryRankUp();
            if (rankedUp)
            {
                OnRankUp?.Invoke(Crucian.CurrentRank);
            }

            // Оповещаем интерфейс
            OnCurrencyChanged?.Invoke(Crucian.Currency);
            OnTotalClicksChanged?.Invoke(Crucian.TotalClicks);

            // Автосохранение после каждого действия
            SaveGame();
        }

        /// <summary>
        /// Купить шляпу
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
                SaveGame();
            }
            return success;
        }

        /// <summary>
        /// Надеть шляпу
        /// </summary>
        public void EquipHat(int hatIndex)
        {
            Hat? hat = Shop.GetHatByIndex(hatIndex);
            if (hat == null) return;

            // Если эта шляпа уже надета — снимаем её
            if (Crucian.EquippedHat == hat)
            {
                Crucian.EquippedHat = null;
            }
            else
            {
                Shop.Equip(hat, Crucian);
            }

            OnHatChanged?.Invoke();
            SaveGame();
        }

        /// <summary>
        /// Сохранить игру
        /// </summary>
        public void SaveGame()
        {
            SaveManager.Save(Crucian);
        }
    }
}
