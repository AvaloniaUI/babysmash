using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace BabySmash
{
    /// <summary>
    /// Interaction logic for CoolSquare.xaml
    /// </summary>
    [Serializable]
    public partial class CoolSquare : UserControl, IHasFace
    {
        public CoolSquare(Brush x) : this()
        {
            this.Body.Fill = x;
        }

        public CoolSquare()
        {
            this.InitializeComponent();
        }
        
        public bool IsFaceVisible { get; set; }
    }
}