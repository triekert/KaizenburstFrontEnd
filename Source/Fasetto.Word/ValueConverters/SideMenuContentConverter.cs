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
                    /// An instance of the current Finance Hierarchy control
                    /// </summary>
                    var mFinanceMenuControl = new MenuControl("7E669DCA-D356-43F0-BB64-5DF6D1499C99");
                    return mFinanceMenuControl;//mChatListControl;

                case SideMenuContent.Menu:
                    /// <summary>
                    /// An instance of the current Hierarchy control
                    /// </summary>
                    var mMenuMenuControl = new MenuControl();
                    return mMenuMenuControl;//mChatListControl;

                // Finance
                case SideMenuContent.ClientSelection:
                    /// <summary>
                    /// An instance of the current Finance Hierarchy control
                    /// </summary>
                    var mClientSelectionControl = new MenuControl("7E669DCA-D356-43F0-BB64-5DF6D1499C99");
                    return mClientSelectionControl;//mChatListControl;

                // Unknown
                default:
                    return "No UI yet, sorry :)";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
