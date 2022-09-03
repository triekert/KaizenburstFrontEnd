using System.Collections.Generic;

namespace Fasetto.Word
{
    /// <summary>
    /// The design-time data for a <see cref=" FinanceSelectionDesignModel"/>
    /// </summary>
    public class FinanceSelectionDesignModel :FinanceSelectionViewModel 
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static FinanceSelectionDesignModel Instance => new FinanceSelectionDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public FinanceSelectionDesignModel()
        {
            FirstName = new TextEntryViewModel { Label = "Fist Name", OriginalText = "Luke" };
            LastName = new TextEntryViewModel { Label = "Last Name", OriginalText = "Malpass" };
            Username = new TextEntryViewModel { Label = "Username", OriginalText = "luke" };
            Password = new PasswordEntryViewModel { Label = "Password", FakePassword = "********" };
            Email = new TextEntryViewModel { Label = "Email", OriginalText = "contact@angelsix.com" };
        }

        #endregion
    }
}
