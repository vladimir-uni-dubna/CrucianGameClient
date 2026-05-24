using System.Collections.Generic;

namespace CrucianGame.Models
{
    /// <summary>
    /// Класс, представляющий магазин шляп
    /// </summary>
    public class Shop
    {
        /// <summary>
        /// Список всех доступных шляп в магазине
        /// </summary>
        public List<Hat> AvailableHats { get; }

        public Shop()
        {
            AvailableHats = new List<Hat>
            {
                new Hat("Кепка", 100, 6.0, "assets/hat_cap.png"),
                new Hat("Соломенная шляпа", 3000, 8.0, "assets/hat_straw.png"),
                new Hat("Цилиндр", 30000, 12.0, "assets/hat_tophat.png"),
                new Hat("Корона", 300000, 20.0, "assets/hat_crown.png"),
                new Hat("Каска", 3000000, 32.0, "assets/hat_helmet.png")
            };
        }

        /// <summary>
        /// Попробовать купить шляпу.
        /// Списание валюты происходит здесь.
        /// Возвращает true, если покупка успешна.
        /// </summary>
        public bool TryBuy(Hat hat, Crucian crucian)
        {
            // Проверяем, хватает ли денег
            if (crucian.Currency < hat.Price)
                return false;

            // Проверяем, не куплена ли уже эта шляпа (по индексу)
            int hatIndex = AvailableHats.IndexOf(hat);
            if (hatIndex >= 0 && crucian.PurchasedHatIndices.Contains(hatIndex))
                return false;

            // Списываем валюту
            crucian.Currency -= hat.Price;

            // Добавляем шляпу в купленные
            if (hatIndex >= 0)
            {
                crucian.PurchasedHatIndices.Add(hatIndex);
            }

            return true;
        }

        /// <summary>
        /// Надеть шляпу на карася
        /// </summary>
        public void Equip(Hat hat, Crucian crucian)
        {
            crucian.EquippedHat = hat;
        }

        /// <summary>
        /// Получить шляпу по индексу
        /// </summary>
        public Hat? GetHatByIndex(int index)
        {
            if (index >= 0 && index < AvailableHats.Count)
                return AvailableHats[index];
            return null;
        }
    }
}
