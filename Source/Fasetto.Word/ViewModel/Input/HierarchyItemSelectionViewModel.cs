using Fasetto.Word.Core;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{
    /// <summary>
    /// The view model for a text entry to edit a string value
    /// <summary>
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
        public string EditedName{ get; set; }

        /// <summary>
        /// GUID representing the primary key of the selected Hierarcy Elements
        /// </summary>
        public bool EditedKid { get; set; }

        /// <summary>
        /// Indicates if the current text is in edit mode
        /// </summary>
        public bool Editing { get; set; }

        /// <summary>
        /// Indicates if the current control is pending an update (in progress)
        /// </summary>
        public bool Working { get; set; }

        /// <summary>
        /// The action to run when saving the text.
        /// Returns true if the commit was successful, or false otherwise.
        /// </summary>
        public Func<Task<bool>> CommitAction { get; set; }

        #endregion

        #region Public Commands

        /// <summary>
        /// Puts the control into edit mode
        /// </summary>
        public ICommand EditCommand { get; set; }

        /// <summary>
        /// Cancels out of edit mode
        /// </summary>
        public ICommand CancelCommand { get; set; }

        /// <summary>
        /// Commits the edits and saves the value
        /// as well as goes back to non-edit mode
        /// </summary>
        public ICommand SaveCommand { get; set; }


        /// <summary>
        /// The user can select a hierarchy item based on the 
        /// root value in the application view model
        /// </summary>
        public ICommand HierarchyitemSelectCommand { get; set; }

        #endregion

        #region Constructor 

        /// <summary>
        /// Default constructor
        /// </summary>
        public HierarchyItemSelectionViewModel()
        {
            // Create commands
            EditCommand = new RelayCommand(Edit);
            CancelCommand = new RelayCommand(Cancel);
            SaveCommand = new RelayCommand(Save);
            HierarchyitemSelectCommand = new RelayCommand(HierarchyitemSelect);
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Puts the control into edit mode
        /// </summary>
        public void Edit()
        {
            // Set the edited text to the current value
            EditedName = OriginalName;

            // Go into edit mode
            Editing = true;
        }

        /// <summary>
        /// Cancels out of edit mode
        /// </summary>
        public void Cancel()
        {
            Editing = false;
        }

        /// <summary>
        /// Cancels out of edit mode
        /// </summary>
        public void HierarchyitemSelect()
        {
            //to do: add a variable for passing KID between parent and child, as well as 
            ViewModelApplication.CurrentPopupContent = PopupContent.HierarchyItemSelection;
            ViewModelApplication.PopupVisible = true;
            EditedName = ViewModelApplication.ControlParameter2;
        }

        /// <summary>
        /// Commits the content and exits out of edit mode
        /// </summary>
        public void Save()
        {
            // Store the result of a commit call
            var result = default(bool);
            EditedName = EditedName;

            // Save currently saved value
            var currentSavedValue = OriginalName;

            RunCommandAsync(() => Working, async () =>
            {
                // While working, come out of edit mode
                Editing = false;

                // Commit the changed text
                // So we can see it while it is working
                OriginalName = EditedName;

                // Try and do the work
                result = CommitAction == null ? true : await CommitAction();

            }).ContinueWith(t =>
            {
                // If we succeeded...
                // Nothing to do
                // If we fail...
                if (!result)
                {
                    // Restore original value
                    OriginalName = currentSavedValue;

                    // Go back into edit mode
                    Editing = true;
                }
            });
        }




        #endregion
    }
}
