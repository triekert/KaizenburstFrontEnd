using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Fasetto.Word
{
    public partial class DocumentGridControl : UserControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty DocumentListProperty =
            DependencyProperty.Register(nameof(DocumentList), typeof(IEnumerable), typeof(DocumentGridControl), new PropertyMetadata(null));

        public IEnumerable DocumentList
        {
            get => (IEnumerable)GetValue(DocumentListProperty);
            set => SetValue(DocumentListProperty, value);
        }

        #endregion

        public DocumentGridControl()
        {
            InitializeComponent();
        }

        #region Event Handlers

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e) { }
        private void DataGridRow_KeyDown(object sender, KeyEventArgs e) { }
        private void DataGridRow_MouseRightClick(object sender, MouseButtonEventArgs e) { }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.Focus();
                textBox.SelectAll();
            }
        }

        #endregion
    }
}