using Avalonia.Controls;

namespace BabySmash
{
    using System;
    using Avalonia;
    using Avalonia.Media;

    /// <summary>   
    /// Interaction logic for CoolOval.xaml
    /// </summary>
    [Serializable]
    public partial class CoolOval : UserControl, IHasFace
    {
        public CoolOval(Brush x) : this()
        {
            this.Body.Fill = x;
        }

        public CoolOval()
        {
            this.InitializeComponent();
        }
        
        public bool IsFaceVisible { get; set; }
    }
}