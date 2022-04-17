using System;
using Avalonia;
using Avalonia.Media;

namespace BabySmash
{
    /// <summary>
    /// Interaction logic for CoolSquare.xaml
    /// </summary>
    [Serializable]
    public partial class CoolSquare : IHasFace
    {
        public CoolSquare(Brush x) : this()
        {
            this.Body.Fill = x;
        }

        public CoolSquare()
        {
            this.InitializeComponent();
        }
        
        public Visibility FaceVisible
        {
            get
            {
                return Face.Visibility;
            }
            set
            {
                Face.Visibility = value;
            }
        }
    }
}