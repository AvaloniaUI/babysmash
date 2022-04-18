using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace BabySmash
{
    /// <summary>
    /// Interaction logic for CoolTrapezoid.xaml
    /// </summary>
    [Serializable]
    public partial class CoolTrapezoid : UserControl, IHasFace
    {
        public CoolTrapezoid(Brush x) : this()
        {
            Body.Fill = x;
        }

        public CoolTrapezoid()
        {
            this.InitializeComponent();
        }
        
        public bool IsFaceVisible { get; set; }
    }
}