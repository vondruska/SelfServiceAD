namespace SelfServiceAD.Helpers
{
    using System;
    using System.DirectoryServices.AccountManagement;
    using System.Runtime.InteropServices;

    using Models;

    public class ActiveDirectory : IDisposable
    {
        private string Username { get; set; }

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

        public WindowsLogonResponse Logon(string password)
        {
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

            // 1907 is ERROR_PASSWORD_MUST_CHANGE
            // 1330 is ERROR_PASSWORD_EXPIRED
            switch (error)
            {
                case 1907:
                case 1330:
                    return WindowsLogonResponse.PasswordChangeRequired;
            }

            return WindowsLogonResponse.Invalid;
        }

        public void ChangePassword(string currentPassword, string newPassword)
        {
            using (var pc = new PrincipalContext(ContextType.Domain))
            {
                var user = UserPrincipal.FindByIdentity(pc, Username);
                if(user != null)
                    user.ChangePassword(currentPassword, newPassword);
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