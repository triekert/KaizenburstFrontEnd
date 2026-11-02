using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Fasetto.Word
{
    /// <summary>
    /// The view model for a datetime entry, accepting an OriginalDateTime 
    /// and consolidating the edits into a single EditedDateTime output.
    /// </summary>
    public class DateTimeViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The label to identify what this value is for
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The initial/original saved datetime value passed into the control
        /// </summary>
        public DateTime OriginalDateTime { get; set; }

        /// <summary>
        /// The single unified result of the edited date and time
        /// </summary>
        public DateTime EditedDateTime { get; set; }

        /// <summary>
        /// Currently selected time string option from the dropdown
        /// </summary>
        public string SelectedTime { get; set; }

        /// <summary>
        /// Available time blocks in 30-minute increments for the ComboBox dropdown
        /// </summary>
        public ObservableCollection<string> AvailableTimes { get; set; }

        /// <summary>
        /// Indicates if the current datetime is in edit mode
        /// </summary>
        public bool Editing { get; set; }

        /// <summary>
        /// Indicates if the current control is pending an update (in progress)
        /// </summary>
        public bool Working { get; set; }

        /// <summary>
        /// The action to run when saving the value.
        /// Returns true if the commit was successful, or false otherwise.
        /// </summary>
        public Func<Task<bool>> CommitAction { get; set; }

        #endregion

        #region Public Commands

        public ICommand EditCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        #endregion

        #region Constructor 

        public DateTimeViewModel()
        {
            EditCommand = new RelayCommand(Edit);
            CancelCommand = new RelayCommand(Cancel);
            SaveCommand = new RelayCommand(Save);

            // Initialize 30-minute increment collection as strings
            AvailableTimes = new ObservableCollection<string>();
            for (var hour = 0; hour < 24; hour++)
            {
                for (var minute = 0; minute < 60; minute += 30)
                {
                    var timeString = $"{hour:D2}:{minute:D2}";
                    AvailableTimes.Add(timeString);
                }
            }
        }

        #endregion

        #region Command Methods

        public void Edit()
        {
            // Set the working EditedDateTime to match the original starting point
            EditedDateTime = OriginalDateTime;

            // Pre-select the closest matching time block string
            MatchClosestTimeBlock(EditedDateTime);

            Editing = true;
        }

        public void Cancel()
        {
            Editing = false;
        }

        public void Save()
        {
            var result = default(bool);
            var previousDateTime = OriginalDateTime;

            RunCommandAsync(() => Working, async () =>
            {
                Editing = false;

                // Extract date part from EditedDateTime and combine it with the selected time string
                var datePart = EditedDateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                var timePart = SelectedTime ?? EditedDateTime.ToString("HH:mm", CultureInfo.InvariantCulture);

                if (DateTime.TryParse($"{datePart} {timePart}", CultureInfo.InvariantCulture, DateTimeStyles.None, out var finalParsedDateTime))
                {
                    EditedDateTime = finalParsedDateTime;
                }

                // Update original to match the newly committed single datetime output
                OriginalDateTime = EditedDateTime;

                // Execute commit action if provided
                result = CommitAction == null ? true : await CommitAction();

            }).ContinueWith(t =>
            {
                if (!result)
                {
                    // Revert back if commit failed
                    OriginalDateTime = previousDateTime;
                    EditedDateTime = previousDateTime;
                    Editing = true;
                }
            });
        }

        private void MatchClosestTimeBlock(DateTime targetTime)
        {
            var totalMinutes = targetTime.Hour * 60 + targetTime.Minute;
            var remainder = totalMinutes % 30;

            if (remainder >= 15) totalMinutes += (30 - remainder);
            else totalMinutes -= remainder;

            if (totalMinutes >= 1440) totalMinutes = 0;

            var targetString = $"{totalMinutes / 60:D2}:{totalMinutes % 60:D2}";

            // Since AvailableTimes contains strings, loop directly through strings
            foreach (var timeString in AvailableTimes)
            {
                if (timeString == targetString)
                {
                    SelectedTime = timeString;
                    break;
                }
            }
        }

        #endregion
    }
}