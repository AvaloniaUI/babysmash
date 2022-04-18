using System;
using Avalonia.Controls;
using Avalonia.Media;

namespace BabySmash
{
    /// <summary>
    /// Interaction logic for CoolHeart.xaml
    /// </summary>
    [Serializable]
    public partial class CoolHeart : UserControl, IHasFace
    {
        public CoolHeart(Brush x) : this()
        {
            Body.Fill = x;
        }

        public CoolHeart()
        {
            InitializeComponent();
        }
        
        public bool IsFaceVisible { get; set; }
    }
}