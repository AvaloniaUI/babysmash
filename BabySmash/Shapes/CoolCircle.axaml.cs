﻿using System;
using Avalonia.Controls;
using Avalonia.Media;

namespace BabySmash
{
    /// <summary>   
    /// Interaction logic for CoolCircle.xaml
    /// </summary>
    [Serializable]
    public partial class CoolCircle : UserControl, IHasFace
    {
        private bool isFaceVisible;

        public CoolCircle(Brush x) : this()
        {
            this.Body.Fill = x;
        }

        public CoolCircle()
        {
            this.InitializeComponent();
        }
         
        /// <inheritdoc />
        public bool IsFaceVisible { get; set; }
    }
}