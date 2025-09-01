using System.Windows;

namespace Fasetto.Word
{
    /// <summary>
    /// Set indent for TreeViewItem controls
    /// </summary>
    public static class TreeViewItemHelper
    {
        public static GridLength GetIndent(DependencyObject obj)
        {
            return (GridLength)obj.GetValue(IndentProperty);
        }

        public static void SetIndent(DependencyObject obj, GridLength value)
        {
            obj.SetValue(IndentProperty, value);
        }

        // Using a DependencyProperty as the backing store for Indent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndentProperty =
            DependencyProperty.RegisterAttached("Indent", typeof(GridLength), typeof(TreeViewItemHelper), new PropertyMetadata(new GridLength(0)));
    }
}
