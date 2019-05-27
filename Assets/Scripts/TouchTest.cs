using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using Assets.Scripts.Components;

public class TouchTest : MonoBehaviour
{
    private Vector3 position;
    private float width;
    private float height;
    public Image TouchField;
    public Button StopButton;
    public Text MotorValuesText;
    private Vector2 m_TouchFieldTopPosition;

    void Awake()
    {
        width = (float)Screen.width / 2.0f;
        height = (float)Screen.height / 2.0f;

        // Position used for the cube.
        position = new Vector3(0.0f, 0.0f, 0.0f);
        StopButton.onClick.AddListener(SendMotorStop);
    }

    void SendMotorStop()
    {
        byte[] payload = new byte[] { 0x04 };
        //ChirpManager.Instance.SendData(payload);
    }

    void Start()
    {
        Debug.Log(TouchField.GetComponent<RectTransform>().rect);
        ChirpManager.Instance.StartSDK();
        m_TouchFieldTopPosition = new Vector2(TouchField.transform.position.x, TouchField.transform.position.y + TouchField.GetComponent<RectTransform>().rect.height) - (Vector2)TouchField.transform.position;
        //GoImage.GetComponent<PointerOverlapHandler>().OnPointerEnterEvent.AddListener((a) => Debug.Log("enter"));
        //GoImage.GetComponent<PointerOverlapHandler>().OnPointerExitEvent.AddListener((a) => Debug.Log("exit"));
    }

    private float Dot
    {
        get { return Vector2.Dot(m_TouchFieldTopPosition.normalized, m_EndPosition.normalized); }
    }

    private float DotNormalized
    {
        get { return Mathf.Asin(Dot) / Mathf.PI * 2; }
    }

    private Vector2 Cross
    {
        get { return Vector2.Perpendicular(m_TouchFieldTopPosition - m_EndPosition).normalized; }
    }

    private Vector2 CrossNormalized
    {
        get { return Vector2.Perpendicular(m_TouchFieldTopPosition.normalized - m_EndPosition.normalized).normalized; }
    }

    private Vector2 Pos
    {
        get { return new Vector2(-CrossNormalized.y, Mathf.Clamp(DotNormalized, -1f, 1f)); }
    }

    private float Dist
    {
        get { return (Vector2.Distance(m_StartPosition, m_EndPosition) * 2) / (TouchField.GetComponent<RectTransform>().rect.height / 2); }
    }

    void OnGUI()
    {
        // Compute a fontSize based on the size of the screen width.
        GUI.skin.label.fontSize = (int)(Screen.width / 25.0f);

        GUI.Label(new Rect(20, 20, width, height * 0.25f),
            "x = " + position.x.ToString("f2") +
            ", y = " + position.y.ToString("f2") +
            ", dot = " + string.Format("{0}", Pos) +
            ", dist = " + string.Format("{0}", Dist)
        );
    }

    [SerializeField, ReadOnlyProperty]
    private Vector2 m_StartPosition = Vector2.zero;
    [SerializeField, ReadOnlyProperty]
    private Vector2 m_EndPosition = Vector2.zero;
    [SerializeField, ReadOnlyProperty]
    private bool m_TouchFieldActive = false;

    float Neg(float value) { return value > 0f ? -value : value; }

    Vector2Int MotorValues
    {
        get
        {
            float power = Mathf.Clamp01(Dist);
            Vector2 motorValues = new Vector2(power, power);
            Vector2 coords = Pos;
            if(coords.x < 0) motorValues.x *= DotNormalized;
            else if(coords.x > 0) motorValues.y *= DotNormalized;

            if(coords.y < 0)
            {
                motorValues.x = Neg(motorValues.x);
                motorValues.y = Neg(motorValues.y);
            }

            motorValues *= 100;
            return new Vector2Int((int)motorValues.x, (int)motorValues.y);
        }
    }

    void SetText()
    {
        if(!m_TouchFieldActive) return;

        Vector2Int motorValues = MotorValues;
        MotorValuesText.text = $"{ motorValues.x }x{ motorValues.y }";
    }
    void Update()
    {
        // Handle screen touches.
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Move the cube if the screen has the finger moving.
            if(touch.phase == TouchPhase.Began && !m_TouchFieldActive)
            {
                m_TouchFieldActive = TouchField.GetComponent<PointerClickHandler>().PointerDown;
                m_StartPosition = touch.position - (Vector2)TouchField.transform.position;

                MotorValuesText.text = string.Empty;
            }
            else if(touch.phase == TouchPhase.Ended && m_TouchFieldActive)
            {
                m_EndPosition = touch.position - (Vector2)TouchField.transform.position;

                SetText();
                Vector2Int motorValues = MotorValues;
                byte[] payload = new byte[] { 0x03, (byte)MotorValues.x, (byte)MotorValues.y };
                //ChirpManager.Instance.SendData(payload);
                m_TouchFieldActive = false;
            }
            else if(touch.phase == TouchPhase.Moved && m_TouchFieldActive)
            {
                Vector2 pos = touch.position;
                pos.x = (pos.x - width) / width;
                pos.y = (pos.y - height) / height;
                position = new Vector3(-pos.x, pos.y, 0.0f);

                //SetText();

                //Debug.Log("Moved");
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    float dot = Vector2.Dot(GoImage.transform.position.normalized, m_EndPosition.normalized);
    //    float dotn = Mathf.Asin(dot) / Mathf.PI * 2;
    //    Handles.Label(new Vector2(100f, 100f), string.Format("{0:0.00}", dotn));
    //}
}
