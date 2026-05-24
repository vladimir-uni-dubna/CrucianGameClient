namespace CrucianGame.Models
{
    /// <summary>
    /// Класс, представляющий шляпу в магазине
    /// </summary>
    public class Hat
    {
        /// <summary>
        /// Название шляпы (например "Кепка")
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Цена шляпы в червячках
        /// </summary>
        public long Price { get; }

        /// <summary>
        /// Множитель заработка, который даёт шляпа (например 1.5)
        /// </summary>
        public double Multiplier { get; }

        /// <summary>
        /// Путь к PNG-изображению шляпы относительно папки assets
        /// </summary>
        public string ImagePath { get; }

        public Hat(string name, long price, double multiplier, string imagePath)
        {
            Name = name;
            Price = price;
            Multiplier = multiplier;
            ImagePath = imagePath;
        }
    }
}
