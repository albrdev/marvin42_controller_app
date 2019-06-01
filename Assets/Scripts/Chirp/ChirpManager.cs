/// <copyright file="ChirpManager.cs" company="Region Östergötland">
/// Released under Apache License v2.0
/// </copyright>
/// <author email="albrdev@gmail.com">Alexander Brunström</author>
/// <date>2019-05-28</date>

namespace Assets.Scripts.Chirp
{
    /// <summary>
    /// Wrapper for ChirpPlugin (part of Chirp Unity Plugin) class
    /// Uses Chirp Unity Plugin by Alexander Gutrits, https://github.com/AlexanderKub/chirp-unity-plugin (Released under MIT license)
    /// </summary>
    public static class ChirpManager
    {
        public static string Key { get; set; } = null;
        public static string Secret { get; set; } = null;
        public static string Config { get; set; } = null;

        private static bool m_IsInitialized = false;
        private static bool m_IsStarted = false;

        public static System.Action<ChirpStateEnum, ChirpStateEnum> OnStateChangedEvent
        {
            get { return ChirpPlugin.OnChangeStateDataEvent; }
            set { ChirpPlugin.OnChangeStateDataEvent = value; }
        }

        public static System.Action<string> OnDataRecievedEvent
        {
            get { return ChirpPlugin.OnRecieveDataEvent; }
            set { ChirpPlugin.OnRecieveDataEvent = value; }
        }

        public static System.Action<string> OnDataSentEvent
        {
            get { return ChirpPlugin.OnSentDataEvent; }
            set { ChirpPlugin.OnSentDataEvent = value; }
        }

        public static void ClearCallbacks()
        {
            OnStateChangedEvent = null;
            OnDataRecievedEvent = null;
            OnDataSentEvent = null;
        }

        public static void InitSDK()
        {
            if(m_IsInitialized || (string.IsNullOrWhiteSpace(Key) || string.IsNullOrWhiteSpace(Secret) || string.IsNullOrWhiteSpace(Config))) return;

            ChirpPlugin.InitSDK(Key, Secret, Config);
            m_IsInitialized = true;
        }

        public static void StartSDK()
        {
            if(m_IsStarted || !m_IsInitialized) return;

            ChirpPlugin.StartSDK();
            m_IsStarted = true;
        }

        public static void StopSDK()
        {
            if(!m_IsInitialized) return;

            ChirpPlugin.StopSDK();
            m_IsInitialized = m_IsStarted = false;
        }

        public static void RestartSDK()
        {
            StopSDK();
            InitSDK();
            StartSDK();
        }

        public static void SendData(byte[] payload)
        {
            ChirpPlugin.SendData(payload);
        }

        public static void SendData(string payload)
        {
            ChirpPlugin.SendData(payload);
        }
    }
}
