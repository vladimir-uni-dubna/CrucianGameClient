using System.Collections.Generic;
using System.Threading.Tasks;
using CrucianGame.Models;
using CrucianGame.Services;

namespace CrucianGame.Save
{
    /// <summary>
    /// Класс для сохранения и загрузки прогресса игры через сервер
    /// </summary>
    public class SaveManager
    {
        private readonly ApiClient _apiClient;

        public SaveManager(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        /// <summary>
        /// Сохранить прогресс игры на сервер
        /// </summary>
        public async Task<bool> Save(Crucian crucian)
        {
            // Определяем индекс надетой шляпы
            int equippedHatIndex = -1;
            if (crucian.EquippedHat != null)
            {
                var shop = new Shop();
                int idx = shop.AvailableHats.IndexOf(crucian.EquippedHat);
                equippedHatIndex = idx;
            }

            return await _apiClient.SaveUserData(
                crucian.Currency,
                crucian.CurrentRankIndex,
                crucian.TotalClicks,
                crucian.PurchasedHatIndices,
                equippedHatIndex
            );
        }

        /// <summary>
        /// Загрузить прогресс игры с сервера
        /// Возвращает true, если данные загружены
        /// </summary>
        public async Task<bool> Load(Crucian crucian)
        {
            UserDataResult data = await _apiClient.LoadUserData();

            if (data == null || !data.Success)
                return false;

            // Загружаем данные из ответа сервера
            crucian.Currency = data.Currency;
            crucian.CurrentRankIndex = data.CurrentRankIndex;
            crucian.TotalClicks = data.TotalClicks;
            crucian.PurchasedHatIndices = data.PurchasedHatIndices ?? new List<int>();

            // Восстанавливаем надетую шляпу
            var shop = new Shop();
            if (data.EquippedHatIndex >= 0 && data.EquippedHatIndex < shop.AvailableHats.Count)
            {
                crucian.EquippedHat = shop.AvailableHats[data.EquippedHatIndex];
            }
            else
            {
                crucian.EquippedHat = null;
            }

            return true;
        }
    }
}
