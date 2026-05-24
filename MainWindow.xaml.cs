using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using CrucianGame.Models;

namespace CrucianGame
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Игровой менеджер, управляющий всей логикой игры
        /// </summary>
        private GameManager _gameManager;

        public MainWindow()
        {
            InitializeComponent();

            // Создаём игровой менеджер
            _gameManager = new GameManager();

            // Подписываемся на события менеджера
            _gameManager.OnCurrencyChanged += UpdateCurrencyDisplay;
            _gameManager.OnRankUp += UpdateRankDisplay;
            _gameManager.OnHatChanged += UpdateHatDisplay;
            _gameManager.OnTotalClicksChanged += UpdateTotalClicksDisplay;

            // Строим интерфейс магазина
            BuildShopUI();

            // Загружаем сохранение или начинаем новую игру
            _gameManager.StartNewOrLoadGame();
        }

        /// <summary>
        /// Построить интерфейс магазина шляп в нижней панели
        /// </summary>
        private void BuildShopUI()
        {
            HatsPanel.Children.Clear();

            for (int i = 0; i < _gameManager.Shop.AvailableHats.Count; i++)
            {
                Hat hat = _gameManager.Shop.AvailableHats[i];
                int index = i; // копируем для замыкания

                // Рамка для каждой шляпы
                Border border = new Border
                {
                    Style = (Style)FindResource("HatBorderStyle"),
                    Width = 130
                };

                // Вертикальный контейнер: картинка, название, цена, кнопка
                StackPanel stack = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };

                // Изображение шляпы
                Image hatImage = new Image
                {
                    Source = new BitmapImage(new Uri(hat.ImagePath, UriKind.Relative)),
                    Width = 50,
                    Height = 50,
                    Margin = new Thickness(0, 0, 0, 5),
                    Stretch = Stretch.Uniform
                };
                stack.Children.Add(hatImage);

                // Название шляпы
                TextBlock nameText = new TextBlock
                {
                    Text = hat.Name,
                    FontWeight = FontWeights.Bold,
                    FontSize = 13,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                stack.Children.Add(nameText);

                // Цена шляпы
                TextBlock priceText = new TextBlock
                {
                    Text = $"{hat.Price} червячков",
                    FontSize = 11,
                    Foreground = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 2, 0, 2)
                };
                stack.Children.Add(priceText);

                // Множитель шляпы (например "3x")
                TextBlock multiplierText = new TextBlock
                {
                    Text = $"{hat.Multiplier}x",
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromRgb(0x15, 0x65, 0xC0)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 5)
                };
                stack.Children.Add(multiplierText);

                // Кнопка покупки/надевания
                Button actionButton = new Button
                {
                    Style = (Style)FindResource("HatButtonStyle"),
                    Tag = index,
                    DataContext = hat
                };
                actionButton.Click += (sender, e) => OnHatButtonClick(index);
                stack.Children.Add(actionButton);

                border.Child = stack;
                HatsPanel.Children.Add(border);
            }
        }

        /// <summary>
        /// Обновить состояние кнопок шляп (Купить / Надеть / Надета)
        /// </summary>
        private void RefreshShopButtons()
        {
            for (int i = 0; i < HatsPanel.Children.Count; i++)
            {
                if (HatsPanel.Children[i] is not Border border) continue;
                if (border.Child is not StackPanel stack) continue;
                if (stack.Children[stack.Children.Count - 1] is not Button button) continue;

                Hat hat = _gameManager.Shop.AvailableHats[i];
                bool isPurchased = _gameManager.Crucian.PurchasedHatIndices.Contains(i);
                bool isEquipped = _gameManager.Crucian.EquippedHat == hat;

                if (isEquipped)
                {
                    button.Content = "Надета ✓";
                    button.Background = new SolidColorBrush(Color.FromRgb(0x15, 0x65, 0xC0));
                    button.Foreground = Brushes.White;
                    button.IsEnabled = true;
                }
                else if (isPurchased)
                {
                    button.Content = "Надеть";
                    button.Background = new SolidColorBrush(Color.FromRgb(0x1E, 0x88, 0xE5));
                    button.Foreground = Brushes.White;
                    button.IsEnabled = true;
                }
                else if (_gameManager.Crucian.Currency >= hat.Price)
                {
                    button.Content = "Купить";
                    button.Background = new SolidColorBrush(Color.FromRgb(0x21, 0x96, 0xF3));
                    button.Foreground = Brushes.White;
                    button.IsEnabled = true;
                }
                else
                {
                    button.Content = "Купить";
                    button.Background = new SolidColorBrush(Color.FromRgb(0xB0, 0xB0, 0xB0));
                    button.Foreground = Brushes.White;
                    button.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Обработчик нажатия на кнопку шляпы в магазине
        /// </summary>
        private void OnHatButtonClick(int hatIndex)
        {
            Hat? hat = _gameManager.Shop.GetHatByIndex(hatIndex);
            if (hat == null) return;

            bool isPurchased = _gameManager.Crucian.PurchasedHatIndices.Contains(hatIndex);
            bool isEquipped = _gameManager.Crucian.EquippedHat == hat;

            if (isEquipped)
            {
                // Если шляпа уже надета — снимаем её
                _gameManager.EquipHat(hatIndex);
            }
            else if (isPurchased)
            {
                // Если куплена, но не надета — надеваем
                _gameManager.EquipHat(hatIndex);
            }
            else
            {
                // Пытаемся купить
                bool bought = _gameManager.BuyHat(hatIndex);
                if (!bought)
                {
                    // Не хватает денег
                    ShowFloatingText("Не хватает червячков!", Brushes.Red);
                }
            }

            RefreshShopButtons();
        }

        /// <summary>
        /// Обработчик клика по карасю
        /// </summary>
        private void OnCrucianClick(object sender, RoutedEventArgs e)
        {
            _gameManager.HandleClick();

            // Маленькая анимация: масштабирование карася и шляпы вместе при клике
            var scaleTransform = new ScaleTransform(1, 1);
            CrucianContainer.RenderTransform = scaleTransform;
            CrucianContainer.RenderTransformOrigin = new Point(0.5, 0.5);

            var animation = new DoubleAnimation
            {
                From = 0.9,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(100),
                AutoReverse = false
            };
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
        }

        /// <summary>
        /// Показать всплывающий текст (например об ошибке покупки)
        /// </summary>
        private void ShowFloatingText(string text, Brush color)
        {
            // Берём главный контейнер и добавляем текст на 1.5 секунды
            TextBlock floatingText = new TextBlock
            {
                Text = text,
                Foreground = color,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Добавляем поверх сетки
            if (Content is Grid mainGrid)
            {
                mainGrid.Children.Add(floatingText);

                // Анимация появления и исчезновения
                var opacityAnimation = new DoubleAnimation
                {
                    From = 1.0,
                    To = 0.0,
                    Duration = TimeSpan.FromMilliseconds(1500)
                };

                opacityAnimation.Completed += (s, a) =>
                {
                    mainGrid.Children.Remove(floatingText);
                };

                floatingText.BeginAnimation(TextBlock.OpacityProperty, opacityAnimation);
            }
        }

        // ===== ОБРАБОТЧИКИ СОБЫТИЙ =====

        /// <summary>
        /// Обновить отображение валюты
        /// </summary>
        private void UpdateCurrencyDisplay(long currency)
        {
            Dispatcher.Invoke(() =>
            {
                CurrencyText.Text = currency.ToString("N0");
                RefreshShopButtons();
            });
        }

        /// <summary>
        /// Обновить отображение звания
        /// </summary>
        private void UpdateRankDisplay(Rank rank)
        {
            Dispatcher.Invoke(() =>
            {
                RankNameText.Text = rank.Name;

                // Показываем уведомление о повышении
                ShowFloatingText($"Повышение! Теперь ты — {rank.Name}!", Brushes.DodgerBlue);
            });
        }

        /// <summary>
        /// Обновить отображение шляпы на карасе
        /// </summary>
        private void UpdateHatDisplay()
        {
            Dispatcher.Invoke(() =>
            {
                if (_gameManager.Crucian.EquippedHat != null)
                {
                    HatOverlayImage.Source = new BitmapImage(
                        new Uri(_gameManager.Crucian.EquippedHat.ImagePath, UriKind.Relative));
                    HatOverlayImage.Visibility = Visibility.Visible;
                }
                else
                {
                    HatOverlayImage.Visibility = Visibility.Collapsed;
                }

                RefreshShopButtons();
            });
        }

        /// <summary>
        /// Обновить отображение статистики кликов
        /// </summary>
        private void UpdateTotalClicksDisplay(long totalClicks)
        {
            Dispatcher.Invoke(() =>
            {
                TotalClicksText.Text = totalClicks.ToString("N0");
            });
        }
    }
}
