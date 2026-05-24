using System.Collections.Generic;

namespace CrucianGame.Models
{
    /// <summary>
    /// Класс, представляющий звание карася
    /// </summary>
    public class Rank
    {
        /// <summary>
        /// Название звания (например "Икринка")
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Сколько червячков даёт один клик на этом ранге
        /// </summary>
        public int ClicksPerHit { get; }

        /// <summary>
        /// Сколько червячков нужно накопить, чтобы перейти на этот ранг
        /// </summary>
        public long RequiredCurrency { get; }

        public Rank(string name, int clicksPerHit, long requiredCurrency)
        {
            Name = name;
            ClicksPerHit = clicksPerHit;
            RequiredCurrency = requiredCurrency;
        }

        /// <summary>
        /// Статический список всех званий (прогрессия)
        /// </summary>
        public static List<Rank> GetAllRanks()
        {
            return new List<Rank>
            {
                new Rank("Икринка", 1, 100),
                new Rank("Малёк", 2, 1000),
                new Rank("Карасёнок", 16, 10000),
                new Rank("Карась-студент", 80, 100000),
                new Rank("Бывалый карась", 500, 1000000),
                new Rank("Карась-качок", 2500, 50000000),
                new Rank("Золотой карась", 5000, 250000000),
                new Rank("Карась-авторитет", 10000, 1000000000),
                new Rank("Великий карась", 25000, 5000000000),
                new Rank("Бог пруда", 50000, 10000000000)
            };
        }
    }
}
