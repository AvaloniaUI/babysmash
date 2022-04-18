using System;
using Avalonia.Controls;
using Avalonia.Media;

namespace BabySmash
{
    /// <summary>
    /// Interaction logic for CoolHexagon.xaml
    /// </summary>
    [Serializable]
    public partial class CoolHexagon : UserControl, IHasFace
    {
        public CoolHexagon(Brush x) : this()
        {
            this.Body.Fill = x;
        }

        public CoolHexagon()
        {
            this.InitializeComponent();
        }
        
        public bool IsFaceVisible { get; set; }
    }
}