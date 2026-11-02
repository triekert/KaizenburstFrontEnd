using Fasetto.Word.Core;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{
    /// <summary>
    /// The view model for a text entry to edit a string value
    /// </summary>
    public class HierarchyItemSelectionViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The label to identify what this value is for
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The current saved value
        /// </summary>
        public string OriginalName { get; set; }

        /// <summary>
        /// The current non-commit edited text
        /// </summary>
        public string EditedName { get; set; }

        /// <summary>
        /// GUID representing the primary key of the selected Hierarchy Elements
        /// </summary>
        public string OriginalKid { get; set; }

        /// <summary>
        /// GUID representing the primary key of the selected Hierarchy Elements
        /// </summary>
        public string EditedKid { get; set; }

        /// <summary>
        /// GUID representing the primary key of the type of hierarchy represented by a hierarchy of elements
        /// </summary>
        public string HierarchyTypeID { get; set; }

        /// <summary>
        /// GUID representing the primary key of the root of a hierarchy of elements
        /// </summary>
        public string HierarchyID { get; set; }

        /// <summary>
        /// GUID representing the primary key of the selected Client
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// GUID representing the root of the hierarchy if not the main root
        /// </summary>
        public string RootID { get; set; }

        /// <summary>
        /// Indicates if the current text is in edit mode
        /// </summary>
        public bool Editing { get; set; }

        /// <summary>
        /// Indicates if the current control is pending an update (in progress)
        /// </summary>
        public bool Working { get; set; }

        /// <summary>
        /// Indicates if the current control is pending an update (in progress)
        /// </summary>
        public bool Processed { get; set; }

        /// <summary>
        /// Store View Model of current popup to allow reverse navigation
        /// </summary>
        public object PriorPopupViewModel { get; set; }

        /// <summary>
        /// Store enum of current popup view
        /// </summary>
        public PopupContent PriorPopupContent { get; set; }

        /// <summary>
        /// Level limit for hierarchy's to be returned (1 = top level only...)
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// The action to run when saving the newly selected hierarchy item.
        /// Returns true if the commit was successful, or false otherwise.
        /// </summary>
        public Func<Task<bool>> CommitAction { get; set; }

        /// <summary>
        /// The action to run after selection of hierarchy item.
        /// This is in preparation for updating the current field.
        /// </summary>
        public Func<Task<bool>> ProcessSelectionAction { get; set; }

        /// <summary>
        /// The action to run when initiating the control.
        /// Returns true if the preparation was successful, or false otherwise.
        /// </summary>
        public Func<Task<bool>> PrepareAction { get; set; }

        #endregion

        #region Public Commands

        public ICommand EditCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand NullifyCommand { get; set; }
        public ICommand HierarchyitemSelectCommand { get; set; }

        #endregion

        #region Constructor 

        /// <summary>
        /// Default constructor
        /// </summary>
        public HierarchyItemSelectionViewModel()
        {
            // Create commands pointing to the async implementations
            EditCommand = new RelayCommand(async () => await EditAsync());
            CancelCommand = new RelayCommand(async () => await CancelAsync());
            SaveCommand = new RelayCommand(async () => await SaveAsync());
            NullifyCommand = new RelayCommand(async () => await NullifyAsync());
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Puts the control into edit mode asynchronously
        /// </summary>
        public async Task EditAsync()
        {
            EditedName = OriginalName;

            await RunCommandAsync(() => Working, async () =>
            {
                var result = PrepareAction == null ? true : await PrepareAction();

                ViewModelApplication.PopupVisible = true;
                ViewModelApplication.CurrentPopupContent = 0;
                ViewModelApplication.CurrentPopupContent = PopupContent.HierarchySelection;

                await Task.Delay(50);
            });
        }

        /// <summary>
        /// Commits the content and exits out of edit mode asynchronously
        /// </summary>
        public async Task SaveAsync()
        {
            var currentSavedValue = OriginalName;
            var result = default(bool);

            await RunCommandAsync(() => Working, async () =>
            {
                Editing = false;
                result = CommitAction == null ? true : await CommitAction();
            });

            if (!result)
            {
                OriginalName = currentSavedValue;
                Editing = true;
            }
            else
            {
                ClearKeyboardFocus();
            }
        }

        /// <summary>
        /// Processes selection asynchronously and restores focus to textbox
        /// </summary>
        public async Task ProcessSelectionAsync()
        {
            var result = default(bool);

            await RunCommandAsync(() => Working, async () =>
            {
                result = ProcessSelectionAction == null ? true : await ProcessSelectionAction();
                await Task.Delay(50);
            });

            // Toggle editing off and back on to re-trigger focus attached property cleanly
            Editing = false;
            Editing = true;
        }

        /// <summary>
        /// Cancels out of edit mode asynchronously
        /// </summary>
        public async Task CancelAsync()
        {
            await Task.CompletedTask;
            EditedKid = "";
            EditedName = "";
            Editing = false;
            ClearKeyboardFocus();
        }

        /// <summary>
        /// Nullifies the current content of the field asynchronously
        /// </summary>
        public async Task NullifyAsync()
        {
            await Task.CompletedTask;
            Editing = false;
            EditedKid = "";
            EditedName = "";
            OriginalName = "";
            OriginalKid = "";
            ClearKeyboardFocus();
        }

        /// <summary>
        /// Safely clears keyboard focus from the active element back to the container root
        /// </summary>
        private void ClearKeyboardFocus()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Keyboard.ClearFocus();
            });
        }

        #endregion
    }
}