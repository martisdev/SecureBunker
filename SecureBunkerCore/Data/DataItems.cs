using System;

namespace SecureBunkerCore.Data
{
    
    [Serializable]
    public class DataItems
    {
        #region FIELDS

        #endregion

        #region PROPERTIES

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name == value)
                    return;
                //TODO check a unique name.
                _Name = value;
            }
        }

        private string _Description;

        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description == value)
                    return;
                //TODO check a unique name.
                _Description = value;
            }
        }

        private string _OtherText;

        public string OtherText
        {
            get { return _OtherText; }
            set
            {
                if (_OtherText == value)
                    return;
                //TODO check a unique name.
                _OtherText = value;
            }
        }

        private string _User;

        public string User
        {
            get { return _User; }
            set
            {
                if (_User == value)
                    return;
                //TODO check a unique name.
                _User = value;
            }
        }

        private string _Password;

        public string Password
        {
            get { return _Password; }
            set
            {
                if (_Password == value)
                    return;
                //TODO check a unique name.
                _Password = value;
            }
        }

        private string _URL;

        public string URL
        {
            get { return _URL; }
            set
            {
                if (_URL == value)
                    return;
                //TODO check a unique name.
                _URL = value;
            }
        }

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataItems()
        {

        }

        public DataItems(DataItems DataEncrypt)
        {
            Name = DataEncrypt.Name;
            Description = DataEncrypt.Description;
            User = DataEncrypt.URL;
            Password = DataEncrypt.Password;
            URL = DataEncrypt.URL;
            OtherText = DataEncrypt.OtherText;
        }

        public DataItems(string mName, string mDescription, string mUser, string mPassword, string mUrl, string mOthers)
        {
            Name = mName;
            Description = mDescription;
            User = mUser;
            Password = mPassword;
            URL = mUrl;
            OtherText = mOthers;
        }

        #endregion

        #region METHODS

        public override string ToString()
        {
            return _Name;
        }

        #endregion
    }
}
