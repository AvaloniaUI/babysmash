using System;
using Avalonia.Controls;
using Avalonia.Media;

namespace BabySmash
{
    /// <summary>
    /// Interaction logic for CoolSquare.xaml
    /// </summary>
    [Serializable]
    public partial class CoolRectangle : UserControl, IHasFace
    {
        private bool isFaceVisible;

        public CoolRectangle(Brush x) : this()
        {
            Body.Fill = x;
        }

        public CoolRectangle()
        {
            InitializeComponent();
        }
         
        public bool IsFaceVisible { get; set; }
    }
}