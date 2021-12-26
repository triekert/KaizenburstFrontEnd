using Fasetto.Word.Core;
using System;
using System.Globalization;

namespace Fasetto.Word
{
    /// <summary>
    /// A converter that takes a <see cref="SideMenuContent"/> and converts it to the 
    /// correct UI element
    /// </summary>
    public class PopupContentConverter : BaseValueConverter<PopupContentConverter>
    {
        #region Protected Members

        /// <summary>
        /// An instance of the current chat list control
        /// </summary>
        //protected ChatListControl mChatListControl = new ChatListControl();


        /// <summary>
        /// An instance of the current Hiearchy control
        /// </summary>
 

        #endregion

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Get the side menu type
            var popupType = (PopupContent)value;

            // Switch based on type
            switch (popupType)
            {
                // Add Hierarchy Element 
                case PopupContent.AddElement:
                    /// <summary>
                    /// An instance of HierarchyElement control
                    /// </summary>

                    var mHierarchyElementControl = new HierarchyElementControl();
                    return mHierarchyElementControl;

                    /// <summary>
                    /// An instance of HierarchyElement control
                    /// </summary>
                case PopupContent.SelectHierarchy:
                    var mHierarchySelectControl = new SettingsControl();
                    return mHierarchySelectControl;



                // Unknown
                default:
                    return "No UI yet, sorry :)";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
