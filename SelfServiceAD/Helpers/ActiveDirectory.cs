namespace SelfServiceAD.Helpers
{
    using System;
    using System.DirectoryServices.AccountManagement;
    using System.Runtime.InteropServices;

    using Models;

    public class ActiveDirectory : IDisposable
    {
        private string Username { get; set; }

        /// <summary>
        /// Instatnates a new ActiveDirectory object
        /// </summary>
        /// <param name="userName">Username to take action with</param>
        public ActiveDirectory(string userName)
        {
            Username = userName;
        }

        private bool _disposed;
        IntPtr _token;
        
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(
            string username,
            string domain,
            string password,
            int logonType,
            int logonProvider,
            out IntPtr token
            );

        [DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);

        /// <summary>
        /// Attempt to logon to the Windows domain the server is apart of
        /// </summary>
        /// <param name="password">Password for the user</param>
        /// <returns></returns>
        public WindowsLogonResponse Logon(string password)
        {
            // using Pinvoke for this so we can capture the expired password and forced password change
            // since LDAP and PrinicpialContext.ValidateCredentials() will fail if those conditions exist
            if (LogonUser(
                Username,
                "",
                password,
                (int)LogonType.LOGON32_LOGON_NETWORK,
                (int)LogonProvider.LOGON32_PROVIDER_DEFAULT,
                out _token))
            {
                return WindowsLogonResponse.Successful;
            }

            var error = Marshal.GetLastWin32Error();

            // 1907 = ERROR_PASSWORD_MUST_CHANGE
            // 1330 = ERROR_PASSWORD_EXPIRED
            switch (error)
            {
                case 1907:
                case 1330:
                    return WindowsLogonResponse.PasswordChangeRequired;
                case 1909:
                    return WindowsLogonResponse.LockedOut;
            }

            return WindowsLogonResponse.Invalid;
        }

        /// <summary>
        /// Attempt to change the password of the user
        /// </summary>
        /// <param name="oldPassword">The current/old password of the user </param>
        /// <param name="newPassword">The new password</param>
        public void ChangePassword(string oldPassword, string newPassword)
        {
            using (var pc = new PrincipalContext(ContextType.Domain))
            {
                var user = UserPrincipal.FindByIdentity(pc, Username);
                if(user != null)
                    user.ChangePassword(oldPassword, newPassword);
            }
        }

        /// <summary>
        /// Determine if the user is allowed to change their password based on rules
        /// </summary>
        /// <returns>True if the user cannot change their password, false if the password change is allowed</returns>
        public bool UserCannotChangePassword()
        {
            using (var pc = new PrincipalContext(ContextType.Domain))
            {
                var user = UserPrincipal.FindByIdentity(pc, Username);
                return user == null || user.UserCannotChangePassword;
            }
        }

        public UserPrincipal GetUserPrincipal()
        {
            using (var pc = new PrincipalContext(ContextType.Domain))
            {
                return UserPrincipal.FindByIdentity(pc, Username);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this._disposed)
            {
                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.
                CloseHandle(_token);
                _token = IntPtr.Zero;

                // Note disposing has been done.
                _disposed = true;

            }
        }

        ~ActiveDirectory()
        {
            Dispose(false);
        }
    }
}