using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Fasetto.Word
{
    public class AlignedGridViewRowPresenter : GridViewRowPresenter
    {
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register(
                nameof(TextAlignment),
                typeof(TextAlignment),
                typeof(AlignedGridViewRowPresenter),
                new FrameworkPropertyMetadata(TextAlignment.Left, FrameworkPropertyMetadataOptions.AffectsArrange));

        public TextAlignment TextAlignment
        {
            get => (TextAlignment)GetValue(TextAlignmentProperty);
            set => SetValue(TextAlignmentProperty, value);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            //Apply alignment to each child
            //foreach (var child in InternalChildren)
            //{
            //    if (child is ContentPresenter cp)
            //    {
            //        if (cp.ContentTemplate == null && cp.Content is string)
            //        {
            //            cp.HorizontalAlignment = TextAlignment switch
            //            {
            //                System.Windows.TextAlignment.Center => HorizontalAlignment.Center,
            //                System.Windows.TextAlignment.Right => HorizontalAlignment.Right,
            //                _ => HorizontalAlignment.Left
            //            };
            //        }
            //    }
            //}
            return base.ArrangeOverride(arrangeSize);
        }
    }

    //public class DebugGridViewRowPresenter : GridViewRowPresenter
    //{
    //    private IEnumerable<UIElement> InternalChildren;

    //    protected override Size ArrangeOverride(Size finalSize)
    //    {
    //        var list = (System.Collections.IList)Panel.InternalChildren;
    //        for (var i = 0; i < list.Count; i++)
    //        {
    //            var child = (UIElement)list[i];
    //            Console.WriteLine($"Child type: {child.GetType().Name}");
    //        }
    //        return base.ArrangeOverride(finalSize);
    //    }
    //}

}
