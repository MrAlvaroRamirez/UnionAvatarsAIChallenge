using System;

namespace UnionAvatars.API
{
    public class APIOperationFailed : Exception
    {
        public APIOperationFailed (string message) : base("API Operation failed: " + message)
        {
        }
        public APIOperationFailed (ErrorResponse response) : base("API Operation failed: " + response.detail)
        {
        }
    }
}
