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
        /// Store View Model of current popup to allow reverse navigation
        /// </summary>
        public object PriorPopupViewModel { get; set; }


        // <summary>
        /// Level limit for hierarchy's to be returned (1 = top level only...)
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// The action to run when saving the text.
        /// Returns true if the commit was successful, or false otherwise.
        /// </summary>
        public Func<Task<bool>> CommitAction { get; set; }


        /// <summary>
        /// The action to run when initiating the control.
        /// Returns true if the preparation was successful, or false otherwise.
        /// </summary>
        public Func<Task<bool>> PrepareAction { get; set; }

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
            //HierarchyitemSelectCommand = new RelayCommand(HierarchyitemSelect);
            //ViewModelApplication.CurrentControlViewModel = this;
            //HISVM MviewModel = new HISVM(this);    

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
            //Editing = true;

            var result = default(bool);

            RunCommandAsync(() => Working, async () =>
            {

                // Try and do the work
                result = PrepareAction == null ? true : await PrepareAction();

            }).ContinueWith(t =>
            {

            });

            ViewModelApplication.PopupVisible = true;
            ViewModelApplication.CurrentPopupContent = 0;

            ViewModelApplication.CurrentPopupContent = PopupContent.HierarchySelection;


        }

        /// <summary>
        /// Commits the content and exits out of edit mode
        /// </summary>
        public void Save()
        {
            // Store the result of a commit call
            var result = default(bool);


            RunCommandAsync(() => Working, async () =>
            {
                // While working, come out of edit mode
                Editing = false;

                EditedKid = EditedKid;
                EditedName = EditedName;

                OriginalKid = OriginalKid;

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
                    //OriginalText = currentSavedValue;

                    // Go back into edit mode
                    Editing = true;
                }
            });
        }


        /// <summary>
        /// Cancels out of edit mode
        /// </summary>
        public void Cancel()
        {
            Editing = false;
        }





        #endregion
    }
}
