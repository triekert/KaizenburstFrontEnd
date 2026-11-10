using Dna;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Fasetto.Word.Core;using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Fasetto.Word
{
    public class ClassificationContext
    {
        public TransactionViewModel Selected { get; set; }
        public TransactionViewModel Selected1 { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionDescription { get; set; }
        public string TransactionNotes { get; set; }
        public string AllocationText { get; set; }
        public string UnitsText { get; set; }
        public bool IsTemplate { get; set; }

        // Selection ViewModels containing EditedKid / EditedName
        public HierarchyItemSelectionViewModel Category { get; set; }
        public HierarchyItemSelectionViewModel Party { get; set; }
        public HierarchyItemSelectionViewModel Account { get; set; }
        public HierarchyItemSelectionViewModel Project { get; set; }
        public HierarchyItemSelectionViewModel Asset { get; set; }
        public HierarchyItemSelectionViewModel Person { get; set; }

        // Collections
        public ObservableCollection<TransactionViewModel> MasterTransactions { get; set; }
        public ObservableCollection<TransactionViewModel> DetailTransactions { get; set; }
        public TransactionResultListApiModel ChangeList { get; set; }
        public List<DocDataViewModel> Documents { get; set; }
    }
}
