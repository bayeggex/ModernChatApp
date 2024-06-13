using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chat
{
    public partial class MainWindow : Window
    {
        private void AddImageToCanvas(string imagePath)
        {
            var mediaElement = new MediaElement
            {
                LoadedBehavior = MediaState.Play,
                Stretch = Stretch.Uniform,
                Width = 100,
                Height = 100,
                Margin = new Thickness(5)
            };

            var uri = new Uri(imagePath);
            mediaElement.Source = uri;

            var textBlock = new TextBlock
            {
                Text = tag,
                Foreground = Brushes.White,
                Background = Brushes.Transparent,
                Margin = new Thickness(5),
                FontSize = 12,
                FontWeight = FontWeights.Bold
            };

            var deleteButton = new Button
            {
                Content = "X",
                Width = 20,
                Height = 20,
                Visibility = Visibility.Collapsed,
                Background = Brushes.Red,
                Foreground = Brushes.White,
                Margin = new Thickness(0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            deleteButton.Click += (s, e) => ChatCanvas.Children.Remove((UIElement)((Button)s).Parent);

            var canvas = new Canvas();
            Canvas.SetLeft(deleteButton, -10);
            Canvas.SetTop(deleteButton, -10);

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(mediaElement);

            canvas.Children.Add(deleteButton);
            canvas.Children.Add(stackPanel);

            canvas.MouseEnter += (s, e) => deleteButton.Visibility = Visibility.Visible;
            canvas.MouseLeave += (s, e) => deleteButton.Visibility = Visibility.Collapsed;

            AddElementToCanvas(canvas);

            mediaElement.MouseLeftButtonUp += (sender, e) =>
            {
                var selectedMediaElement = sender as MediaElement;

                var bigImageWindow = new Window
                {
                    Title = "Big Image",
                    Width = 500,
                    Height = 500,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Content = new MediaElement
                    {
                        LoadedBehavior = MediaState.Play,
                        Stretch = Stretch.Uniform,
                        Source = selectedMediaElement.Source
                    }
                };

                bigImageWindow.ShowDialog();
            };
        }

        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg";
            if (openFileDialog.ShowDialog() == true)
            {
                AddImageToCanvas(openFileDialog.FileName);
                StartDragDropCountdown();
            }
        }

        private void AddGifButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "GIF files (*.gif)|*.gif";
            if (openFileDialog.ShowDialog() == true)
            {
                AddImageToCanvas(openFileDialog.FileName);
                StartDragDropCountdown();
            }
        }
    }
}
