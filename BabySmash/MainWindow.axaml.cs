using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace BabySmash
{
    public partial class MainWindow : Window
    {
        public Controller Controller { get; set; }

        private readonly IDisposable _handler;

        public UserControl CustomCursor { get; set; }

        public void AddFigure(UserControl c)
        {
            figuresCanvas.Children.Add(c);
        }

        public void RemoveFigure(UserControl c)
        {
            figuresCanvas.Children.Remove(c);
        }

        public MainWindow()
        {
            InitializeComponent();
            _handler = this.AddDisposableHandler(TextInputEvent, RoutedTextInput, RoutingStrategies.Tunnel);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _handler?.Dispose();
            base.OnClosing(e);
        }

        private void RoutedTextInput(object? sender, TextInputEventArgs? e)
        {
            if (e is null)
                return;

            e.Handled = true;

            foreach (var inChar in e.Text ?? string.Empty)
            {
                Controller.ProcessChar(this, inChar);
            }
        }

        /// <inheritdoc />
        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            Controller.MouseWheel(this, e);
        }

        //
        // protected override void OnMouseUp(MouseButtonEventArgs e)
        // {
        //     base.OnMouseUp(e);
        //     controller.MouseUp(this, e);
        // }
        //

        /// <inheritdoc />
        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            Controller.PointerReleased(e);
        }

        /// <inheritdoc />
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            Controller.PointerPressed(this, e);
        }


        /// <inheritdoc />
        protected override void OnPointerEnter(PointerEventArgs e)
        {
            base.OnPointerEnter(e);
            AssertCursor();
            CustomCursor.IsVisible = true;
        }

        /// <inheritdoc />
        protected override void OnPointerLeave(PointerEventArgs e)
        {
            base.OnPointerLeave(e);
            Cursor = new Cursor(StandardCursorType.Arrow);
            CustomCursor.IsVisible = false;
        }

        /// <inheritdoc />
        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            // if (controller.isOptionsDialogShown == false)
            // {
            CustomCursor.IsVisible = true;
            var p = e.GetPosition(mouseDragCanvas);
            double pX = p.X;
            double pY = p.Y;
            Cursor = new Cursor(StandardCursorType.None);
            Canvas.SetTop(CustomCursor, pY);
            Canvas.SetLeft(CustomCursor, pX);
            CustomCursor.ZIndex = int.MaxValue;
            // }
            Controller.PointerMove(this, e);
        }

        private void AssertCursor()
        {
            mouseCursorCanvas.Children.Clear();
            CustomCursor = Utils.GetCursor();
            ((Canvas) CustomCursor.Parent!)?.Children.Remove(CustomCursor);
            CustomCursor.RenderTransform = new ScaleTransform(0.5, 0.5);
            CustomCursor.Name = "customCursor";
            mouseCursorCanvas.Children.Insert(0, CustomCursor); //in front!
            CustomCursor.IsVisible = true;
        }
    }
}