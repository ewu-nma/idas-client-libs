﻿using Gandalan.IDAS.WebApi.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gandalan.IDAS.Client.Contracts.Contracts
{
    public interface IWebApiConfig
    {
        string Url { get; set; }
        string UserName { get; set; }
        [JsonIgnore]
        string Passwort { get; set; }
        [JsonIgnore]
        string Mandant { get; set; }
        UserAuthTokenDTO AuthToken { get; set; }
        [JsonIgnore]
        Guid AppToken { get; set; }
        string FriendlyName { get; set; }
        string DisplayText { get; }

        [JsonIgnore]
        Guid InstallationId { get; set; }
        [JsonIgnore]
        string UserAgent { get; set; }
        [JsonIgnore]
        bool UseCompression { get; set; }
        /// <summary>
        /// CMSUrl für Dokumente
        /// </summary>
        string DocUrl { get; set; }
        /// <summary>
        /// CMS für Variantenspez
        /// </summary>
        string CMSUrl { get; set; }
        /// <summary>
        /// Package Store
        /// </summary>
        string StoreUrl { get; set; }
        /// <summary>
        /// Feedback-Tool
        /// </summary>
        string FeedbackUrl { get; set; }
        string NotifyUrl { get; set; }
        string HelpCenterUrl { get; set; }

        void Save();

        List<IWebApiConfig> GetAll();

        void CopyToThis(IWebApiConfig settings);
    }
}
