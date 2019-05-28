using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Components;
using Assets.Scripts.Chirp;
using Assets.Scripts.Tools;
using System.Collections;

public enum CommandID : byte
{
    MotorSettings = 2,
    MotorSpeed = 3,
    MotorStop = 4
}

public class Marvin42_Controller : MonoBehaviour
{
    [SerializeField, ReadOnlyProperty]
    private Vector2 m_StartPosition = Vector2.zero;
    [SerializeField, ReadOnlyProperty]
    private Vector2 m_EndPosition = Vector2.zero;
    [SerializeField, ReadOnlyProperty]
    private bool m_TouchFieldActive = false;

#pragma warning disable 0649
    [SerializeField]
    private Image m_TouchField;
    [SerializeField]
    private Image m_TouchFieldBorder;
    [SerializeField]
    private Button m_CancelButton;
    [SerializeField]
    private Button m_StopButton;
    [SerializeField]
    private Image m_ProgressBar;
    [SerializeField]
    private Text m_SpeedLeftText;
    [SerializeField]
    private Text m_SpeedRightText;
    [SerializeField]
    private Text m_ChirpStateText;
#pragma warning restore 0649

    [SerializeField]
    private float m_ProgressBarRevolutionTime = 1f;

    [SerializeField]
    private string m_ChirpAppKey = string.Empty;
    [SerializeField]
    private string m_ChirpAppSecret = string.Empty;
    [SerializeField]
    private string m_ChirpAppConfig = string.Empty;

    private Vector2 m_TouchFieldTopPosition;
    private Coroutine m_ProgressBarRoutine = null;
    private float m_ProgressBarTargetValue = 1f;
    private ChirpStateEnum m_ChirpState = ChirpStateEnum.NotInitialised;

    private bool TouchFieldActive
    {
        get { return m_TouchFieldActive; }
        set
        {
            m_TouchFieldActive = value;
            m_TouchFieldBorder.color = ColorTools.FromHex(m_TouchFieldActive ? "FFD559" : "BF9F42");
        }
    }

    private void ChirpStateChanged(ChirpStateEnum oldState, ChirpStateEnum newState)
    {
        m_ChirpState = newState;
        m_ChirpStateText.text = m_ChirpState.ToString();
        if(newState == ChirpStateEnum.Sending)
        {
            StopProgressBarCoroutine();
            StartProgressBarCoroutine();
        }
    }

    private float PointerMovementDistanceNormalized
    {
        get
        {
            return (Vector2.Distance(m_StartPosition, m_EndPosition) * 2) / (m_TouchField.GetComponent<RectTransform>().rect.height / 2);
        }
    }

    private Vector2 PointerDeviation
    {
        get
        {
            return new Vector2
            (
                -Vector2Tools.CrossNormalized(m_TouchFieldTopPosition, m_EndPosition).y,
                Mathf.Clamp(Vector2Tools.DotNormalized(m_TouchFieldTopPosition, m_EndPosition), -1f, 1f)
            );
        }
    }

    public Vector2Int GetMotorSpeedValues()
    {
        float power = Mathf.Clamp01(PointerMovementDistanceNormalized);
        Vector2 motorValues = new Vector2(power, power);

        Vector2 deviation = PointerDeviation;
        if(deviation.x < 0f) motorValues.x *= deviation.y;
        else if(deviation.x > 0f) motorValues.y *= deviation.y;

        if(deviation.y < 0f)
        {
            motorValues.x = MathTools.Neg(motorValues.x);
            motorValues.y = MathTools.Neg(motorValues.y);
        }

        motorValues *= 100f;
        return new Vector2Int((int)motorValues.x, (int)motorValues.y);
    }

    public void SendMotorSpeed(sbyte speedLeft, sbyte speedRight)
    {
        if(speedLeft == 0 && speedRight == 0)
        {
            SendMotorStop();
            return;
        }

        Debug.Log($"Sending MotorSpeed({ speedLeft }, { speedRight })");
        ChirpManager.SendData(new byte[] { (byte)CommandID.MotorSpeed, (byte)speedLeft, (byte)speedRight });
    }

    public void SendMotorStop()
    {
        Debug.Log("Sending MotorStop");
        m_SpeedRightText.text = m_SpeedLeftText.text = "0";
        ChirpManager.SendData(new byte[] { (byte)CommandID.MotorStop });
    }

    private void DisplayMotorSpeed(sbyte speedLeft, sbyte speedRight)
    {
        if(!TouchFieldActive) return;

        m_SpeedLeftText.text = speedLeft.ToString();
        m_SpeedRightText.text = speedRight.ToString();
    }

    private IEnumerator UpdateProgressBar()
    {
        while(m_ChirpState == ChirpStateEnum.Sending)
        {
            m_ProgressBar.fillAmount = Mathf.Clamp01(m_ProgressBar.fillAmount + ((UnityEngine.Time.deltaTime / m_ProgressBarRevolutionTime) * (m_ProgressBarTargetValue < 0.5f ? -1f : 1f)));
            if(m_ProgressBar.fillAmount >= 1f)
            {
                m_ProgressBarTargetValue = 0f;
                m_ProgressBar.fillClockwise = false;
            }
            else if(m_ProgressBar.fillAmount <= 0f)
            {
                m_ProgressBarTargetValue = 1f;
                m_ProgressBar.fillClockwise = true;
            }

            yield return null;
        }

        m_ProgressBar.fillAmount = 0f;
        m_ProgressBarTargetValue = 1f;
        m_ProgressBar.fillClockwise = true;
        m_ProgressBarRoutine = null;
    }

    private void StartProgressBarCoroutine()
    {
        if(m_ProgressBarRoutine != null) return;

        m_ProgressBarRoutine = StartCoroutine(UpdateProgressBar());
    }

    private void StopProgressBarCoroutine()
    {
        if(m_ProgressBarRoutine == null) return;

        StopCoroutine(m_ProgressBarRoutine);
        m_ProgressBarRoutine = null;
    }

    private void Awake()
    {
        ChirpManager.Key = m_ChirpAppKey;
        ChirpManager.Secret = m_ChirpAppSecret;
        ChirpManager.Config = m_ChirpAppConfig;
    }

    private void Start()
    {
        ChirpManager.InitSDK();
        ChirpManager.StartSDK();

        ChirpManager.OnStateChangedEvent = ChirpStateChanged;

        m_CancelButton.onClick.AddListener(() => TouchFieldActive = false);
        m_StopButton.onClick.AddListener(SendMotorStop);

        m_TouchFieldTopPosition = new Vector2(m_TouchField.transform.position.x, m_TouchField.transform.position.y + m_TouchField.GetComponent<RectTransform>().rect.height) - (Vector2)m_TouchField.transform.position;

        m_ProgressBar.fillAmount = 0f;
        m_ProgressBar.fillClockwise = true;

        TouchFieldActive = false;
    }

    private void OnDestroy()
    {
        ChirpManager.ClearCallbacks();
        ChirpManager.StopSDK();
    }

    private void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began && !TouchFieldActive)
            {
                TouchFieldActive = m_TouchField.GetComponent<PointerClickHandler>().PointerDown;
                if(TouchFieldActive)
                {
                    m_StartPosition = touch.position - (Vector2)m_TouchField.transform.position;

                    m_SpeedLeftText.text = "0";
                    m_SpeedRightText.text = "0";
                }
            }
            else if(touch.phase == TouchPhase.Ended && TouchFieldActive)
            {
                m_EndPosition = touch.position - (Vector2)m_TouchField.transform.position;

                Vector2Int motorValues = GetMotorSpeedValues();
                sbyte speedLeft = MathTools.Clamp((sbyte)motorValues.x, -100, 100);
                sbyte speedRight = MathTools.Clamp((sbyte)motorValues.y, -100, 100);

                DisplayMotorSpeed(speedLeft, speedRight);
                SendMotorSpeed(speedLeft, speedRight);

                TouchFieldActive = false;
            }
            else if(touch.phase == TouchPhase.Moved && TouchFieldActive)
            {
                m_EndPosition = touch.position - (Vector2)m_TouchField.transform.position;

                Vector2Int motorValues = GetMotorSpeedValues();
                DisplayMotorSpeed(MathTools.Clamp((sbyte)motorValues.x, -100, 100), MathTools.Clamp((sbyte)motorValues.y, -100, 100));
            }
        }
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        GUI.Box
        (
            new Rect(20f, 20f, ((float)Screen.width / 2), ((float)Screen.height / 2) * 0.25f),
            ""
        );

        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = (int)(Screen.width / 25f);
        labelStyle.normal.textColor = Color.red;
        GUI.Label
        (
            new Rect(20f, 20f, ((float)Screen.width / 2), ((float)Screen.height / 2) * 0.25f),
            $"Position\t= { m_EndPosition.ToString("0.00") }\n" +
            $"Distance\t= { PointerMovementDistanceNormalized.ToString("0.00") }\n" +
            $"Deviation\t= { PointerDeviation.ToString("0.00") }",
            labelStyle
        );
    }
#endif
}
