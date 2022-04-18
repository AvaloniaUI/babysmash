using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace BabySmash
{
    /// <summary>
    /// Interaction logic for CoolStar.xaml
    /// </summary>
    [Serializable]
    public partial class CoolStar : UserControl, IHasFace
    {
        public CoolStar(Brush x) : this()
        {
            Body.Fill = x;
        }
        
        public CoolStar()
        {
            this.InitializeComponent();
        }
        
        public bool IsFaceVisible { get; set; }
    }
}