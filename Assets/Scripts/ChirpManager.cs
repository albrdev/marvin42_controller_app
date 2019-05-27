using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using Assets.Scripts.Components;

public class ChirpManager : MonoBehaviourSingleton
{
    [System.Serializable]
    public class ConfigEntity
    {
        public string name;
        public string value;
    }

    [SerializeField]
    private string m_Key = string.Empty;
    [SerializeField]
    private string m_Secret = string.Empty;
    [SerializeField, ReadOnlyProperty]
    private string m_Config = string.Empty;

    [SerializeField]
    private ConfigEntity[] m_Configs;

    private bool m_IsInitialized = false;

    public static new ChirpManager Instance
    {
        get { return GetInstance<ChirpManager>(); }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnDestroy()
    {
        StopSDK();
        base.OnDestroy();
    }

    private void Start()
    {
    }

    private void ChirpPlugin_OnChangeStateDataEvent(ChirpStateEnum oldState, ChirpStateEnum newState)
    {
        Debug.Log("State change: " + oldState.ToString() + " => " + newState.ToString());
    }


    private void ChirpPlugin_OnRecieveDataEvent(string data)
    {
        Debug.Log("Recv: " + data + " | length: " + data.Length);
    }

    private void ChirpPlugin_OnSentDataEvent(string data)
    {
        Debug.Log("Send: " + data);
    }

    public void InitSDK()
    {
        if(m_IsInitialized)
        {
            StopSDK();
        }

        ChirpPlugin.OnSentDataEvent -= ChirpPlugin_OnSentDataEvent;
        ChirpPlugin.OnRecieveDataEvent -= ChirpPlugin_OnRecieveDataEvent;
        ChirpPlugin.OnChangeStateDataEvent -= ChirpPlugin_OnChangeStateDataEvent;

        ChirpPlugin.InitSDK(m_Key, m_Secret, m_Config);

        ChirpPlugin.OnSentDataEvent += ChirpPlugin_OnSentDataEvent;
        ChirpPlugin.OnRecieveDataEvent += ChirpPlugin_OnRecieveDataEvent;
        ChirpPlugin.OnChangeStateDataEvent += ChirpPlugin_OnChangeStateDataEvent;

        m_IsInitialized = true;
    }

    public void StartSDK()
    {
        if(m_IsInitialized) StopSDK();
        if(!m_IsInitialized) InitSDK();

        ChirpPlugin.StartSDK();
    }

    public void StopSDK()
    {
        if(!m_IsInitialized) return;

        ChirpPlugin.StopSDK();
        m_IsInitialized = false;
    }

    //public void SendData(byte[] payload)
    //{
    //    ChirpPlugin.SendData(payload);
    //}

    public void SendData(string payload)
    {
        ChirpPlugin.SendData(payload);
    }
}
