// *****************************************************************************
// Gandalan GmbH & Co. KG - (c) 2017
// *****************************************************************************
// Middleware//Gandalan.IDAS.WebApi.Client//WebRoutinenBase.cs
// Created: 27.01.2016
// Edit: phil - 21.02.2017
// *****************************************************************************

using Gandalan.IDAS.Client.Contracts.Contracts;
using Gandalan.IDAS.Logging;
using Gandalan.IDAS.Web;
using Gandalan.IDAS.WebApi.Client.Settings;
using Gandalan.IDAS.WebApi.DTO;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Gandalan.IDAS.WebApi.Client
{
    public class WebRoutinenBase : RESTClientBase
    {
        #region  Felder

        public IWebApiConfig Settings;
        public bool IsJwt;

        #endregion

        #region  Eigenschaften

        public UserAuthTokenDTO AuthToken { get; set; }
        public string Status { get; private set; }
        public bool IgnoreOnErrorOccured { get; set; }
        public string JwtToken { get; set; }

        #endregion

        public static event ErrorOccuredEventHandler ErrorOccured;

        public delegate void ErrorOccuredEventHandler(object sender, ApiErrorArgs e);

        public WebRoutinenBase(IWebApiConfig settings)
        {
            // Settings werden kopiert, damit die abgeleiteten Klassen ggf. eigene Settings ergänzen/
            // ändern können (vor allem z.B. Base-URL)
            if (settings != null)
            {
                Settings = new WebApiSettings()
                {
                    AppToken = settings.AppToken,
                    AuthToken = settings.AuthToken,
                    FriendlyName = settings.FriendlyName,
                    Mandant = settings.Mandant,
                    Passwort = settings.Passwort,
                    Url = settings.Url,
                    UserName = settings.UserName,
                    InstallationId = settings.InstallationId,
                    UserAgent = settings.UserAgent,
                    UseCompression = settings.UseCompression
                };

                if (settings is IJwtWebApiConfig)
                {
                    IsJwt = true;
                    JwtToken = ((IJwtWebApiConfig)settings).JwtToken;
                }
            }

            if (settings?.AuthToken?.Token != Guid.Empty && settings?.AuthToken?.Expires > DateTime.UtcNow)
            {
                AuthToken = settings.AuthToken;
            }
        }

        protected virtual void OnErrorOccured(ApiErrorArgs e)
        {
            if (e.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new ApiUnauthorizedException(Status = e.Message);
            }

            ErrorOccured?.Invoke(this, e);
        }

        /// <summary>
        /// Meldet mit den aktuellen Credentials am Endpunkt /api/Login/Authenticate an. Wenn ein AuthToken vorhanden ist, wird
        /// zunächst versucht, dieses zu verwenden und ggf. zu updaten. Wenn das fehlschlägt, erfolgt eine Anmeldung mit Username/
        /// Passwort aus den Settings.
        /// </summary>
        /// <returns>Status des Logins</returns>
        public bool Login()
        {
            if (IsJwt)
            {
                return CheckJwtToken();
            }

            try
            {
                UserAuthTokenDTO result = null;
                if (AuthToken == null || AuthToken.Expires < DateTime.UtcNow)
                {
                    var ldto = new LoginDTO()
                    {
                        Email = Settings.UserName,
                        Password = Settings.Passwort,
                        AppToken = Settings.AppToken
                    };
                    result = Post<UserAuthTokenDTO>("/api/Login/Authenticate", ldto);
                }
                else
                {
                    if (AuthToken.Expires < DateTime.UtcNow.AddHours(6))
                    {
                        result = Put<UserAuthTokenDTO>("/api/Login/Update", AuthToken);
                    }
                    else
                    {
                        Status = "OK (Cached)";
                        return true;
                    }
                }

                if (result != null)
                {
                    AuthToken = result;
                    Status = "OK";
                    return true;
                }

                AuthToken = null;
                Status = "Error";
                return false;
            }
            catch (ApiException apiex)
            {
                Status = apiex.Message;
                if (Status.ToLower().Contains("<title>"))
                {
                    Status = InternalStripHtml(Status);
                }

                if (apiex.InnerException != null)
                    Status += " - " + apiex.InnerException.Message;
                AuthToken = null;
                return false;
            }
            catch (Exception ex)
            {
                Status = ex.Message;
                AuthToken = null;
                return false;
            }
        }

        public async Task<bool> LoginAsync()
        {
            if (IsJwt)
            {
                return await CheckJwtTokenAsync();
            }

            try
            {
                UserAuthTokenDTO result = null;
                if (AuthToken == null || AuthToken.Expires < DateTime.UtcNow)
                {
                    var ldto = new LoginDTO()
                    {
                        Email = Settings.UserName,
                        Password = Settings.Passwort,
                        AppToken = Settings.AppToken
                    };
                    result = await PostAsync<UserAuthTokenDTO>("/api/Login/Authenticate", ldto);
                }
                else
                {
                    if (AuthToken.Expires < DateTime.UtcNow.AddHours(6))
                    {
                        result = await PutAsync<UserAuthTokenDTO>("/api/Login/Update", AuthToken);
                    }
                    else
                    {
                        Status = "OK (Cached)";
                        return true;
                    }
                }

                if (result != null)
                {
                    AuthToken = result;
                    Status = "OK";
                    return true;
                }

                AuthToken = null;
                Status = "Error";
                return false;
            }
            catch (ApiException apiex)
            {
                Status = apiex.Message;
                if (Status.ToLower().Contains("<title>"))
                {
                    Status = InternalStripHtml(Status);
                }

                if (apiex.InnerException != null)
                    Status += " - " + apiex.InnerException.Message;
                AuthToken = null;
                return false;
            }
            catch (Exception ex)
            {
                Status = ex.Message;
                AuthToken = null;
                return false;
            }
        }

        public UserAuthTokenDTO RefreshToken(Guid authTokenGuid)
        {
            try
            {
                return Put<UserAuthTokenDTO>("/api/Login/Update", new UserAuthTokenDTO() {Token = authTokenGuid});
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<UserAuthTokenDTO> RefreshTokenAsync(Guid authTokenGuid)
        {
            try
            {
                return await PutAsync<UserAuthTokenDTO>("/api/Login/Update", new UserAuthTokenDTO() { Token = authTokenGuid });
            }
            catch (Exception)
            {
                return null;
            }
        }

        public T Post<T>(string uri, object data, JsonSerializerSettings settings = null)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return cl.Post<T>(uri, data, settings);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri, data);
                }
            }
        }

        public async Task<T> PostAsync<T>(string uri, object data, JsonSerializerSettings settings = null)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return await cl.PostAsync<T>(uri, data, settings);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri, data);
                }
            }
        }

        public string Post(string uri, object data, JsonSerializerSettings settings = null)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return cl.Post(uri, data, settings);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri, data);
                }
            }
        }

        public async Task<string> PostAsync(string uri, object data, JsonSerializerSettings settings = null)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return await cl.PostAsync(uri, data, settings);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri, data);
                }
            }
        }

        public byte[] PostData(string uri, byte[] data)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return cl.PostData(uri, data);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri, data);
                }
            }
        }

        public async Task<byte[]> PostDataAsync(string uri, byte[] data)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return await cl.PostDataAsync(uri, data);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri, data);
                }
            }
        }

        public string Get(string uri)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return cl.Get(uri);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri);
                }
            }
        }

        public async Task<string> GetAsync(string uri)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return await cl.GetAsync(uri);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri);
                }
            }
        }

        public byte[] GetData(string uri)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return cl.GetData(uri);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri);
                }
            }
        }

        public async Task<byte[]> GetDataAsync(string uri)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return await cl.GetDataAsync(uri);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri);
                }
            }
        }

        public T Get<T>(string uri, JsonSerializerSettings settings = null)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return cl.Get<T>(uri, settings);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri);
                }
            }
        }

        public async Task<T> GetAsync<T>(string uri, JsonSerializerSettings settings = null)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return await cl.GetAsync<T>(uri, settings);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri);
                }
            }
        }

        public T Put<T>(string uri, object data, JsonSerializerSettings settings = null)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return cl.Put<T>(uri, data, settings);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri, data);
                }
            }
        }

        public async Task<T> PutAsync<T>(string uri, object data, JsonSerializerSettings settings = null)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return await cl.PutAsync<T>(uri, data, settings);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri, data);
                }
            }
        }

        public string Put(string uri, object data, JsonSerializerSettings settings = null)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return cl.Put(uri, data, settings);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri, data);
                }
            }
        }

        public async Task<string> PutAsync(string uri, object data, JsonSerializerSettings settings = null)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return await cl.PutAsync(uri, data, settings);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri, data);
                }
            }
        }

        public byte[] PutData(string uri, byte[] data)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return cl.PutData(uri, data);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri, data);
                }
            }
        }

        public async Task<byte[]> PutDataAsync(string uri, byte[] data)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return await cl.PutDataAsync(uri, data);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri, data);
                }
            }
        }

        public T Delete<T>(string uri, object data, JsonSerializerSettings settings = null)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return cl.Delete<T>(uri, data, settings);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri, data);
                }
            }
        }

        public async Task<T> DeleteAsync<T>(string uri, object data, JsonSerializerSettings settings = null)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return await cl.DeleteAsync<T>(uri, data, settings);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri, data);
                }
            }
        }

        public T Delete<T>(string uri, JsonSerializerSettings settings = null)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return cl.Delete<T>(uri, settings);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri);
                }
            }
        }

        public async Task<T> DeleteAsync<T>(string uri, JsonSerializerSettings settings = null)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return await cl.DeleteAsync<T>(uri, settings);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri);
                }
            }
        }

        public string Delete(string uri)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return cl.Delete(uri);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri);
                }
            }
        }

        public async Task<string> DeleteAsync(string uri)
        {
            using (var cl = new RESTRoutinen(Settings.Url))
            {
                try
                {
                    SetupRestRoutinen(cl);

                    return await cl.DeleteAsync(uri);
                }
                catch (WebException ex)
                {
                    throw HandleWebException(ex, uri);
                }
            }
        }

        private static string InternalStripHtml(string htmlString)
        {
            var result = htmlString;
            if (result.ToLower().Contains("<title>") && result.ToLower().Contains("</title>"))
            {
                var start = result.IndexOf("<title>") + 7;
                var end = result.IndexOf("</title>");
                if (end > start)
                    result = $"Interner Serverfehler (\"{result.Substring(start, end - start)}\"). Bitte versuchen Sie es zu einem späteren Zeitpunkt erneut.";
            }
            return result;
        }

        private void SetupRestRoutinen(RESTRoutinen cl)
        {
            if (IsJwt)
            {
                if (!string.IsNullOrEmpty(JwtToken))
                {
                    cl.AdditionalHeaders.Add("Authorization: Bearer " + JwtToken);
                }
            }
            else
            {
                if (AuthToken != null)
                {
                    cl.AdditionalHeaders.Add("X-Gdl-AuthToken: " + AuthToken.Token);
                }
            }

            if (Settings.InstallationId != Guid.Empty)
            {
                cl.AdditionalHeaders.Add("X-Gdl-InstallationId: " + Settings.InstallationId);
            }

            if (!string.IsNullOrEmpty(Settings.UserAgent))
            {
                cl.UserAgent = Settings.UserAgent;
            }

            if (Settings.UseCompression)
            {
                cl.UseCompression = Settings.UseCompression;
            }
        }

        private bool CheckJwtToken()
        {
            if (CheckJwtTokenInternal(out var refreshToken, out bool checkResult))
            {
                return checkResult;
            }

            try
            {
                // refresh JWT using refresh token
                var newJwt = Put<string>("/api/LoginJwt/Refresh", new { Token = refreshToken });
                JwtToken = newJwt;
                return true;
            }
            catch (ApiException apiex)
            {
                Status = apiex.Message;
                if (Status.ToLower().Contains("<title>"))
                {
                    Status = InternalStripHtml(Status);
                }

                if (apiex.InnerException != null)
                {
                    Status += " - " + apiex.InnerException.Message;
                }

                JwtToken = null;
                return false;
            }
            catch (Exception ex)
            {
                Status = ex.Message;
                JwtToken = null;
                return false;
            }
        }

        private async Task<bool> CheckJwtTokenAsync()
        {
            if (CheckJwtTokenInternal(out var refreshToken, out bool checkResult))
            {
                return checkResult;
            }

            try
            {
                // refresh JWT using refresh token
                var newJwt = await PutAsync<string>("/api/LoginJwt/Refresh", new { Token = refreshToken });
                JwtToken = newJwt;
                return true;
            }
            catch (ApiException apiex)
            {
                Status = apiex.Message;
                if (Status.ToLower().Contains("<title>"))
                {
                    Status = InternalStripHtml(Status);
                }

                if (apiex.InnerException != null)
                {
                    Status += " - " + apiex.InnerException.Message;
                }

                JwtToken = null;
                return false;
            }
            catch (Exception ex)
            {
                Status = ex.Message;
                JwtToken = null;
                return false;
            }
        }

        private bool CheckJwtTokenInternal(out string refreshToken, out bool checkResult)
        {
            refreshToken = null;
            var tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(JwtToken))
            {
                // unreadable
                checkResult = false;
                return true;
            }

            var jwtToken = tokenHandler.ReadJwtToken(JwtToken);
            if (jwtToken.ValidTo >= DateTime.UtcNow)
            {
                // token is not expired
                checkResult = true;
                return true;
            }

            var refreshTokenClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "refreshToken");
            refreshToken = refreshTokenClaim?.Value;
            Guid.TryParse(refreshToken, out Guid refreshTokenGuid);

            if (refreshTokenClaim == null ||
                refreshTokenGuid == Guid.Empty)
            {
                // JWT is expired and has no refreshToken
                checkResult = false;
                return true;
            }

            // token is good - return "false" for further processing
            checkResult = true;
            return false;
        }

        private ApiException HandleWebException(WebException ex, string url)
        {
            ApiException exception = TranslateException(ex);
            return InternalHandleWebException(exception, url);
        }

        private ApiException HandleWebException(WebException ex, string url, object data)
        {
            ApiException exception = TranslateException(ex, data);
            return InternalHandleWebException(exception, url);
        }

        private ApiException InternalHandleWebException(ApiException exception, string url)
        {
            if (!IgnoreOnErrorOccured)
            {
                OnErrorOccured(new ApiErrorArgs(exception.Message, exception.StatusCode));
            }

            var foundUrlInData = false;
            object dataBaseUrl = null;
            object dataUrl = null;
            object dataCallMethod = null;

            // Check if we already have data from RESTRoutinen.AddInfoToException()
            if (!exception.Data.Contains("URL"))
            {
                var innerException = exception.InnerException;
                while (innerException != null)
                {
                    if (innerException.Data.Contains("URL"))
                    {
                        dataBaseUrl = innerException.Data["BaseUrl"];
                        dataUrl = innerException.Data["URL"];
                        dataCallMethod = innerException.Data["CallMethod"];

                        foundUrlInData = true;
                    }

                    innerException = innerException.InnerException;
                }
            }
            else
            {
                dataBaseUrl = exception.Data["BaseUrl"];
                dataUrl = exception.Data["URL"];
                dataCallMethod = exception.Data["CallMethod"];

                foundUrlInData = true;
            }

            if (!foundUrlInData)
            {
                var callerMethodName = new StackTrace().GetFrame(2)?.GetMethod()?.Name;

                exception.Data.Add("BaseUrl", Settings.Url);
                exception.Data.Add("URL", url);
                exception.Data.Add("CallMethod", callerMethodName);

                dataBaseUrl = exception.Data["BaseUrl"];
                dataUrl = exception.Data["URL"];
                dataCallMethod = exception.Data["CallMethod"];
            }

            L.Fehler(exception, $"Exception data: BaseUrl: {dataBaseUrl} URL: {dataUrl} CallMethod: {dataCallMethod}{Environment.NewLine}");

            return exception;
        }
    }
}
