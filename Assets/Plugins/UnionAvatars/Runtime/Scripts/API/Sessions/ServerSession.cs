using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnionAvatars.Log;

namespace UnionAvatars.API
{
    public class ServerSession : ISession
    {
        //Setup the session
        //This class will contain important information that should be kept alive, such as the session token
        private SessionContext _sessionContext;
        public SessionContext UnionAvatarsSession
        {
            get => _sessionContext;
        }

        //Setup the logger
        private LogHandler _logHandler;
        public LogHandler LogHandler
        {
            get => _logHandler;
        }

        private CancellationToken cancellationToken;

        /// <summary>
        /// Creates a new session object
        /// </summary>
        /// <param name="url">
        /// URL of the API
        /// </param>
        /// <param name="ct">
        /// Cancellation Token to cancel the ongoing operations on this session
        /// </param>
        /// <param name="logToUnity">
        /// If true, warnings and errors will be logged to Unity
        /// </param>
        public ServerSession(string url, bool logToUnity, CancellationToken ct = default)
        {
            _sessionContext = new SessionContext(url);
            _logHandler = new LogHandler(logToUnity);
            cancellationToken = ct;
        }

        #region User

        /// <summary>
        /// Logs into the union avatars portal
        /// </summary>
        /// <returns>
        /// Bool: Successful login
        /// </returns>
        public async Task<bool> Login(string username, string password)
        {
            if(username == "" || password == "")
            {
                LogHandler.CustomLog("Invalid Credentials", "User and password cannot be empty", Log.AvatarSDKLogType.Error);
                return false;
            }

            KeyValuePair<string, string>[] parameters =
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
            };
            WebResponse<UserToken> loginResponse = await WebRequests.PostForm<UserToken>(UnionAvatarsSession.Url + "login",
                                                                                         parameters,
                                                                                         UnionAvatarsSession,
                                                                                         cancellationToken);

            if(loginResponse.success)
            {
                UnionAvatarsSession.UserToken = loginResponse.data;
                
                //Check if user is activated
                WebResponse<bool> userResponse = await WebRequests.Get<bool>(UnionAvatarsSession.Url + "users/",
                                                                             default,
                                                                             UnionAvatarsSession,
                                                                             cancellationToken);

                if(userResponse.success)
                {
                    return true;
                }
                else
                {
                    LogHandler.APIWarning("User is not active, check your email to verify it");
                    return false;
                }
            }
            else
            {
                LogHandler.APIWarning(loginResponse.responseErrorMessage);
                return false;
            }
        }

        /// <summary>
        /// Logs into the union avatars portal
        /// </summary>
        /// <returns>
        /// Bool: Successful login
        /// </returns>
        public async Task<bool> Register(string username, string email, string password)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"username", username},
                {"email", email},
                {"password", password},
            };

            WebResponse<bool> registerResponse = await WebRequests.PostJson<bool>(UnionAvatarsSession.Url + "users/",
                                                                                  JsonConvert.SerializeObject(parameters),
                                                                                  UnionAvatarsSession,
                                                                                  cancellationToken);

            if(registerResponse.success)
            {
                return true;
            }
            else
            {
                LogHandler.APIWarning(registerResponse.responseErrorMessage);
                return false;
            }
        }


        #endregion

        #region Bodies

        /// <summary>
        /// Retrieve all the bodies metadata
        /// </summary>
        /// <param name="limit">
        /// Number of bodies included in the response
        /// </param>
        /// <param name="skip">
        /// Starting index of body list
        /// </param>
        public async Task<Body[]> GetBodies(int limit = 10, int skip = 0)
        {
            if(UnionAvatarsSession.UserToken == null)
            {
                LogHandler.LoginWarning();
                return null;
            }
            
            KeyValuePair<string, string>[] parameters =
            {
                new KeyValuePair<string, string>("limit", limit.ToString()),
                new KeyValuePair<string, string>("skip", skip.ToString())
            };
            WebResponse<Body[]> bodiesResponse = await WebRequests.Get<Body[]>(UnionAvatarsSession.Url + (UnionAvatarsSession.endApi ? "portal/" : "") + "bodies",
                                                                                    parameters,
                                                                                    UnionAvatarsSession,
                                                                                    cancellationToken);

            if(bodiesResponse.success)
            {
                return bodiesResponse.data;
            }
            else
            {
                LogHandler.APIWarning(bodiesResponse.responseErrorMessage);
                return null;
            }
        }

        #endregion

        #region Avatars
   
        /// <summary>
        /// Creates a new avatar in the logged account
        /// </summary>
        public async Task<AvatarMetadata> CreateAvatar(AvatarRequest avatarRequest)
        {
            if(UnionAvatarsSession.UserToken == null)
            {
                LogHandler.LoginWarning();
                return null;
            }

            WebResponse<AvatarMetadata> avatarResponse = await WebRequests.PostJson<AvatarMetadata>(UnionAvatarsSession.Url + (UnionAvatarsSession.endApi ? "portal/" : "") + "avatars",
                                                                                                    JsonConvert.SerializeObject(avatarRequest),
                                                                                                    UnionAvatarsSession,
                                                                                                    cancellationToken);
            
            if(avatarResponse.success)
            {
                return avatarResponse.data;
            }
            else
            {
                LogHandler.APIWarning(avatarResponse.responseErrorMessage);
                return null;
            }
        }
    
        /// <summary>
        /// Retrieve all the user avatars
        /// </summary>
        /// <param name="limit">
        /// Number of avatars included in the response
        /// </param>
        /// <param name="skip">
        /// Starting index of avatar list
        /// </param>
        public async Task<AvatarMetadata[]> GetAvatars(int limit = 10, int skip = 0)
        {
            if(UnionAvatarsSession.UserToken == null)
            {
                LogHandler.LoginWarning();
                return null;
            }
            
            KeyValuePair<string, string>[] parameters =
            {
                new KeyValuePair<string, string>("limit", limit.ToString()),
                new KeyValuePair<string, string>("skip", skip.ToString())
            };

            WebResponse<AvatarMetadata[]> avatarsResponse = await WebRequests.Get<AvatarMetadata[]>(UnionAvatarsSession.Url + (UnionAvatarsSession.endApi ? "portal/" : "") + "avatars",
                                                                                                    parameters,
                                                                                                    UnionAvatarsSession,
                                                                                                    cancellationToken);

            if(avatarsResponse.success)
            {
                return avatarsResponse.data;
            }
            else
            {
                LogHandler.APIWarning(avatarsResponse.responseErrorMessage);
                return null;
            }
        }
        
        /// <summary>
        /// Retrieves an avatar by it's id
        /// </summary>
        public async Task<AvatarMetadata> GetAvatar(string avatarId)
        {
            if(UnionAvatarsSession.UserToken == null)
            {
                LogHandler.LoginWarning();
                return null;
            }

            string endpoint = UnionAvatarsSession.Url + (UnionAvatarsSession.endApi ? "portal/" : "") + "avatars/" + avatarId;

            KeyValuePair<string, string>[] parameters = {};
            WebResponse<AvatarMetadata> avatarResponse = await WebRequests.Get<AvatarMetadata>(endpoint,
                                                                                               parameters,
                                                                                               UnionAvatarsSession,
                                                                                               cancellationToken);
            
            if(avatarResponse.success)
            {
                return avatarResponse.data;
            }
            else
            {
                LogHandler.APIWarning(avatarResponse.responseErrorMessage);
                return null;
            }
        }

        /// <summary>
        /// Delete an avatar from the servers
        /// </summary>
        /// /// <param name="avatarRequest">
        /// The avatar to be deleted
        /// </param>
        public async Task DeleteAvatar(AvatarMetadata avatarRequest)
        {
            if(UnionAvatarsSession.UserToken == null)
            {
                LogHandler.LoginWarning();
                return;
            }

            KeyValuePair<string, string>[] parameters = {};

            WebResponse deleteResponse = await WebRequests.Delete(UnionAvatarsSession.Url + (UnionAvatarsSession.endApi ? "portal/" : "") + "avatars/" + avatarRequest.Id,
                                                                  parameters,
                                                                  UnionAvatarsSession,
                                                                  cancellationToken);
            
            if(!deleteResponse.success)
                LogHandler.APIWarning(deleteResponse.responseErrorMessage);
        }

        #endregion

        #region Heads

        /// <summary>
        /// Creates a new head in the logged account
        /// </summary>
        public async Task<Head> CreateHead(HeadRequest headRequest)
        {
            if(UnionAvatarsSession.UserToken == null)
            {
                LogHandler.LoginWarning();
                return null;
            }

            WebResponse<Head> headResponse = await WebRequests.PostJson<Head>(UnionAvatarsSession.Url + (UnionAvatarsSession.endApi ? "portal/" : "") + "heads",
                                                                                                    JsonConvert.SerializeObject(headRequest),
                                                                                                    UnionAvatarsSession,
                                                                                                    cancellationToken);
            
            if(headResponse.success)
            {
                return headResponse.data;
            }
            else
            {
                LogHandler.APIWarning(headResponse.responseErrorMessage);
                return null;
            }
        }

        /// <summary>
        /// Retrieve all the bodies metadata
        /// </summary>
        /// <param name="limit">
        /// Number of heads included in the response
        /// </param>
        /// <param name="skip">
        /// Starting index of head list
        /// </param>
        public async Task<Head[]> GetHeads(int limit = 5, int skip = 0)
        {
            if(UnionAvatarsSession.UserToken == null)
            {
                LogHandler.LoginWarning();
                return null;
            }
            
            KeyValuePair<string, string>[] parameters =
            {
                new KeyValuePair<string, string>("limit", limit.ToString()),
                new KeyValuePair<string, string>("skip", skip.ToString())
            };
            WebResponse<Head[]> bodiesResponse = await WebRequests.Get<Head[]>(UnionAvatarsSession.Url + (UnionAvatarsSession.endApi ? "portal/" : "") + "heads",
                                                                                    parameters,
                                                                                    UnionAvatarsSession,
                                                                                    cancellationToken);

            if(bodiesResponse.success)
            {
                return bodiesResponse.data;
            }
            else
            {
                LogHandler.APIWarning(bodiesResponse.responseErrorMessage);
                return null;
            }
        }
        
        /// <summary>
        /// Retrieves a head by it's id
        /// </summary>
        /// <param name="headId">
        /// Head Id, if null it will return the last avatar
        /// </param>
        /// <returns></returns>
        public async Task<Head> GetHead(string headId = null)
        {
            if(UnionAvatarsSession.UserToken == null)
            {
                LogHandler.LoginWarning();
                return null;
            }

            string endpoint = UnionAvatarsSession.Url + (UnionAvatarsSession.endApi ? "portal/" : "") + "heads/" + headId;

            KeyValuePair<string, string>[] parameters = {};
            WebResponse<Head[]> headResponse = await WebRequests.Get<Head[]>(endpoint,
                                                                             parameters,
                                                                             UnionAvatarsSession,
                                                                             cancellationToken);
            
            if(headResponse.success)
            {
                return headResponse.data[0];
            }
            else
            {
                LogHandler.APIWarning(headResponse.responseErrorMessage);
                return null;
            }
        }

        /// <summary>
        /// Delete a head from the servers
        /// </summary>
        /// /// <param name="headRequest">
        /// The head to be deleted
        /// </param>
        public async Task DeleteHead(Head headRequest)
        {
            if(UnionAvatarsSession.UserToken == null)
            {
                LogHandler.LoginWarning();
                return;
            }

            KeyValuePair<string, string>[] parameters = {};

            WebResponse deleteResponse = await WebRequests.Delete(UnionAvatarsSession.Url + (UnionAvatarsSession.endApi ? "portal/" : "") + "heads/" + headRequest.Id,
                                                                  parameters,
                                                                  UnionAvatarsSession,
                                                                  cancellationToken);
            if(!deleteResponse.success)
                LogHandler.APIWarning(deleteResponse.responseErrorMessage);
        }

        #endregion
    }
}
