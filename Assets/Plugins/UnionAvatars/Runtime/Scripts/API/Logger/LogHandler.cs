using System;
using UnityEngine;

namespace UnionAvatars.Log
{
    public class LogHandler
    {
        private bool logToUnity = true;
        public event Action<string, string, AvatarSDKLogType> onLog; 

        public LogHandler(bool shouldLogToUnity)
        {
            logToUnity = shouldLogToUnity;
        }

        public void APIWarning(string message)
        {
            onLog?.Invoke("API Error", message, AvatarSDKLogType.Error);
            if(logToUnity) Debug.LogWarning(message);
        }
        public void LoginWarning()
        {
            onLog?.Invoke("Login Error", "You are not logged in", AvatarSDKLogType.Warning);
            if(logToUnity) Debug.LogWarning("Couldn't perform operations because you are not logged in");
        }
        public void AvatarWarning(string message)
        {
            onLog?.Invoke("Avatar Error", message, AvatarSDKLogType.Error);
            if(logToUnity) Debug.LogWarning("Avatar Conversion Pipeline Error: " + message);
        }
        public void CustomLog(string title, string message, AvatarSDKLogType type = AvatarSDKLogType.Error)
        {
            onLog?.Invoke(title, message, type);
            if(logToUnity) Debug.LogWarning($"{title}: {message}");
        }
    }

    public enum AvatarSDKLogType
    {
        Info,
        Warning,
        Error
    }
}

