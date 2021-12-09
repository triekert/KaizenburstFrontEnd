using Fasetto.Word.Core;
using System;
using System.Globalization;

namespace Fasetto.Word
{
    /// <summary>
    /// A converter that takes a <see cref="SideMenuContent"/> and converts it to the 
    /// correct UI element
    /// </summary>
    public class SideMenuContentConverter : BaseValueConverter<SideMenuContentConverter>
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
            var sideMenuType = (SideMenuContent)value;

            // Switch based on type
            switch (sideMenuType)
            {
                // Chat 
                case SideMenuContent.Chat:
                    /// <summary>
                    /// An instance of the current chat list control
                    /// </summary>

                    var mChatListControl = new ChatListControl();
                    return mChatListControl;

                // Finance
                case SideMenuContent.Finance:
                    /// <summary>
                    /// An instance of the current Hiearchy control
                    /// </summary>
                    var mFinanceMenuControl = new HierarchyControl();
                    return mFinanceMenuControl;//mChatListControl;

                // Unknown
                default:
                    return "No UI yet, sorry :)";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
