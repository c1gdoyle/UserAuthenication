using System.Windows;

namespace Demo.Presentation.Dialog
{
    /// <summary>
    /// Defines behaviour of a simple dialog service for displaying <see cref="MessageBoxResult"/>.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Shows a message in a WPF message box.
        /// </summary>
        /// <param name="message">The message to display.</param>
        void ShowMessage(string message);

        /// <summary>
        /// Shows a message in WPF message box with a supplied boolean indicating if
        /// the action producing the message was successful or failed.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="successful">Indicates if action that produced the message was successful or failed.</param>
        void ShowMessage(string message, bool successful);
    }
}
