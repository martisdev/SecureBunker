using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SecureBunkerCore;
using SecureBunkerCore.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using SecureBunker.ViewModels;
using NetCoreFileAccess.SourceAccess;
using NetCoreFileAccess;

namespace SecureBunker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        #region FIELDS
        private bool IsModified = false;

        private bool _closeConfirmed = false;

        #endregion

        #region CONSTRUCTOR
        public MainWindow()
        {
            InitializeComponent();

            // set DataContext so XAML bindings resolve
            this.DataContext = new MainViewModel();

            LoadICon();
            SetComboboxItemps();
            this.StatusBarBottom.Items[0] = $"Version: {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";
        }

        #endregion


        #region EVENTS

        private void LaunchOnGitHub_Click(object sender, RoutedEventArgs e)
        {
            LaunchURL(Manager.URL_GITHUB);            
        }


        private bool LaunchURL(string URL )
        {
            if (!string.IsNullOrEmpty(URL))
            {
                try
                {
                    var psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = URL,
                        UseShellExecute = true,
                        Verb = "open"
                    };
                    System.Diagnostics.Process.Start(psi);
                    return true;
                }
                catch (System.ComponentModel.Win32Exception ex)
                {
                    this.ShowMessageAsync("Error", $"Unable to open URL: {ex.Message}");
                    return false;
                }
            }
            else
            {
                this.ShowMessageAsync("Validation Error", "URL is empty.");
                return false;
            }
        }
        private void LaunchSettings_Click(object sender, RoutedEventArgs e)
        {
            //Open settings window
            this.settingsFlyout.IsOpen = !this.settingsFlyout.IsOpen;
        }

        private void LaunchURL_Click(object sender, RoutedEventArgs e)
        {
            LaunchURL(this.TxtURL.Text);            
        }
        private void TileNew_Click(object sender, RoutedEventArgs e)
        {
            SetStateControls(true,false);            
        }

        private void TextBoxButtonCmd(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.TxtURL.Text))
            {                
                try
                {
                    var psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "this.TxtURL.Text",
                        UseShellExecute = true,
                        Verb = "open"
                    };
                    System.Diagnostics.Process.Start(psi);
                }
                catch (System.ComponentModel.Win32Exception ex)
                {
                    this.ShowMessageAsync("Error", $"Unable to open URL: {ex.Message}");                    
                }
            }
        }

        private async void TileSave_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;
            var mySettings = new MetroDialogSettings
            {
                AffirmativeButtonText = "OK",
                AnimateShow = true,
                AnimateHide = false
            };

            if (lbNew.Visibility== Visibility.Visible)
            {
                if( string.IsNullOrEmpty(this.TxtName.Text))
                {
                    await this.ShowMessageAsync("Validation Error", "Name is required.", MessageDialogStyle.Affirmative, mySettings);
                    return;
                }
                DataItems NewItem = new DataItems()
                {
                    Name = this.TxtName.Text,
                    Description = this.TxtDescrip.Text,
                    URL = this.TxtURL.Text,
                    User = this.TxtUser.Text,
                    Password = this.txtPsw.Password,
                    OtherText = new TextRange(this.txtComment.Document.ContentStart, this.txtComment.Document.ContentEnd).Text
                };
                FileManipulation.ListItems.Add(NewItem);
            }
            else
            {
                // Fix: Check for null before calling ToString()
                if (this.cmbList.SelectedItem is string NameItem)
                {
                    DataItems? EditItem = FileManipulation.ListItems.FirstOrDefault(x => x.Name == NameItem);
                    if (EditItem != null)
                    {
                        EditItem.Name = this.TxtName.Text;
                        EditItem.Description = this.TxtDescrip.Text;
                        EditItem.URL = this.TxtURL.Text;
                        EditItem.User = this.TxtUser.Text;
                        EditItem.Password = this.txtPsw.Password;
                        EditItem.OtherText = new TextRange(this.txtComment.Document.ContentStart, this.txtComment.Document.ContentEnd).Text;
                    }
                    FileManipulation.ListItems.Remove(EditItem);
                    FileManipulation.ListItems.Add(EditItem);
                }
                else
                {
                    await this.ShowMessageAsync("Validation Error", "No item selected.", MessageDialogStyle.Affirmative, mySettings);
                    return;
                }
            }
                
            MemoryStream data = FileManipulation.EncryptDocument(Manager.User, Manager.Password);
            await Manager.SaveFile(data);
            

            SetComboboxItemps();
            SetStateControls(false, true);
            ProgressBar.Visibility = Visibility.Hidden;
        }

        private async void TileDel_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;
            var mySettings = new MetroDialogSettings
            {
                AffirmativeButtonText = "OK",
                AnimateShow = true,
                AnimateHide = false

            };
            // Fix: Check for null before calling ToString()
            if (this.cmbList.SelectedItem is string NameItem)
            {
                DataItems? DelItem = FileManipulation.ListItems.FirstOrDefault(x => x.Name == NameItem);
                if(DelItem != null)                
                    FileManipulation.ListItems.Remove(DelItem);

                MemoryStream data = FileManipulation.EncryptDocument(Manager.User, Manager.Password);
                bool ret = await Manager.SaveFile(data);

                if (!ret)
                    await this.ShowMessageAsync("Deleting Error", "Error saving data to source.", MessageDialogStyle.Affirmative, mySettings);
                
                SetComboboxItemps();
                SetStateControls(false, true);
            }
            else
            {
                await this.ShowMessageAsync("Deleting Error", "No item selected.", MessageDialogStyle.Affirmative, mySettings);
            }
            ProgressBar.Visibility = Visibility.Hidden;
        }

        private void Txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(this.TileSave != null)
                this.TileSave.IsEnabled = !string.IsNullOrEmpty(this.TxtName.Text);

            IsModified = true;
        }

        private void cmbList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cmbList.SelectedItem is string NameItem)
            {
                LoadItemDetails(NameItem);
                SetStateControls(false, false);
            }
        }

        private async void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // if we've already confirmed closing, let it proceed
            if (_closeConfirmed) return;

            if (IsModified)
            {
                // immediately cancel the close so async dialog can run safely
                e.Cancel = true;

                var mySettings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Yes",
                    NegativeButtonText = "No",
                    ColorScheme = MetroDialogOptions.ColorScheme,
                    DialogButtonFontSize = 20D
                };
                MessageDialogResult result = await this.ShowMessageAsync("Unsaved Changes", "You have unsaved changes. Do you want to save before exiting?", MessageDialogStyle.AffirmativeAndNegative, mySettings);
                if (result == MessageDialogResult.Affirmative)
                {
                    // Call the save method here
                    TileSave_Click(sender, new RoutedEventArgs());
                }

                _closeConfirmed = true;
                // call Close again to finish; handler will return immediately because of _closeConfirmed
                this.Close();

            }
        }

        private void txtPsw_PasswordChanged(object sender, RoutedEventArgs e)
        {
            IsModified = true;
        }

        private void btnConfSave_Click(object sender, RoutedEventArgs e)
        {
            this.settingsFlyout.IsOpen = false;

            // save settings to config file
            //\config\config.json
            if (this.cmbSource.SelectedItem is string selectedSource)
            {
                if (Enum.TryParse<SourceType>(selectedSource, out var sourceType))
                {
                    if(sourceType != SourceType.None)
                    {

                        // Save the selected source type to the configuration
                        Config.sourceType = sourceType;
                        
                        // FTP settings
                        Config.FTPConfig.Host = this.txtFtpHost.Text;
                        if (int.TryParse(this.txtFtpPort.Text, out int port))
                            Config.FTPConfig.Port = port;
                        Config.FTPConfig.Username = this.txtFtpUser.Text;                        
                        if (!string.IsNullOrEmpty(this.txtFtpPassword.Password))
                            Config.FTPConfig.Password = this.txtFtpPassword.Password;
                        Config.FTPConfig.PathFile = this.txtFtpPathFile.Text;
                        //google settings
                        Config.GoogleConfig.PathFile = this.txtGooglePathFile.Text;

                        // save configuration to file
                        Manager.SaveConfiguration();

                        var mySettings = new MetroDialogSettings
                        {
                            AffirmativeButtonText = "OK",
                            AnimateShow = true,
                            AnimateHide = false
                        };
                        this.ShowMessageAsync("Configuration saved", "If you have changed the connection type, you must restart the program", MessageDialogStyle.Affirmative, mySettings);
                    }                    
                }
            }
        }

        #endregion

        #region PRIVATE METHODS


        private void LoadICon()
        {
            // If the icon is in Properties.Resources (resx) as a System.Drawing.Icon named "bunker":
            var resIcon = Properties.Resources.Bunker; // check Resources.resx for exact name
            if (resIcon != null)
            {
                using (var ms = new MemoryStream(resIcon))
                {
                    var decoder = new IconBitmapDecoder(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                    this.Icon = decoder.Frames[0];
                }
            }
        }

        private void LoadItemDetails(string name)
        {
            var item = FileManipulation.ListItems.FirstOrDefault(x => x.Name == name);
            if (item != null)
            {
                this.TxtName.Text = item.Name;
                this.TxtDescrip.Text = item.Description;
                this.TxtURL.Text = item.URL;
                this.TxtUser.Text = item.User;
                this.txtPsw.Password = item.Password;
                this.txtComment.Document.Blocks.Clear();
                this.txtComment.Document.Blocks.Add(new Paragraph(new Run(item.OtherText)));
            }
        }

        private void SetStateControls( bool IsNew, bool Clear)
        {           
            this.lbNew.Visibility = IsNew ? Visibility.Visible: Visibility.Hidden ;
            this.cmbList.IsReadOnly = IsNew;
            this.TileNew.IsEnabled = !IsNew;
            this.TileDel.IsEnabled = !IsNew;
            this.TileSave.IsEnabled = false;

            if (IsNew | Clear)
            {
                this.TxtName.Text = string.Empty;
                this.TxtDescrip.Text = string.Empty;
                this.TxtURL.Text = string.Empty;
                this.TxtUser.Text = string.Empty;
                this.txtPsw.Password = string.Empty;
                this.txtComment.Document.Blocks.Clear();
            }            
            
            IsModified = false;
        }

        private void SetComboboxItemps()
        {
            this.cmbList.ItemsSource = FileManipulation.ListItems.Select(x => x.Name).OrderBy(x => x).ToList();
        }
        #endregion

        
    }
}
