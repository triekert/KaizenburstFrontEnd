using Fasetto.Word.Core;
using System.Security;

namespace Fasetto.Word
{
    /// <summary>
    /// Interaction logic for LoadReadingsPage.xaml
    /// </summary>
    public partial class LoadReadingsPage : BasePage<LoadReadingsPageViewModel>, IHavePassword
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public LoadReadingsPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor with specific view model
        /// </summary>
        public LoadReadingsPage(LoadReadingsPageViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }

        #endregion


        /// <summary>
        /// The secure password for this login page
        /// </summary>
        public SecureString SecurePassword => PasswordText.SecurePassword;
    }
}
