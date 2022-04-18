using System;
using Avalonia.Controls;
using Avalonia.Media;

namespace BabySmash
{
    /// <summary>
    /// Interaction logic for CoolTriangle.xaml
    /// </summary>
    [Serializable]
    public partial class CoolTriangle : UserControl, IHasFace
    {
        public CoolTriangle(Brush x) : this()
        {
            Body.Fill = x;
        }

        public CoolTriangle()
        {
            this.InitializeComponent();
        }
        
        public bool IsFaceVisible { get; set; }
    }
}