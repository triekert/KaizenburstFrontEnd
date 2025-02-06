using System;
using System.Windows.Input;

namespace Fasetto.Word
{
    internal class DelegateCommand<T> : ICommand
{
    private readonly Action<object> mExecute;
    private readonly Func<object, bool> mCanExecute;

    public event EventHandler CanExecuteChanged;

    public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
    {
        mExecute = execute ?? throw new ArgumentNullException(nameof(execute));
        mCanExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return mCanExecute == null || mCanExecute(parameter);
    }

    public void Execute(object parameter)
    {
        mExecute(parameter);
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
}