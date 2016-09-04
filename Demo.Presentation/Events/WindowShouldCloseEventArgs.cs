using System;

namespace Demo.Presentation.Events
{
    public class WindowShouldCloseEventArgs : EventArgs
    {
        internal WindowShouldCloseEventArgs(bool? dialogResult)
        {
            DialogResult = dialogResult;
        }

        public bool? DialogResult
        {
            get;
            private set;
        }
    }
}
