using System;
using Avalonia.Controls;
using Avalonia.Media;

namespace BabySmash
{
    /// <summary>   
    /// Interaction logic for CoolOval.xaml
    /// </summary>
    [Serializable]
    public partial class CoolOval : UserControl, IHasFace
    {
        public CoolOval(Brush x) : this()
        {
            Body.Fill = x;
        }

        public CoolOval()
        {
            InitializeComponent();
        }
        
        public bool IsFaceVisible { get; set; }
    }
}