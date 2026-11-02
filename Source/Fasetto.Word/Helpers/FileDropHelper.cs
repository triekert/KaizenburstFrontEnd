using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fasetto.Word.Helpers
{
    using System.IO;
    using System.Windows;
    using System.Windows.Input;

    namespace Fasetto.Word
    {
        public static class FileDropHelper
        {
            // Enables or disables the Drop target
            public static readonly DependencyProperty IsFileDropTargetProperty =
                DependencyProperty.RegisterAttached(
                    "IsFileDropTarget",
                    typeof(bool),
                    typeof(FileDropHelper),
                    new PropertyMetadata(false, OnIsFileDropTargetChanged));

            public static void SetIsFileDropTarget(UIElement element, bool value) => element.SetValue(IsFileDropTargetProperty, value);
            public static bool GetIsFileDropTarget(UIElement element) => (bool)element.GetValue(IsFileDropTargetProperty);

            // Binds an ICommand to execute when a file/OLE object is dropped
            public static readonly DependencyProperty FileDropCommandProperty =
                DependencyProperty.RegisterAttached(
                    "FileDropCommand",
                    typeof(ICommand),
                    typeof(FileDropHelper),
                    new PropertyMetadata(null));

            public static void SetFileDropCommand(UIElement element, ICommand value) => element.SetValue(FileDropCommandProperty, value);
            public static ICommand GetFileDropCommand(UIElement element) => (ICommand)element.GetValue(FileDropCommandProperty);

            private static void OnIsFileDropTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                if (d is UIElement element)
                {
                    if ((bool)e.NewValue)
                    {
                        element.AllowDrop = true;
                        element.PreviewDragOver += OnPreviewDragOver;
                        element.Drop += OnDrop;
                    }
                    else
                    {
                        element.AllowDrop = false;
                        element.PreviewDragOver -= OnPreviewDragOver;
                        element.Drop -= OnDrop;
                    }
                }
            }

            private static void OnPreviewDragOver(object sender, DragEventArgs e)
            {
                // Check if dragged item contains File Drop data (standard OLE file payload)
                if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.OemText))
                {
                    e.Effects = DragDropEffects.Copy;
                    e.Handled = true;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }

            private static void OnDrop(object sender, DragEventArgs e)
            {
                if (sender is UIElement element)
                {
                    var command = GetFileDropCommand(element);

                    // Handle standard File Drop payload (Explorer, Outlook attachments saved, etc.)
                    if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                        if (e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
                        {
                            if (command?.CanExecute(files[0]) == true)
                            {
                                command.Execute(files[0]); // Passes the full file path
                            }
                        }
                    }
                    // Handle raw OLE Data Streams / Embedded Objects (e.g. MemoryStream or Object Descriptors)
                    else if (e.Data.GetDataPresent("Native") || e.Data.GetDataPresent("Object Descriptor"))
                    {
                        // For embedded non-file OLE streams, retrieve the raw data stream:
                        var oleData = e.Data.GetData("Native") ?? e.Data.GetData(DataFormats.Serializable);

                        if (command?.CanExecute(oleData) == true)
                        {
                            command.Execute(oleData);
                        }
                    }
                }
            }
        }
    }
}
