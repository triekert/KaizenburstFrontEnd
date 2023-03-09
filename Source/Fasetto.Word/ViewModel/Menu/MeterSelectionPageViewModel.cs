using static Fasetto.Word.DI;
using static Fasetto.Word.Core.CoreDI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Fasetto.Word.Core;

namespace Fasetto.Word
{
    /// <summary>
    /// A view model for managing hierarchies 
    /// </summary>
    public class MeterSelectionPageViewModel : BaseViewModel
    {
        #region Protected Members

        /// <summary>
        /// The last searched text in this list
        /// </summary>
        protected string mLastSearchText;

        /// <summary>
        /// The text to search for in the search command
        /// </summary>
        protected string mSearchText;


        /// <summary>
        /// The chat thread items for the list
        /// </summary>
        //protected ObservableCollection<ChatMessageListItemViewModel> mItems;

        /// <summary>
        /// A flag indicating if the search dialog is open
        /// </summary>
        protected bool mSearchIsOpen;

        public HierarchyTreeViewModel mViewModel ;
        #endregion

        #region Public Properties

        /// <summary>
        /// The chat thread items for the list
        /// NOTE: Do not call Items.Add to add messages to this list
        ///       as it will make the FilteredItems out of sync
        /// </summary>
        //public ObservableCollection<ChatMessageListItemViewModel> Items
        //{
        //    get => mItems;
        //    set
        //    {
        //        // Make sure list has changed
        //        if (mItems == value)
        //            return;

        //        // Update value
        //        mItems = value;

        //        // Update filtered list to match
        //        FilteredItems = new ObservableCollection<ChatMessageListItemViewModel>(mItems);
        //    }
        //}

        /// <summary>
        /// The chat thread items for the list that include any search filtering
        /// </summary>
        //public ObservableCollection<ChatMessageListItemViewModel> FilteredItems { get; set; }

        /// <summary>
        /// The title of this chat list
        /// </summary>
        public string DisplayTitle { get; set; }
        /// <summary>
        /// Selected Client for bulk metering
        /// </summary>
        public TextEntryViewModel Client { get; set; }

        /// <summary>
        /// Selected Bulk Meter for reconciliation of water consumption
        /// </summary>
        public TextEntryViewModel BulkMeter { get; set; }
        /// <summary>
        /// Selected Client for bulk metering
        /// </summary>
        public DateTimeViewModel TimeStart { get; set; }

        /// <summary>
        /// Selected Bulk Meter for reconciliation of water consumption
        /// </summary>
        public DateTimeViewModel TimeStop { get; set; }




        /// <summary>
        /// The text to search for when we do a search
        /// </summary>


       

        #endregion

        #region Public Commands

        /// <summary>
        /// The command for when the attachment button is clicked
        /// </summary>
        public ICommand AttachmentButtonCommand { get; set; }

        /// <summary>
        /// The command for when the area outside of any popup is clicked
        /// </summary>
        public ICommand PopupClickawayCommand { get; set; }

        /// <summary>
        /// The command for when the user clicks the send button
        /// </summary>
        public ICommand SendCommand { get; set; }

        /// <summary>
        /// The command for when the user wants to search
        /// </summary>
        public ICommand SearchCommand { get; set; }

        /// <summary>
        /// The command for when the user wants to open the search dialog
        /// </summary>
        public ICommand OpenSearchCommand { get; set; }

        /// <summary>
        /// The command for when the user wants to close to search dialog
        /// </summary>
        public ICommand CloseSearchCommand { get; set; }

        /// <summary>
        /// The command for when the user wants to clear the search text
        /// </summary>
        public ICommand ClearSearchCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MeterSelectionPageViewModel()
        {
            //Populate screen title
            //mViewModel = (HierarchyTreeViewModel)ViewModelApplication.CurrentControlViewModel;
            //var results = mViewModel.mHDML.FirstOrDefault(x => x.ParentCategoryID == "00000000-0000-0000-0000-000000000000");
            DisplayTitle = "Bulk Meter Selection";
            // Create commands
            //AttachmentButtonCommand = new RelayCommand(AttachmentButton);
            //PopupClickawayCommand = new RelayCommand(PopupClickaway);



            // Make a default menu
            //AttachmentMenu = new ChatAttachmentPopupMenuViewModel();
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// When the attachment button is clicked show/hide the attachment pop-up
        /// </summary>
        //public void AttachmentButton()
        //{
        //    // Toggle menu visibility
        //    AttachmentMenuVisible ^= true;
        //}

        /// <summary>
        /// When the pop-up click away area is clicked hide any pop-ups
        /// </summary>
        //public void PopupClickaway()
        //{
        //    // Hide attachment menu
        //    AttachmentMenuVisible = false;
        //}





        #endregion
    }
}
