using System;
using Avalonia;
using Avalonia.Media;

namespace BabySmash
{
    /// <summary>
    /// Interaction logic for CoolTrapezoid.xaml
    /// </summary>
    [Serializable]
    public partial class CoolTrapezoid : IHasFace
    {
        public CoolTrapezoid(Brush x) : this()
        {
            Body.Fill = x;
        }

        public CoolTrapezoid()
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