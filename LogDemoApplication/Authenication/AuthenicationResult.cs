namespace LogDemoApplication.Authenication
{
    /// <summary>
    /// Represents the results of a login authenication.
    /// </summary>
    public class AuthenicationResult
    {
        /// <summary>
        /// Gets or sets whether the authenication succeeded.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the details of authenication to display.
        /// </summary>
        public string Message { get; set; }
    }
}
