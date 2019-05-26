using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class Chirp : MonoBehaviour
{
    [System.Serializable]
    public class ConfigEntity
    {
        public string name;
        public string value;
    }

    public string Key;
    public string Secret;
    private string Config;
    public ConfigEntity[] Configs;

    public Dropdown ProtocolSelect;
    public Button InitButton;
    public Button StartButton;
    public Button StopButton;
    public Button SendButton;
    public Text StatusText;
    public Text MessageText;
    public Text ErrorText;
    public InputField MessageInput;

    // Use this for initialization
    void Start() {
        HideErrorMessage();
        ProtocolSelect.ClearOptions();
        ProtocolSelect.AddOptions(GetProtocolOptions());
        Config = Configs[0].value;
    }

    private List<Dropdown.OptionData> GetProtocolOptions() {
        List<Dropdown.OptionData> result = new List<Dropdown.OptionData>();
        for (int i = 0; i < Configs.Length; i++) {
            result.Add(new Dropdown.OptionData(Configs[i].name));
        }
        return result;
    }

    public void OnChangeProtocol(Dropdown change) {
        HideErrorMessage();
        Config = Configs[change.value].value;
        ChirpStopSDK();
        InitButton.interactable = true;
        StartButton.interactable = false;
    }

    public void ChirpInitSDK() {
        HideErrorMessage();
        ChirpPlugin.OnSentDataEvent -= ChirpPlugin_OnSentDataEvent;
        ChirpPlugin.OnRecieveDataEvent -= ChirpPlugin_OnRecieveDataEvent;
        ChirpPlugin.OnChangeStateDataEvent -= ChirpPlugin_OnChangeStateDataEvent;

        try {
            ChirpPlugin.InitSDK(Key, Secret, Config);
        } catch(System.Exception ex) {
            ErrorText.text = ex.Message;
            return;
        }

        ChirpPlugin.OnSentDataEvent += ChirpPlugin_OnSentDataEvent;
        ChirpPlugin.OnRecieveDataEvent += ChirpPlugin_OnRecieveDataEvent;
        ChirpPlugin.OnChangeStateDataEvent += ChirpPlugin_OnChangeStateDataEvent;
        InitButton.interactable = false;
        StartButton.interactable = true;
    }

    void ChirpPlugin_OnChangeStateDataEvent(
        ChirpStateEnum oldState, ChirpStateEnum newState) {

        Debug.Log("State change: " + oldState.ToString() + " => " + newState.ToString());
        StatusText.text = "Status: " + newState.ToString();
        SendButton.interactable = newState == ChirpStateEnum.Running;
    }


    void ChirpPlugin_OnRecieveDataEvent(string data) {
        MessageText.text = ">" + data;
        Debug.Log("Recv: " + data + " | length: " + data.Length);
    }

    void ChirpPlugin_OnSentDataEvent(string data) {
        MessageText.text = ">" + data;
        Debug.Log("Send: " + data);
    }


    public void ChirpStartSDK() {
        HideErrorMessage();
        try {
            ChirpPlugin.StartSDK();
        } catch(System.Exception ex) {
            ErrorText.text = ex.Message;
            return;
        }

        StartButton.interactable = false;
        StopButton.interactable = true;
        SendButton.interactable = true;
        MessageInput.interactable = true;
    }

    public void ChirpStopSDK() {
        HideErrorMessage();
        try {
            ChirpPlugin.StopSDK();
        } catch(System.Exception ex) {
            ErrorText.text = ex.Message;
            return;
        }

        StartButton.interactable = true;
        StopButton.interactable = false;
        SendButton.interactable = false;
        MessageInput.interactable = false;
    }

    public void ChirpSendData() {
        HideErrorMessage();
        string payload = "Test message";
        payload = MessageInput.text.Length > 0 ? MessageInput.text : payload;
        SendButton.interactable = false;
        try {
            ChirpPlugin.SendData(payload);
        } catch(System.Exception ex) {
            ErrorText.text = ex.Message;
            SendButton.interactable = true;
        }
    }

    private void HideErrorMessage() {
        ErrorText.text = "";
    }
}
