using NetCoreFileAccess;
using NetCoreFileAccess.SourceAccess;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static NetCoreFileAccess.Config;

[assembly: InternalsVisibleTo("SecureBunker.UnitTests")]
namespace SecureBunkerCore
{
    public static class Manager
    {        
        #region CONST

        public const string URL_GITHUB = "https://github.com/martisdev/SecureBunker";

        private const string FILE_EXTENSION = ".fscr";

        /// <summary>
        /// set your own Google API credentials here. 
        /// You can create credentials for a desktop application in the Google Cloud Console.
        /// </summary>
        private const string GOOGLE_CLIENT_ID = "";

        /// <summary>
        /// set your own Google API credentials here. 
        /// You can create credentials for a desktop application in the Google Cloud Console.
        /// </summary>
        private const string GOOGLE_CLIENT_SECRET = "";

        /*             
        - The conditions are string must be between 4 and 20 characters long. 
        - string must contain at least one number. 
        - string must contain at least one uppercase letter. 
        - string must contain at least one lowercase letter                                
        - string must contain at least one special character (@$!%*#?&)
        */        
        private const string PASSWORD_PATTERN1 = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{4,20}$";

        #endregion

        #region FIELDS

        public static SourceType sourceType;

        internal static SourceAccess sourceAccess;

        #endregion

        #region PROPERTIES

        public static string User { get; private set; }
        
        public static string Password { get; private set; }


        #endregion

        #region PUBLIC METHODS

        public static void SetSource()
        {

            string AppName = Assembly.GetEntryAssembly().GetName().Name;
            Configurations.LoadConfigurationFile();            
            switch (Config.sourceType)
            {
                case SourceType.Local:
                    sourceAccess = new SourceAccess(new LocalAccess(AppName));
                    break;
                case SourceType.GoogleDrive:
                    sourceAccess = new SourceAccess(new GoogleDriveAccess(AppName.ToLower()));                    
                    break;
                case SourceType.Ftp:
                    sourceAccess = new SourceAccess(new FtpAccess(AppName));
                    break;
            }            
        }

        public static bool TryLogin()
        {
            bool result = false;
            
            switch (sourceType)
            {
                case SourceType.Local:
                    //Password validation for local access, for other types we can consider that the validation is done by the source access class                    
                    result = sourceAccess.TryLogin(PASSWORD_PATTERN1);
                    break;
                case SourceType.Ftp:
                    //FTP params
                    result = sourceAccess.TryLogin(
                                    PASSWORD_PATTERN1,
                                    FTPConfig.Host,
                                    FTPConfig.Username,
                                    FTPConfig.Password,
                                    FTPConfig.PathFile);
                    break;
                case SourceType.GoogleDrive:
                    
                    result = sourceAccess.TryLogin(
                                    PASSWORD_PATTERN1,
                                    Config.GoogleConfig.PathFile,
                                    GOOGLE_CLIENT_ID,
                                    GOOGLE_CLIENT_SECRET); 
                    break;
            }

            
            if(result && sourceAccess.IsInicializing)
                FileManipulation.CreateEmptyDocument(sourceAccess.UserName, sourceAccess.Password);

            User = sourceAccess.UserName;
            Password = sourceAccess.Password;
            
            return result;
        }
  
        public static async Task<bool> SaveFile(MemoryStream content)
        {
            if (content == null) return false;

            try
            {
                // SourceAccess.SaveFile is synchronous in the current design.
                // Run it on a thread-pool thread to avoid blocking UI callers.
                return await Task.Run(() => sourceAccess.SaveFile(content));
            }
            catch
            {
                return false;
            }
        }


        public static void GetSourceData()
        {
            //Get data from source and load into memory
            MemoryStream msData = sourceAccess.GetFileData();
            FileManipulation.LoadDocument(msData, sourceAccess.UserName, sourceAccess.Password);
        }


        public static void SaveConfiguration()
        {            
            Configurations.SaveConfigurationFile();
        }
        #endregion

    }
}
