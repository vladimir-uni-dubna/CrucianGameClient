using System.Collections.Generic;

namespace CrucianGame.Models
{
    /// <summary>
    /// Класс, представляющий карася (игрока)
    /// </summary>
    public class Crucian
    {
        /// <summary>
        /// Текущее количество червячков (валюты)
        /// </summary>
        public long Currency { get; set; }

        /// <summary>
        /// Индекс текущего звания в списке рангов
        /// </summary>
        public int CurrentRankIndex { get; set; }

        /// <summary>
        /// Общее количество кликов за всю игру
        /// </summary>
        public long TotalClicks { get; set; }

        /// <summary>
        /// Текущая наделая шляпа (может быть null)
        /// </summary>
        public Hat? EquippedHat { get; set; }

        /// <summary>
        /// Список индексов купленных шляп
        /// </summary>
        public List<int> PurchasedHatIndices { get; set; }

        /// <summary>
        /// Текущее звание (вычисляется по индексу)
        /// </summary>
        public Rank CurrentRank => Rank.GetAllRanks()[CurrentRankIndex];

        public Crucian()
        {
            Currency = 0;
            CurrentRankIndex = 0;
            TotalClicks = 0;
            EquippedHat = null;
            PurchasedHatIndices = new List<int>();
        }

        /// <summary>
        /// Выполнить клик по карасю.
        /// Возвращает количество заработанных червячков с учётом звания и шляпы.
        /// </summary>
        public long Click()
        {
            TotalClicks++;

            // Базовый заработок от текущего звания
            double earnings = CurrentRank.ClicksPerHit;

            // Умножаем на множитель от надетой шляпы (если она есть)
            if (EquippedHat != null)
            {
                earnings *= EquippedHat.Multiplier;
            }

            long finalEarnings = (long)earnings;
            Currency += finalEarnings;
            return finalEarnings;
        }

        /// <summary>
        /// Попробовать повысить звание.
        /// Возвращает true, если звание повышено.
        /// </summary>
        public bool TryRankUp()
        {
            List<Rank> ranks = Rank.GetAllRanks();

            // Если уже максимальное звание — повышать некуда
            if (CurrentRankIndex >= ranks.Count - 1)
                return false;

            Rank nextRank = ranks[CurrentRankIndex + 1];

            // Проверяем, хватает ли валюты для перехода
            if (Currency >= nextRank.RequiredCurrency)
            {
                CurrentRankIndex++;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Получить максимальное количество званий
        /// </summary>
        public int MaxRankIndex => Rank.GetAllRanks().Count - 1;
    }
}
