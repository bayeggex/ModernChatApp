using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chat
{
    public partial class MainWindow : Window
    {
        private bool _isCurrentUserYou = true;
        private string tag = "User: ";

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageTextBox.Text;
            if (!string.IsNullOrEmpty(message))
            {
                AddMessageToCanvas(tag + message);
                MessageTextBox.Clear();
                StartDragDropCountdown();
            }
        }

        private void AddMessageToCanvas(string message)
        {
            var textBlock = new TextBlock
            {
                Text = message,
                Foreground = Brushes.White,
                Background = Brushes.Gray,
                Padding = new Thickness(5),
                Margin = new Thickness(5),
                FontSize = 14
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

            canvas.Children.Add(deleteButton);
            canvas.Children.Add(textBlock);

            canvas.MouseEnter += (s, e) => deleteButton.Visibility = Visibility.Visible;
            canvas.MouseLeave += (s, e) => deleteButton.Visibility = Visibility.Collapsed;

            AddElementToCanvas(canvas);
        }

        private void SwitchUserButton_Click(object sender, RoutedEventArgs e)
        {
            _isCurrentUserYou = !_isCurrentUserYou;
            SwitchUserButton.Content = _isCurrentUserYou ? "Switch to User" : "Switch to You";

            tag = _isCurrentUserYou ? "User: " : "Other: ";
        }
    }
}
