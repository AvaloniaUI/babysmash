using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;
using BabySmash.Shapes;
using BrushControlFunc = System.Func<Avalonia.Media.Brush, Avalonia.Controls.UserControl>;

namespace BabySmash
{
    public class FigureTemplate
    {
        public Brush Fill { get; set; }
        public Color Color { get; set; }

        public BrushControlFunc GeneratorFunc { get; set; }

        // public Effect Effect { get; set; }
        public string Name { get; set; }
        public string Letter { get; set; }
    }

    public class FigureGenerator
    {
        private static readonly List<KeyValuePair<BabySmashShape, BrushControlFunc>> hashTableOfFigureGenerators =
            new List<KeyValuePair<BabySmashShape, BrushControlFunc>>
            {
                new(BabySmashShape.Circle, x => new CoolCircle(x)),
                new(BabySmashShape.Oval, x => new CoolOval(x)),
                new(BabySmashShape.Rectangle, x => new CoolRectangle(x)),
                new(BabySmashShape.Hexagon, x => new CoolHexagon(x)),
                new(BabySmashShape.Trapezoid, x => new CoolTrapezoid(x)),
                new(BabySmashShape.Star, x => new CoolStar(x)),
                new(BabySmashShape.Square, x => new CoolSquare(x)),
                new(BabySmashShape.Triangle, x => new CoolTriangle(x)),
                new(BabySmashShape.Heart, x => new CoolHeart(x))
            };

        public static UserControl NewUserControlFrom(FigureTemplate template)
        {
            UserControl retVal;

            if (template.Letter.Length == 1 && char.IsLetterOrDigit(template.Letter[0]))
            {
                retVal = new CoolLetter(template.Fill, template.Letter[0]);
            }
            else
            {
                retVal = template.GeneratorFunc(template.Fill);
            }

            return retVal;
        }

        //TODO: Should this be in XAML? Would that make it better?
        //TODO: Should I change the height, width and stroke to be relative to the screen size?
        //TODO: Where can I get REALLY complex shapes like animal vectors or custom pics? Where do I store them?

        public static FigureTemplate GenerateFigureTemplate(char displayChar)
        {
            var c = Utils.GetRandomColor();

            string name = null;
            var nameFunc =
                hashTableOfFigureGenerators[Utils.RandomBetweenTwoNumbers(0, hashTableOfFigureGenerators.Count - 1)];
            if (char.IsLetterOrDigit(displayChar))
            {
                name = displayChar.ToString();
            }
            else
            {
                name = Controller.GetLocalizedString(nameFunc.Key.ToString());
            }

            return new FigureTemplate
            {
                Color = c,
                Name = name,
                GeneratorFunc = nameFunc.Value,
                Fill = Utils.GetGradientBrush(c),
                Letter = displayChar.ToString(),
                // Effect = Animation.GetRandomBitmapEffect()
            };
        }
    }
}