using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Fasetto.Word
{
    //Based on https://blogs.msdn.microsoft.com/atc_avalon_team/2006/03/01/treelistview-show-hierarchy-data-with-details-in-columns/
    public class TreeListView : TreeView
    {
        public static readonly DependencyProperty ViewProperty = DependencyProperty.Register(
            "View", typeof(GridView), typeof(TreeListView), new PropertyMetadata(default(GridView), OnViewChanged));

        public static readonly DependencyProperty SelectItemOnRightClickProperty = DependencyProperty.Register(
            "SelectItemOnRightClick", typeof(bool), typeof(TreeListView), new PropertyMetadata(true));

        public static readonly DependencyProperty SelectItemOnLeftClickProperty = DependencyProperty.Register(
            "SelectItemOnLeftClick", typeof(bool), typeof(TreeListView), new PropertyMetadata(true));



        public static readonly DependencyProperty SelectedItemExProperty = DependencyProperty.Register(
            "SelectedItemEx", typeof(object), typeof(TreeListView), new PropertyMetadata(default(object)));

        public static readonly DependencyProperty ChildrenPropertyNameProperty = DependencyProperty.Register(
            "ChildrenPropertyName", typeof(string), typeof(TreeListView),
            new PropertyMetadata(default(string), OnChildrenPropertyNameChanged));

        //Important: Disable property ItemTemplate
        public static readonly DependencyPropertyKey ReadOnlyItemTemplateProperty = DependencyProperty.RegisterReadOnly(
            "ItemTemplate", typeof(DataTemplate), typeof(TreeListView), new PropertyMetadata(default(DataTemplate)));

        public static new readonly DependencyProperty ItemTemplateProperty
            = ReadOnlyItemTemplateProperty.DependencyProperty;

        private GridViewColumn mCurrentGridViewColumn;
        private DataTemplate mOldDataTemplate;
        private BindingBase mOldDisplayMemberBindingBase;
        private GridViewColumnCollection mOldGridViewColumnCollection;
        private DataTemplateSelector mOldDataTemplateSelector;

        static TreeListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListView),
                new FrameworkPropertyMetadata(typeof(TreeListView)));
        }

        public TreeListView()
        {
            SelectedItemChanged += OnSelectedItemChanged;
        }

        public new DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            protected set => SetValue(ItemTemplateProperty, value);
        }

        public string ChildrenPropertyName
        {
            get => (string)GetValue(ChildrenPropertyNameProperty);
            set => SetValue(ChildrenPropertyNameProperty, value);
        }

        public GridView View
        {
            get => (GridView)GetValue(ViewProperty);
            set => SetValue(ViewProperty, value);
        }

        public object SelectedItemEx
        {
            get => GetValue(SelectedItemExProperty);
            set => SetValue(SelectedItemExProperty, value);
        }

        public bool SelectItemOnRightClick
        {
            get => (bool)GetValue(SelectItemOnRightClickProperty);
            set => SetValue(SelectItemOnRightClickProperty, value);
        }


        public bool SelectItemOnLeftClick
        {
            get => (bool)GetValue(SelectItemOnLeftClickProperty);
            set => SetValue(SelectItemOnLeftClickProperty, value);
        }


        private static void OnViewChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var treeListView = (TreeListView)dependencyObject;
            var gridView = (GridView)dependencyPropertyChangedEventArgs.NewValue;
            treeListView.OnUpdateGridView(gridView?.Columns);
        }

        private void OnUpdateGridView(GridViewColumnCollection gridViewColumnCollection)
        {
            var isOldGridView = gridViewColumnCollection == mOldGridViewColumnCollection;

            if (!isOldGridView && mOldGridViewColumnCollection != null)
            {
                //unsubscribe old GridView
                mOldGridViewColumnCollection.CollectionChanged -= ColumnsOnCollectionChanged;
                ResetCurrentGridViewColumn();
                mOldGridViewColumnCollection = null;
            }

            if (gridViewColumnCollection == null)
                return;

            if (!isOldGridView)
                gridViewColumnCollection.CollectionChanged += ColumnsOnCollectionChanged;

            if (gridViewColumnCollection.Count == 0)
                return;

            var firstColumn = gridViewColumnCollection[0];
            ResetCurrentGridViewColumn();
            mCurrentGridViewColumn = firstColumn;

            mOldDataTemplate = firstColumn.CellTemplate;
            mOldDataTemplateSelector = firstColumn.CellTemplateSelector;
            mOldDisplayMemberBindingBase = firstColumn.DisplayMemberBinding;

            var spFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            spFactory.SetBinding(ContentPresenter.ContentProperty, firstColumn.DisplayMemberBinding ?? new Binding("."));
            if (firstColumn.CellTemplate != null)
                spFactory.SetValue(ContentPresenter.ContentTemplateProperty, firstColumn.CellTemplate);
            else if (firstColumn.CellTemplateSelector != null)
                spFactory.SetValue(ContentPresenter.ContentTemplateSelectorProperty, firstColumn.CellTemplateSelector);

            spFactory.SetBinding(MarginProperty,
                new Binding
                {
                    RelativeSource =
                        new RelativeSource(RelativeSourceMode.FindAncestor, typeof(TreeListViewItem), 1),
                    Converter = (IValueConverter)Application.Current.Resources["LengthConverter"]
                });

            var dataTemplate = new DataTemplate { VisualTree = spFactory };
            firstColumn.DisplayMemberBinding = null;
            firstColumn.CellTemplateSelector = null;
            firstColumn.CellTemplate = dataTemplate;

            mOldGridViewColumnCollection = gridViewColumnCollection;
        }

        private void ResetCurrentGridViewColumn()
        {
            if (mCurrentGridViewColumn == null)
                return;

            mCurrentGridViewColumn.CellTemplate = mOldDataTemplate;
            mCurrentGridViewColumn.DisplayMemberBinding = mOldDisplayMemberBindingBase;
            mCurrentGridViewColumn.CellTemplateSelector = mOldDataTemplateSelector;

            mOldDataTemplate = null;
            mOldDisplayMemberBindingBase = null;
            mOldDataTemplateSelector = null;
        }

        private void ColumnsOnCollectionChanged(object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            OnUpdateGridView((GridViewColumnCollection)sender);
        }

        private void OnSelectedItemChanged(object sender,
            RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
        {
            SelectedItemEx = routedPropertyChangedEventArgs.NewValue;
        }

        private static void OnChildrenPropertyNameChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var treeListView = (TreeListView)dependencyObject;
            var newValue = (string)dependencyPropertyChangedEventArgs.NewValue;
            treeListView.UpdateItemTemplate(new HierarchicalDataTemplate { ItemsSource = new Binding(newValue) });
        }

        private void UpdateItemTemplate(DataTemplate dataTemplate)
        {
            base.ItemTemplate = dataTemplate;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);
            if (!SelectItemOnRightClick)
                return;

            var treeListViewItem =
                WpfExtensions.VisualUpwardSearch<TreeListViewItem>(e.OriginalSource as DependencyObject);
            if (treeListViewItem != null)
            {
                var mA = treeListViewItem.Header;
                treeListViewItem.Focus();
                treeListViewItem.IsSelected = true;
                e.Handled = true;
            }
        }
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            if (!SelectItemOnLeftClick)
                return;
            var source = e.OriginalSource;
            var treeListViewItem =
                WpfExtensions.VisualUpwardSearch<TreeListViewItem>(e.OriginalSource as DependencyObject);
            if (treeListViewItem != null)
            {
                var header = treeListViewItem.Header;
                treeListViewItem.Focus();
                treeListViewItem.IsSelected = true;
                var isFocused = treeListViewItem.IsFocused; 
                //e.Handled = true;
            }
        }

    }
}
