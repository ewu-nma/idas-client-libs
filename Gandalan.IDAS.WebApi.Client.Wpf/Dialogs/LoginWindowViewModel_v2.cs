﻿using Gandalan.IDAS.Client.Contracts.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Gandalan.IDAS.WebApi.Client.Wpf.Dialogs
{
    public class LoginWindowViewModel_v2 : INotifyPropertyChanged
    {
        private string _url;
        private string _userName;
        private string _passwort;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Title => "Neher Cloud - Anmeldung";
        public bool LoginInProgress { get; set; } = false;
        public bool ShowServerSelection { get; set; }

        public bool EnvAlsDefault { get; set; } = false;

        public string Url { get { return _url; } set { _url = value; SetOrClearMessage("Url", string.IsNullOrEmpty(_url), "Ungültige Serveradresse"); } }

        private void SetOrClearMessage(string v1, bool v2, string v3)
        {
            // TODO
        }

        public string UserName { get { return _userName; } set { _userName = value; SetOrClearMessage("UserName", string.IsNullOrEmpty(_userName), "Ungültiger Benutzername"); } }
        public string Passwort { get { return _passwort; } set { _passwort = value; SetOrClearMessage("Passwort", string.IsNullOrEmpty(_passwort), "Ungültiges Passwort"); } }
        public bool SaveCredentials { get; set; } = true;

        public IEnumerable<IWebApiConfig> AlleEnvironments { get; private set; }
        public IWebApiConfig ServerEnvironment { get; set; }
        public IEnumerable<IWebApiConfig> LoggedInEnvironments { get; set; }

        public bool ShowLoggedInEnvironments { get; set; }
        public bool ShowLoginFields => !ShowLoggedInEnvironments;
        public string StatusText { get; set; }
        
        public LoginWindowViewModel_v2(IWebApiConfig webApiSettings)
        {
            AlleEnvironments = webApiSettings.GetAll();
            LoggedInEnvironments = AlleEnvironments.Where(e => e.AuthToken != null && e.AuthToken.Token != Guid.Empty);
            ShowLoggedInEnvironments = LoggedInEnvironments.Any();
            ServerEnvironment = AlleEnvironments.FirstOrDefault(e => e.FriendlyName.Equals(webApiSettings.FriendlyName, StringComparison.InvariantCultureIgnoreCase));

#if DEBUG
            ShowServerSelection = true;
#endif
        }

//        public LoginWindowViewModel_v2()
//        {
//#if DEBUG
//            ShowServerSelection = true;
//#endif
//        }
    }
}
