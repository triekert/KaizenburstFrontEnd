using Fasetto.Word;
using Fasetto.Word.Core;
using System.Collections.Generic;

/// <summary>
/// The Hierarchy element as a view model utilizing Approach 2 model exposure.
/// </summary>
public class DocDataViewModel : BaseModelViewModel<DocDataModel>
{
    #region Constructors

    public DocDataViewModel() : base() { }

    public DocDataViewModel(DocDataModel element) : base(element) { }

    #endregion

    #region View-Model Specific Properties (UI State / Logic Only)

    private bool mIsRemove;
    public bool IsRemove
    {
        get => mIsRemove;
        set { if (mIsRemove != value) { mIsRemove = value; OnPropertyChanged(); } }
    }

    public string TextColor => "FF8B0000";

    #endregion
}

/// <summary>
/// A view model for the overview chat list
/// </summary>
public class DocDataListViewModel : BaseViewModel
{
    /// <summary>
    /// The chat list items for the list
    /// </summary>
    public List<DocDataViewModel> Items { get; set; }
}
