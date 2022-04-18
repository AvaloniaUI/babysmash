using System;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;

namespace BabySmash
{
    public static class AnimationHelpers
    {
        public static void ApplyRandomAnimationEffect(Control fe)
        {
            switch (Utils.RandomBetweenTwoNumbers(0, 3))
            {
                case 0:
                    ApplyJiggle(fe);
                    break;
                case 1:
                    ApplySnap(fe);
                    break;
                case 2:
                    ApplyThrob(fe);
                    break;
                case 3:
                    ApplyRotate(fe);
                    break;
            }
        }

        public static void ApplyJiggle(Control fe)
        {
            fe.Classes.Remove("jiggle");
            fe.Classes.Add("jiggle");
        }

        public static void ApplyThrob(Control fe)
        {
            fe.Classes.Remove("throb");
            fe.Classes.Add("throb");
        }

        public static void ApplyZoom(Control fe, TimeSpan duration, double scale)
        { 
            var da = new Animation()
            {
                Duration = duration * 2,
                FillMode = FillMode.Both,
                Children =
                {
                    new KeyFrame()
                    {
                        Cue = new Cue(0.5),
                        Setters = {new Setter(ScaleTransform.ScaleYProperty, scale)}
                    }
                }
            };

            da.RunAsync(fe, null);

        }

        public static void ApplyRotate(Control fe)
        {
            fe.Classes.Remove("rotate");
            fe.Classes.Add("rotate");
        }

        public static void ApplySnap(Control fe)
        {
            fe.Classes.Remove("snap");
            fe.Classes.Add("snap");
        }
    }
}