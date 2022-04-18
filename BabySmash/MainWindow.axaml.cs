using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

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
        // protected override void OnMouseDown(MouseButtonEventArgs e)
        // {
        //     base.OnMouseDown(e);
        //     controller.MouseDown(this, e);
        // }
        //
        // protected override void OnMouseEnter(MouseEventArgs e)
        // {
        //     base.OnMouseEnter(e);
        //     AssertCursor();
        //     CustomCursor.Visibility = Visibility.Visible;
        // }
        //
        // protected override void OnMouseLeave(MouseEventArgs e)
        // {
        //     base.OnMouseLeave(e);
        //     CustomCursor.Visibility = Visibility.Hidden;
        // }
        //
        // protected override void OnMouseMove(MouseEventArgs e)
        // {
        //     base.OnMouseMove(e);
        //     if (controller.isOptionsDialogShown == false)
        //     {
        //         CustomCursor.Visibility = Visibility.Visible;
        //         Point p = e.GetPosition(mouseDragCanvas);
        //         double pX = p.X;
        //         double pY = p.Y;
        //         Cursor = Cursors.None;
        //         Canvas.SetTop(CustomCursor, pY);
        //         Canvas.SetLeft(CustomCursor, pX);
        //         Canvas.SetZIndex(CustomCursor, int.MaxValue);
        //     }
        //     controller.MouseMove(this, e);
        // }

        //
        // protected override void OnLostMouseCapture(MouseEventArgs e)
        // {
        //     base.OnLostMouseCapture(e);
        //     controller.LostMouseCapture(this, e);
        // }
        //
        // internal void AssertCursor()
        // {
        //     try
        //     {
        //         mouseCursorCanvas.Children.Clear();
        //         CustomCursor = Utils.GetCursor();
        //         if (CustomCursor.Parent != null)
        //         {
        //             ((Canvas)CustomCursor.Parent).Children.Remove(CustomCursor);
        //         }
        //         CustomCursor.RenderTransform = new ScaleTransform(0.5, 0.5);
        //         CustomCursor.Name = "customCursor";
        //         mouseCursorCanvas.Children.Insert(0, CustomCursor); //in front!
        //         CustomCursor.Visibility = Visibility.Hidden;
        //     }
        //     catch (System.NotSupportedException)
        //     {
        //         //we can die here if we ALT-F4 while in the Options Dialog
        //     }
        // }
        //
    }
}