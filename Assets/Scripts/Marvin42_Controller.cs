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

/// <summary>
/// marvin42 Controller main component
/// </summary>
public class Marvin42_Controller : MonoBehaviour
{
    [SerializeField, ReadOnlyProperty, Tooltip("Touch start position")]
    private Vector2 m_TouchStartPosition = Vector2.zero;
    [SerializeField, ReadOnlyProperty, Tooltip("Touch end position")]
    private Vector2 m_TouchEndPosition = Vector2.zero;
    [SerializeField, ReadOnlyProperty, Tooltip("Touch field active")]
    private bool m_TouchFieldActive = false;

#pragma warning disable 0649 // Disable "Field is never assigned to" warning
    [SerializeField, Tooltip("Touch field component")]
    private Image m_TouchField;
    [SerializeField, Tooltip("Touch cancel button")]
    private Button m_CancelButton;
    [SerializeField, Tooltip("Send motor stop button")]
    private Button m_StopButton;
    [SerializeField, Tooltip("Toggle motor speed value clamping to each other")]
    private Toggle m_ClampMotorValuesToggle;
    [SerializeField, Tooltip("Toggle motor speed value clamping to interval")]
    private Toggle m_ClampToIntervalToggle;
    [SerializeField, Tooltip("Radial progress bar")]
    private Image m_ProgressBar;
    [SerializeField, Tooltip("Left motor speed value")]
    private Text m_SpeedLeftText;
    [SerializeField, Tooltip("Right motor speed value")]
    private Text m_SpeedRightText;
    [SerializeField, Tooltip("Current Chirp state")]
    private Text m_ChirpStateText;
#pragma warning restore 0649

    [SerializeField, Tooltip("If motor speed values is within this value's interval from each other, clamp them to the extremest of those values")]
    private float m_MotorSpeedClampDelta = 5f;

    [SerializeField, Tooltip("The time it takes for thre progress bar to revolve a full circle")]
    private float m_ProgressBarRevolutionTime = 1f;

    [SerializeField, Tooltip("Chirp app key")]
    private string m_ChirpAppKey = string.Empty;
    [SerializeField, Tooltip("Chirp app secret")]
    private string m_ChirpAppSecret = string.Empty;
    [SerializeField, Tooltip("Chirp app config")]
    private string m_ChirpAppConfig = string.Empty;

    private Vector2 m_TouchFieldTopCenterPosition; // Touch field top-center position
    private Coroutine m_ProgressBarRoutine = null;
    private ChirpStateEnum m_ChirpState = ChirpStateEnum.NotInitialised;

    /// <summary>
    /// Check if touch field is active nor not
    /// <returns>'true' if touch field is pressed and/or holding down, or 'false otherwise</returns>
    /// </summary>
    public bool TouchFieldActive
    {
        get { return m_TouchFieldActive; }
        private set
        {
            m_TouchFieldActive = value;
            if(value)
            {
                m_TouchField.GetComponent<UIAlphaFader>().Fade(1f, 0.5f); // Fade in when touch field becomes active
            }
            else
            {
                m_TouchField.GetComponent<UIAlphaFader>().Fade(0.5f, 0.25f); // Fade out when touch field becomes inactive
            }
        }
    }

    /// <summary>
    /// Is called when Chirp state changes
    /// Sets UI text label to current state
    /// Starts progress bar update coroutine
    /// </summary>
    private void ChirpStateChanged(ChirpStateEnum oldState, ChirpStateEnum newState)
    {
        m_ChirpState = newState;
        m_ChirpStateText.text = m_ChirpState.ToString();
        // When the new state becomes 'Sending', start the progress bar 'animation' update
        if(newState == ChirpStateEnum.Sending)
        {
            StopProgressBarCoroutine();
            StartProgressBarCoroutine();
        }
    }

    /// <summary>
    /// Calculates the normalized distance of touch-and-drag movement on the touch field
    /// <returns>The normalized(0-1, float) distance from position where touch field was activated to the position where it was released</returns>
    /// </summary>
    private float PointerMovementDistanceNormalized
    {
        get
        {
            // Distance moved (normalized) = distance moved / half of the touch field radius
            return (Vector2.Distance(m_TouchStartPosition, m_TouchEndPosition) * 2) / (m_TouchField.GetComponent<RectTransform>().rect.height / 2);
        }
    }

    /// <summary>
    /// Calculates the deviation (relation to the touch field centre in a coordinate system context) of the pointers current position
    /// Explanation of the returned vector:
    /// <list type="deviation">
    /// <item>
    /// <description>'x' is less than zero: pointer is on the left side of the touch field's centre</description>
    /// </item>
    /// <item>
    /// <description>'x' is greater than zero: pointer is on the right side of the touch field's centre</description>
    /// </item>
    /// <item>
    /// <description>'y' is less than zero: pointer is below the touch field's centre</description>
    /// </item>
    /// <item>
    /// <description>'y' is greater than zero: pointer is above the touch field's centre</description>
    /// </item>
    /// </list>
    /// <returns>A 2 dimensional vector containing 'x' for horizontal and 'y' for vertical pointer deviation(relation)</returns>
    /// </summary>
    private Vector2 PointerDeviation
    {
        get
        {
            return new Vector2
            (
                // x = y of normalized cross product of touch field's top-center position and where the touch input ended (values between -1 and 1)
                -Vector2Tools.CrossNormalized(m_TouchFieldTopCenterPosition, m_TouchEndPosition).y,
                // y = Normalized dot product of touch field's top-center position and where the touch input ended, clamped to a value between -1 and 1
                Mathf.Clamp(Vector2Tools.DotNormalized(m_TouchFieldTopCenterPosition, m_TouchEndPosition), -1f, 1f)
            );
        }
    }

    /// <summary>
    /// Calculates motor speed values using deviation as direction(speed balance between motor A and B) and touch field movement distance (motor power multiplier)
    /// <returns>A normalized(0-100, int) 2 dimensional vector containing the calculated speed on both motors ('x' for motor A and 'y' for motor B)</returns>
    /// </summary>
    public Vector2Int GetMotorSpeedValues()
    {
        float power = Mathf.Clamp01(PointerMovementDistanceNormalized); // Treat normalized distance (between touch start and touch end) as a motor power multiplier
        Vector2 motorValues = new Vector2(power, power);

        Vector2 deviation = PointerDeviation; // Get pointer relation between the touch field's center position
        if(deviation.x < 0f) motorValues.x *= deviation.y; // Pointer is on the left side
        else if(deviation.x > 0f) motorValues.y *= deviation.y; // Pointer is on the right side

        if(deviation.y < 0f) // Pointer is below the center
        {
            // Negate the motor power to make them run backwards
            motorValues.x = MathTools.Neg(motorValues.x);
            motorValues.y = MathTools.Neg(motorValues.y);
        }

        float mscd = m_MotorSpeedClampDelta / 100f; // 'Convert' from percentage to decimal form

        // Clamp motor values to an interval
        if(m_ClampToIntervalToggle.isOn)
        {
            motorValues.x = Mathf.Round(motorValues.x / mscd) * mscd;
            motorValues.y = Mathf.Round(motorValues.y / mscd) * mscd;
        }

        // Clamp motor values to each other, if they are close enough
        if(m_ClampMotorValuesToggle.isOn)
        {
            // Clamp them to zero, if near center
            if(MathTools.Approximately(motorValues.x, 0f, mscd) && MathTools.Approximately(motorValues.y, 0f, mscd))
            {
                motorValues.y = motorValues.x = 0f;
            }
            else if(MathTools.Approximately(motorValues.x, motorValues.y, mscd))
            {
                if(deviation.y > 0)
                {
                    // Clamp to maximum value if going forwards
                    motorValues.y = motorValues.x = Mathf.Max(motorValues.x, motorValues.y);
                }
                else if(deviation.y < 0f)
                {
                    // Clamp to minumum value if going backwards
                    motorValues.y = motorValues.x = Mathf.Min(motorValues.x, motorValues.y);
                }
            }
        }

        motorValues *= 100f; // 'Convert' decimal to percent form
        return new Vector2Int((int)Mathf.Clamp(motorValues.x, -100, 100), (int)Mathf.Clamp(motorValues.y, -100, 100)); // Clamp values to min/max and return them as int version of Vector2
    }

    /// <summary>
    /// Sends the specified motor speed values as Chirp audio
    /// </summary>
    public void SendMotorSpeed(sbyte speedLeft, sbyte speedRight)
    {
        // If power is zero, send motor stop instead
        if(speedLeft == 0 && speedRight == 0)
        {
            SendMotorStop();
            return;
        }

        Debug.Log($"Sending MotorSpeed({ speedLeft }, { speedRight })");
        ChirpManager.SendData(new byte[] { (byte)CommandID.MotorSpeed, (byte)speedLeft, (byte)speedRight }); // Make a byte array with type and the motor speeds, then send it as Chirp audio
    }

    /// <summary>
    /// Sends motor stop as Chirp audio
    /// </summary>
    public void SendMotorStop()
    {
        Debug.Log("Sending MotorStop");
        m_SpeedRightText.text = m_SpeedLeftText.text = "0"; // Set UI texts to display '0'
        ChirpManager.SendData(new byte[] { (byte)CommandID.MotorStop }); // Make a byte array with motor stop type, then send it as Chirp audio
    }

    /// <summary>
    /// Sets both UI texts to display each motor speed value respectively
    /// </summary>
    private void DisplayMotorSpeed(sbyte speedLeft, sbyte speedRight)
    {
        if(!TouchFieldActive) return;

        // Set UI texts to display both motor speeds
        m_SpeedLeftText.text = speedLeft.ToString();
        m_SpeedRightText.text = speedRight.ToString();
    }

    /// <summary>
    /// Progress bar update coroutine
    /// Updates radial progress bar while Chirp state is set to 'Sending'
    /// </summary>
    private IEnumerator UpdateProgressBar()
    {
        if(m_ProgressBarRevolutionTime <= 0f)
        {
            m_ProgressBarRoutine = null;
            yield break;
        }

        //m_ProgressBar.fillAmount = 0f;
        m_ProgressBar.fillClockwise = true;

        float timeLapse = 0f;
        while(m_ChirpState == ChirpStateEnum.Sending) // Run while Chirp is sending audio
        {
            float progress = Mathf.Clamp01(timeLapse / m_ProgressBarRevolutionTime);
            // Set progress bar fill according to fill direction
            if(m_ProgressBar.fillClockwise)
            {
                // Lerp towards 1, filling the circle with color
                m_ProgressBar.fillAmount = Mathf.Lerp(0f, 1f, progress);
            }
            else
            {
                // Lerp towards 0, withdraws the color from the circle in the opposite direction (creating an arbitrary progress 'animation')
                m_ProgressBar.fillAmount = Mathf.Lerp(1f, 0f, progress);
            }

            if(timeLapse < m_ProgressBarRevolutionTime) // Keep on filling until target value as been reached
            {
                timeLapse += UnityEngine.Time.unscaledDeltaTime;
            }
            else // Target value reached, (re)set values and go in the opposite direction
            {
                m_ProgressBar.fillClockwise = !m_ProgressBar.fillClockwise; // Invert fill direction (will indirectly also change target fill value)
                timeLapse = 0f; // Reset time elapsed
            }

            yield return null;
        }

        m_ProgressBar.fillAmount = 0f;
        //m_ProgressBar.fillClockwise = true;
        m_ProgressBarRoutine = null;
    }

    /// <summary>
    /// Starts the progress bar coroutine
    /// </summary>
    private void StartProgressBarCoroutine()
    {
        if(m_ProgressBarRoutine != null) return;

        m_ProgressBarRoutine = StartCoroutine(UpdateProgressBar());
    }

    /// <summary>
    /// Stops the progress bar coroutine
    /// </summary>
    private void StopProgressBarCoroutine()
    {
        if(m_ProgressBarRoutine == null) return;

        StopCoroutine(m_ProgressBarRoutine);
        m_ProgressBarRoutine = null;
    }

    /// <summary>
    /// Called by Unity (only once) when this script is instantiated, works as a constructor
    /// Sets Chirp SDK credentials
    /// </summary>
    private void Awake()
    {
        ChirpManager.Key = m_ChirpAppKey;
        ChirpManager.Secret = m_ChirpAppSecret;
        ChirpManager.Config = m_ChirpAppConfig;
    }

    /// <summary>
    /// Called by Unity (only once) when this script is instantiated (but after Awake) only if/when script is enabled
    /// Initializes Chirp SDK and UI components
    /// </summary>
    private void Start()
    {
        ChirpManager.InitSDK();
        ChirpManager.StartSDK();

        ChirpManager.OnStateChangedEvent = ChirpStateChanged;

        m_CancelButton.onClick.AddListener(() => TouchFieldActive = false);
        m_StopButton.onClick.AddListener(SendMotorStop);

        m_TouchFieldTopCenterPosition = new Vector2(m_TouchField.transform.position.x, m_TouchField.transform.position.y + m_TouchField.GetComponent<RectTransform>().rect.height) - (Vector2)m_TouchField.transform.position;

        TouchFieldActive = false; // Also (re)sets touch field alpha to initial value (just in case)
    }

    /// <summary>
    /// Called by Unity when this script is about to be destroyed
    /// Cleanup, stops Chirp SDK
    /// </summary>
    private void OnDestroy()
    {
        ChirpManager.ClearCallbacks(); // Clear Chirp callbacks
        ChirpManager.StopSDK();
    }

    /// <summary>
    /// Called by Unity each frame (if script is active)
    /// Checks for touch field input and handles what to happen
    /// </summary>
    private void Update()
    {
        if(Input.touchCount > 0) // If any touch input was detected
        {
            Touch touch = Input.GetTouch(0); // We need only the first one

            if(touch.phase == TouchPhase.Began && !TouchFieldActive) // The screen was touched
            {
                TouchFieldActive = m_TouchField.GetComponent<PointerClickHandler>().PointerDown; // If the touch was inside the touch field
                if(TouchFieldActive)
                {
                    m_TouchStartPosition = touch.position - (Vector2)m_TouchField.transform.position; // Calculate start position relative to the touch field

                    // Reset UI texts
                    m_SpeedLeftText.text = "0";
                    m_SpeedRightText.text = "0";
                }
            }
            else if(touch.phase == TouchPhase.Ended && TouchFieldActive) // The touch was released from the screen
            {
                m_TouchEndPosition = touch.position - (Vector2)m_TouchField.transform.position; // Calculate end position relative to the touch field

                Vector2Int motorValues = GetMotorSpeedValues();
                // Cast motor speed values to signed bytes
                sbyte speedLeft = (sbyte)motorValues.x;
                sbyte speedRight = (sbyte)motorValues.y;

                // Display values and send
                DisplayMotorSpeed(speedLeft, speedRight);
                SendMotorSpeed(speedLeft, speedRight);

                TouchFieldActive = false;
            }
            else if(touch.phase == TouchPhase.Moved && TouchFieldActive) // The touch is moving accross the screen
            {
                m_TouchEndPosition = touch.position - (Vector2)m_TouchField.transform.position; // Calculate end position relative to the touch field

                // Gets motor speed values and displays them on the screen so we can preview them before releasing
                Vector2Int motorValues = GetMotorSpeedValues();
                DisplayMotorSpeed((sbyte)motorValues.x, (sbyte)motorValues.y);
            }
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Displays simple GUI components
    /// Used here for debugging purposes
    /// </summary>
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
            $"Position\t= { m_TouchEndPosition.ToString("0.00") }\n" +
            $"Distance\t= { PointerMovementDistanceNormalized.ToString("0.00") }\n" +
            $"Deviation\t= { PointerDeviation.ToString("0.00") }",
            labelStyle
        );
    }
#endif
}
