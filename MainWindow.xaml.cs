using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Chat
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _dragDropTimer;
        private int _countdown;
        private bool _isDragging;
        private Point _lastMousePosition;
        private UIElement _draggedElement;
        private bool _isCurrentUserYou = true;
        private string tag = "User: ";

        public MainWindow()
        {
            InitializeComponent();
            InitializeDragDropTimer();
        }

        private void InitializeDragDropTimer()
        {
            _dragDropTimer = new DispatcherTimer();
            _dragDropTimer.Interval = TimeSpan.FromSeconds(1);
            _dragDropTimer.Tick += DragDropTimer_Tick;
        }

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

            AddElementToCanvas(textBlock);
        }

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

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(mediaElement);

            AddElementToCanvas(stackPanel);

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


        private void AddElementToCanvas(UIElement element)
        {
            ChatCanvas.Children.Add(element);
            Canvas.SetLeft(element, 10);
            Canvas.SetTop(element, ChatCanvas.Children.Count * 30);

            element.MouseLeftButtonDown += Element_MouseLeftButtonDown;
            element.MouseMove += Element_MouseMove;
            element.MouseLeftButtonUp += Element_MouseLeftButtonUp;
        }

        private void StartDragDropCountdown()
        {
            _countdown = 5;
            CountdownTextBlock.Text = $"Drag & drop enabled for: {_countdown} seconds";
            _dragDropTimer.Start();
            ChatCanvas.AllowDrop = true;
        }

        private void DragDropTimer_Tick(object sender, EventArgs e)
        {
            _countdown--;
            if (_countdown > 0)
            {
                CountdownTextBlock.Text = $"Drag & drop enabled for: {_countdown} seconds";
            }
            else
            {
                CountdownTextBlock.Text = "Drag & drop disabled";
                _dragDropTimer.Stop();
                ChatCanvas.AllowDrop = false;
                foreach (UIElement child in ChatCanvas.Children)
                {
                    child.MouseLeftButtonDown -= Element_MouseLeftButtonDown;
                    child.MouseMove -= Element_MouseMove;
                    child.MouseLeftButtonUp -= Element_MouseLeftButtonUp;
                }
            }
        }

        private void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_countdown > 0)
            {
                _isDragging = true;
                _draggedElement = sender as UIElement;
                _lastMousePosition = e.GetPosition(ChatCanvas);
                _draggedElement.CaptureMouse();
            }
        }

        private void Element_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _draggedElement != null)
            {
                var currentPosition = e.GetPosition(ChatCanvas);
                var offset = currentPosition - _lastMousePosition;
                var newLeft = Canvas.GetLeft(_draggedElement) + offset.X;
                var newTop = Canvas.GetTop(_draggedElement) + offset.Y;

                Canvas.SetLeft(_draggedElement, newLeft);
                Canvas.SetTop(_draggedElement, newTop);

                _lastMousePosition = currentPosition;
            }
        }

        private void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging && _draggedElement != null)
            {
                _isDragging = false;
                _draggedElement.ReleaseMouseCapture();
                _draggedElement = null;
            }
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

        private void SwitchUserButton_Click(object sender, RoutedEventArgs e)
        {
            _isCurrentUserYou = !_isCurrentUserYou;
            SwitchUserButton.Content = _isCurrentUserYou ? "Switch to User" : "Switch to You";

            if(_isCurrentUserYou)
            {
                tag = "User: ";
            }
            else
            {
                tag = "Other: ";
            }
        }
    }
}
