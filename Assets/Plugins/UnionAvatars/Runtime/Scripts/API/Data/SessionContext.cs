using System;

namespace UnionAvatars.API
{
    /// <summary>
    /// The session context contains data which should be a constant over the play session, or needs to be cached (such as the bodies array)
    /// </summary>
    public class SessionContext
    {
        private Uri url;
        public bool endApi;
        public UserToken UserToken = null;
        public Uri Url
        {
            get => url;
            set => url = value;
        }

        public SessionContext(string urlString)
        {
            Url = new Uri(urlString);
            endApi = Url.Host.Split('.')[0] == "endapi";
        }
    }
}