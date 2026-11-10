using Fasetto.Word.Core;
using System.Collections.Generic;

namespace Fasetto.Word
{
    public static class TransactionExtensions
    {
        /// <summary>
        /// Copies shared transaction properties from a source to a target TransactionViewModel.
        /// ----------------------------------
        /// // Group them into a list/array and sync them in one line
        ///new[] { Mcategory, Mcategory1 }.CopyPropertiesFrom(Selected);
        ///----------------------------------------------------------------
        /// </summary>
        public static void CopyPropertiesFrom(this TransactionViewModel target, TransactionViewModel source)
        {
            if (target == null || source == null) return;

            target.KFinTranID = source.KFinTranID;
            target.Posted_Date = source.Posted_Date;
            target.Description = source.Description;
            target.TransAmount = source.TransAmount;
            target.ActualAmount = source.ActualAmount;
            target.ShortName = source.ShortName;
            target.KCategoryID = source.KCategoryID;
            target.KFinActualID = source.KFinActualID;
            target.DateEffective = source.DateEffective;
            target.KPartyID = source.KPartyID;
            target.KPartyName = source.KPartyName;
            target.KProjectID = source.KProjectID;
            target.KProjectName = source.KProjectName;
            target.KAssetID = source.KAssetID;
            target.KAssetName = source.KAssetName;
            target.KPersonID = source.KPersonID;
            target.KPersonName = source.KPersonName;
            target.IsTemplate = source.IsTemplate;
            target.Notes = source.Notes;
            target.KAccountID = source.KAccountID;
            target.KAccountName = source.KAccountName;
            target.Units = source.Units;
            target.KClientID = source.KClientID;
        }

        /// <summary>
        /// Copies shared transaction properties from a source to a collection of target TransactionViewModels.
        /// -----------------------------------------------------------------------------------------------------
        /// Mcategory?.CopyPropertiesFrom(Selected);
        ///Mcategory1?.CopyPropertiesFrom(Selected);
        /// -----------------------------------------------------------------------------------------------------
        /// </summary>
        public static void CopyPropertiesFrom(this IEnumerable<TransactionViewModel> targets, TransactionViewModel source)
        {
            if (targets == null || source == null) return;

            foreach (var target in targets)
            {
                target.CopyPropertiesFrom(source);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// ---------------------------------------------------------------------------------------
        /// Mtmp2.Add(Selected.ToApiModel());
        /// Mtmp2.Add(Selected.ToApiModel("u"));
        /// ---------------------------------------------------------------------------------------
        /// <param name="source"></param>
        /// <param name="changeType"></param>
        /// <returns></returns>
        public static TransactionResultApiModel ToApiModel(this TransactionViewModel source, string changeType = "a")
        {
            if (source == null)
                return null;

            return new TransactionResultApiModel
            {
                Month = source.Month,
                KFinTranID = source.KFinTranID,
                Posted_Date = source.Posted_Date,
                Description = source.Description,
                TransAmount = source.TransAmount,
                ActualAmount = source.ActualAmount,
                ShortName = source.ShortName,
                KCategoryID = source.KCategoryID,
                KFinActualID = source.KFinActualID,
                DateEffective = source.DateEffective,
                KPartyID = source.KPartyID,
                KPartyName = source.KPartyName,
                KProjectID = source.KProjectID,
                KProjectName = source.KProjectName,
                KAssetID = source.KAssetID,
                KAssetName = source.KAssetName,
                KPersonID = source.KPersonID,
                KPersonName = source.KPersonName,
                IsTemplate = source.IsTemplate,
                Notes = source.Notes,
                KAccountID = source.KAccountID,
                KClientID = source.KClientID,
                KHierarchyID = source.KHierarchyID,
                KAccountName = source.KAccountName,
                Units = source.Units,
                ChangeType = changeType
            };
        }
    }
}