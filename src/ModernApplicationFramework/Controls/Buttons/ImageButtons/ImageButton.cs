﻿using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.Controls.Buttons
{
    /// <inheritdoc />
    /// <summary>
    /// A button control with an image as background. The image can have three states
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Controls.Buttons.Button" />
    public class ImageButton : Button
    {
        /// <summary>
        /// The hover image property
        /// </summary>
        public static readonly DependencyProperty HoverImageProperty = DependencyProperty.Register(
            "HoverImage", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(default(ImageSource)));

        /// <summary>
        /// The normal image property
        /// </summary>
        public static readonly DependencyProperty NormalImageProperty = DependencyProperty.Register(
            "NormalImage", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(default(ImageSource)));

        /// <summary>
        /// The pressed image property
        /// </summary>
        public static readonly DependencyProperty PressedImageProperty = DependencyProperty.Register(
            "PressedImage", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(default(ImageSource)));

        static ImageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton),
                new FrameworkPropertyMetadata(typeof(ImageButton)));
        }

        public ImageSource HoverImage
        {
            get => (ImageSource) GetValue(HoverImageProperty);
            set => SetValue(HoverImageProperty, value);
        }

        public ImageSource NormalImage
        {
            get => (ImageSource) GetValue(NormalImageProperty);
            set => SetValue(NormalImageProperty, value);
        }

        public ImageSource PressedImage
        {
            get => (ImageSource) GetValue(PressedImageProperty);
            set => SetValue(PressedImageProperty, value);
        }
    }
}