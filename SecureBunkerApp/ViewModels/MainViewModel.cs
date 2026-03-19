using NetCoreFileAccess;
using NetCoreFileAccess.SourceAccess;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SecureBunker.ViewModels
{
    public class MainViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<string> ListOfSources { get; }
        
        private string _selectedSource = string.Empty;
        
        public string SelectedSource
        {
            get => _selectedSource;
            set
            {
                if (_selectedSource == value) return;
                _selectedSource = value;
                OnPropertyChanged(nameof(SelectedSource));
            }
        }

        public string FtpHost
        {
            get => Config.FTPConfig.Host ?? string.Empty;
            set
            {
                if (Config.FTPConfig.Host == value) return;
                Config.FTPConfig.Host = value ?? string.Empty;
                OnPropertyChanged(nameof(FtpHost));
            }
        }

        // Bind port as string for easy XAML TextBox binding and parse on set
        public string FtpPort
        {
            get => Config.FTPConfig.Port.ToString();
            set
            {
                if (int.TryParse(value, out var p))
                {
                    if (Config.FTPConfig.Port != p)
                    {
                        Config.FTPConfig.Port = p;
                        OnPropertyChanged(nameof(FtpPort));
                    }
                }
                else
                {
                    // optional: ignore invalid parse or set to default
                }
            }
        }

        public string FtpUsername
        {
            get => Config.FTPConfig.Username ?? string.Empty;
            set
            {
                if (Config.FTPConfig.Username == value) return;
                Config.FTPConfig.Username = value ?? string.Empty;
                OnPropertyChanged(nameof(FtpUsername));
            }
        }

        public string FtpPathFile
        {
            get => Config.FTPConfig.PathFile ?? string.Empty;
            set
            {
                if (Config.FTPConfig.PathFile == value) return;
                Config.FTPConfig.PathFile = value ?? string.Empty;
                OnPropertyChanged(nameof(FtpPathFile));
            }
        }

        public string GooglePathFile
        {
            get => Config.GoogleConfig.PathFile ?? string.Empty;
            set
            {
                if (Config.GoogleConfig.PathFile == value) return;
                Config.GoogleConfig.PathFile = value ?? string.Empty;
                OnPropertyChanged(nameof(GooglePathFile));
            }
        }

        public MainViewModel()
        {
            // expose the SourceType names as the source list
            ListOfSources = new ObservableCollection<string>(Enum.GetNames(typeof(SourceType)));
            ListOfSources.Remove(SourceType.None.ToString());
            // initialize selected from current config
            SelectedSource = Config.sourceType.ToString();
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
