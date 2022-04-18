using System;
using Avalonia;
using Avalonia.Media;

namespace BabySmash
{
    /// <summary>
    /// Interaction logic for CoolHexagon.xaml
    /// </summary>
    [Serializable]
    public partial class CoolHexagon : IHasFace
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