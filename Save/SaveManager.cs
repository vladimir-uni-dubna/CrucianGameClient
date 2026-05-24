using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using CrucianGame.Models;

namespace CrucianGame.Save
{
    /// <summary>
    /// Класс для сохранения и загрузки прогресса игры в JSON
    /// </summary>
    public class SaveManager
    {
        /// <summary>
        /// Структура данных для сериализации в JSON
        /// </summary>
        public class SaveData
        {
            public long Currency { get; set; }
            public int CurrentRankIndex { get; set; }
            public long TotalClicks { get; set; }
            public List<int> PurchasedHatIndices { get; set; } = new List<int>();
            public int EquippedHatIndex { get; set; } = -1; // -1 означает, что шляпа не надета
        }

        /// <summary>
        /// Путь к файлу сохранения (рядом с .exe)
        /// </summary>
        private static string SavePath => Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "save.json");

        /// <summary>
        /// Сохранить прогресс игры в JSON-файл
        /// </summary>
        public static void Save(Crucian crucian)
        {
            var data = new SaveData
            {
                Currency = crucian.Currency,
                CurrentRankIndex = crucian.CurrentRankIndex,
                TotalClicks = crucian.TotalClicks,
                PurchasedHatIndices = crucian.PurchasedHatIndices,
                EquippedHatIndex = -1
            };

            // Определяем индекс надетой шляпы
            if (crucian.EquippedHat != null)
            {
                var shop = new Shop();
                int idx = shop.AvailableHats.IndexOf(crucian.EquippedHat);
                data.EquippedHatIndex = idx;
            }

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SavePath, json);
        }

        /// <summary>
        /// Загрузить прогресс игры из JSON-файла.
        /// Возвращает SaveData или null, если файла нет.
        /// </summary>
        public static SaveData? Load()
        {
            if (!File.Exists(SavePath))
                return null;

            string json = File.ReadAllText(SavePath);
            return JsonSerializer.Deserialize<SaveData>(json);
        }
    }
}
