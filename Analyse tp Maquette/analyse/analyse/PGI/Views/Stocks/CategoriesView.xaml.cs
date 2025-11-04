using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PGI.Views.Stocks
{
    public partial class CategoriesView : UserControl
    {
        public CategoriesView()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void LoadCategories()
        {
            var categories = new List<string>
            {
                "Vêtements", "Équipement", "Chaussures", "Accessoires", "Camping", "Escalade"
            };

            foreach (var category in categories)
            {
                var border = new Border
                {
                    Background = Brushes.Transparent,
                    BorderBrush = (Brush)new BrushConverter().ConvertFrom("#E2E8F0"),
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Padding = new Thickness(25, 20, 25, 20)
                };

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var textBlock = new TextBlock
                {
                    Text = category,
                    FontSize = 15,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#1E293B"),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(textBlock, 0);

                var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal };

                var editButton = new Button
                {
                    Content = "✏️ Modifier",
                    Background = Brushes.Transparent,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#669BBC"),
                    BorderThickness = new Thickness(0),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Margin = new Thickness(0, 0, 10, 0),
                    Padding = new Thickness(10, 5, 10, 5)
                };

                var deleteButton = new Button
                {
                    Content = "🗑️ Supprimer",
                    Background = Brushes.Transparent,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#EF4444"),
                    BorderThickness = new Thickness(0),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Padding = new Thickness(10, 5, 10, 5)
                };

                buttonPanel.Children.Add(editButton);
                buttonPanel.Children.Add(deleteButton);
                Grid.SetColumn(buttonPanel, 1);

                grid.Children.Add(textBlock);
                grid.Children.Add(buttonPanel);
                border.Child = grid;

                CategoriesListPanel.Children.Add(border);
            }
        }

        private void TxtNewCategory_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtNewCategory.Text == "Nom de la nouvelle catégorie")
            {
                TxtNewCategory.Text = "";
                TxtNewCategory.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E293B"));
            }
        }

        private void TxtNewCategory_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNewCategory.Text))
            {
                TxtNewCategory.Text = "Nom de la nouvelle catégorie";
                TxtNewCategory.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
            }
        }
    }
}

