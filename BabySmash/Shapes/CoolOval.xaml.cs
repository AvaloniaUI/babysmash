namespace BabySmash
{
    using System;
    using Avalonia;
    using Avalonia.Media;

    /// <summary>   
    /// Interaction logic for CoolOval.xaml
    /// </summary>
    [Serializable]
    public partial class CoolOval : IHasFace
    {
        public CoolOval(Brush x) : this()
        {
            this.Body.Fill = x;
        }

        public CoolOval()
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