using System.Windows;

namespace Demo.Presentation.Dialog
{
    /// <summary>
    /// An implementation of <see cref="IDialogService"/> that displays <see cref="MessageBoxResult"/>.
    /// </summary>
    public class DialogService : IDialogService
    {
        /// <summary>
        /// Shows a message in a WPF message box.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public void ShowMessage(string message)
        {
            ShowMessage(message, true);
        }

        /// <summary>
        /// Shows a message in WPF message box with a supplied boolean indicating if
        /// the action producing the message was successful or failed.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="successful">Indicates if action that produced the message was successful or failed.</param>
        public void ShowMessage(string message, bool successful)
        {
            if(successful)
            {
                MessageBox.Show(message, string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(message, string.Empty, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
