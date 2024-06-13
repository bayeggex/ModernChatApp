using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        private void InitializeDragDropTimer()
        {
            _dragDropTimer = new DispatcherTimer();
            _dragDropTimer.Interval = TimeSpan.FromSeconds(1);
            _dragDropTimer.Tick += DragDropTimer_Tick;
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

        private void AddElementToCanvas(UIElement element)
        {
            ChatCanvas.Children.Add(element);
            Canvas.SetLeft(element, 10);
            Canvas.SetTop(element, ChatCanvas.Children.Count * 30);

            element.MouseLeftButtonDown += Element_MouseLeftButtonDown;
            element.MouseMove += Element_MouseMove;
            element.MouseLeftButtonUp += Element_MouseLeftButtonUp;
        }
    }
}
