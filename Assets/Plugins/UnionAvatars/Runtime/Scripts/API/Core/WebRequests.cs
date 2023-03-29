using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace UnionAvatars.API
{
    public class WebRequests
    {
        public static async Task<WebResponse<T>> Get<T>(string endpoint, KeyValuePair<string, string>[] parameters, SessionContext sessionContext,
                                                        CancellationToken overrideCancellationToken = default)
        {
            if (sessionContext == null)
                throw new APIOperationFailed("No Session provided when trying to access: " + endpoint);

            endpoint = AddQuery(endpoint, parameters);
            using UnityWebRequest webRequest = UnityWebRequest.Get(endpoint);

            if (sessionContext.UserToken != null)
                webRequest.SetRequestHeader("Authorization", sessionContext.UserToken.TokenType + " " + sessionContext.UserToken.AccessToken);

            webRequest.SendWebRequest();

            while (!webRequest.isDone)
            {
                overrideCancellationToken.ThrowIfCancellationRequested();

                await Task.Yield();
            }

            return GetResponse<T>(webRequest);
        }

        public static async Task<string> GetRaw(string endpoint, KeyValuePair<string, string>[] parameters, SessionContext sessionContext,
                                          CancellationToken overrideCancellationToken = default)
        {
            if (sessionContext == null)
                throw new APIOperationFailed("No Session provided when trying to access: " + endpoint);

            endpoint = AddQuery(endpoint, parameters);
            using UnityWebRequest webRequest = UnityWebRequest.Get(endpoint);

            if (sessionContext.UserToken != null)
                webRequest.SetRequestHeader("Authorization", sessionContext.UserToken.TokenType + " " + sessionContext.UserToken.AccessToken);

            webRequest.SendWebRequest();

            while (!webRequest.isDone)
            {
                overrideCancellationToken.ThrowIfCancellationRequested();

                await Task.Yield();
            }

            if(webRequest.result == UnityWebRequest.Result.Success)
                return webRequest.downloadHandler.text;
            else
            {
                throw new APIOperationFailed(webRequest.error);
            }
        }

        public static async Task<WebResponse<T>> PostJson<T>(string endpoint, string jsonQuery, SessionContext sessionContext, CancellationToken overrideCancellationToken = default)
        {
            if (sessionContext == null)
                throw new APIOperationFailed("No Session provided when trying to access: " + endpoint);

            using UnityWebRequest webRequest = new UnityWebRequest(endpoint, "POST");
            
            //Add body
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonQuery);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
  
            if(sessionContext.UserToken != null)
                webRequest.SetRequestHeader("Authorization", sessionContext.UserToken.TokenType + " " + sessionContext.UserToken.AccessToken);

            webRequest.SetRequestHeader("Origin", "https://sdk.unionvatars.com/");

            webRequest.SendWebRequest();

            while (!webRequest.isDone)
            {
                overrideCancellationToken.ThrowIfCancellationRequested();

                await Task.Yield();
            }

            return GetResponse<T>(webRequest);
        }

        public static async Task<WebResponse<T>> PostForm<T>(string endpoint, KeyValuePair<string, string>[] parameters, SessionContext sessionContext, CancellationToken overrideCancellationToken = default)
        {
            if (sessionContext == null)
                throw new APIOperationFailed("No Session provided when trying to access: " + endpoint);

            //Add query
            UnityEngine.WWWForm queryForm = new UnityEngine.WWWForm();
            
            if (parameters != null)
			{
                foreach (KeyValuePair<string, string> param in parameters)
                {
                    queryForm.AddField(param.Key, param.Value); 
                }
			}

            using UnityWebRequest webRequest = UnityWebRequest.Post(endpoint, queryForm);

            if(sessionContext.UserToken != null)
                webRequest.SetRequestHeader("Authorization", sessionContext.UserToken.TokenType + " " + sessionContext.UserToken.AccessToken);

            webRequest.SendWebRequest();

            while (!webRequest.isDone)
            {
                overrideCancellationToken.ThrowIfCancellationRequested();

                await Task.Yield();
            }

            return GetResponse<T>(webRequest);
        }

        public static async Task<WebResponse> Delete(string endpoint, KeyValuePair<string, string>[] parameters, SessionContext sessionContext,
                                          CancellationToken overrideCancellationToken = default)
        {
            if (sessionContext == null)
                throw new APIOperationFailed("No Session provided when trying to access: " + endpoint);

            endpoint = AddQuery(endpoint, parameters);
            using UnityWebRequest webRequest = UnityWebRequest.Delete(endpoint);

            if(sessionContext.UserToken != null)
                webRequest.SetRequestHeader("Authorization", sessionContext.UserToken.TokenType + " " + sessionContext.UserToken.AccessToken);

            webRequest.SendWebRequest();

            while (!webRequest.isDone)
            {
                overrideCancellationToken.ThrowIfCancellationRequested();

                await Task.Yield();
            }

            return GetResponse(webRequest);
        }

        private static WebResponse<T> GetResponse<T>(UnityWebRequest webRequest)
        {
            WebResponse<T> response = new WebResponse<T>
            {
                success = (webRequest.result == UnityWebRequest.Result.Success)
            };

            if (webRequest.result == UnityWebRequest.Result.ConnectionError
               || webRequest.result == UnityWebRequest.Result.ProtocolError
               || webRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                response.responseErrorMessage = JsonConvert.DeserializeObject<ErrorResponse>(webRequest.downloadHandler.text).detail;
            }
            try
            {
                response.data = JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text);
                return response;
            }
            catch
            {
                return response;
            }
        }

        private static WebResponse GetResponse(UnityWebRequest webRequest)
        {
            WebResponse response = new WebResponse
            {
                success = (webRequest.result == UnityWebRequest.Result.Success)
            };
            
            if (webRequest.result == UnityWebRequest.Result.ConnectionError
               || webRequest.result == UnityWebRequest.Result.ProtocolError
               || webRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                response.responseErrorMessage = JsonConvert.DeserializeObject<ErrorResponse>(webRequest.downloadHandler.text).detail;
            }

            return response;
        }

        public static string AddQuery(string url, KeyValuePair<string, string>[] parameters)
        {
            if (parameters != null)
			{
                url += "?";

                for (int i = 0; i < parameters.Length; i++)
				{
                    KeyValuePair<string, string> param = parameters[i];
                    
                    if (i > 0)
						url += "&";

					url += param.Key + "=" + UnityWebRequest.EscapeURL(param.Value);
				}
			}

            return url;
        }
    }
}