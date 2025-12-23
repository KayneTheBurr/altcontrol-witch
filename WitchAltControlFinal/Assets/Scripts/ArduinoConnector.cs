using UnityEngine;
using System.IO.Ports;

public class ArduinoConnector : MonoBehaviour
{
    public static ArduinoConnector instance;
    SerialPort serial = new SerialPort("COM5", 115200);
    public int[] sensorValues = new int[10];

    [Header("Runner Position (0..1)")]
    [Tooltip("0 = far right sensor (0), 1 = far left sensor (8)")]
    public float runner01;          // normalized 0..1 across the bar
    public float smoothedRunner01;  // smoothed for nicer movement
    public float smoothSpeed = 10f; // tweak in inspector

    [Header("Extra Button (sensor 9)")]
    public bool extraButtonPressed;
    public int extraButtonThreshold = 700;

    public bool darkMode = false;
    //public int buttonBaseline;
    //public int buttonMargin = 80;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        try
        {
            serial.ReadTimeout = 25;
            serial.Open();

            //Clear potential backfill junk lines
            serial.DiscardInBuffer();
            serial.DiscardOutBuffer();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Failed to open serial port: " + e.Message);
        }
    }
    private void Update()
    {
        if (serial == null || !serial.IsOpen) return;

        try
        {
            string latestLine = null;

            // clear out extra buffer lines 
            while (serial.BytesToRead > 0)
            {
                latestLine = serial.ReadLine();
            }

            // If we actually got at least one full line this frame, parse it
            if (!string.IsNullOrEmpty(latestLine))
            {
                string[] values = latestLine.Trim().Split(',');

                if (values.Length == sensorValues.Length)
                {
                    for (int i = 0; i < sensorValues.Length; i++)
                    {
                        if (int.TryParse(values[i], out int val))
                        {
                            sensorValues[i] = val;
                        }
                    }
                }
            }
        }
        catch (System.TimeoutException)
        {
            // no new full line this frame, just skip
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Serial read error: " + e.Message);
        }
        ProcessSensors();
    }

    void ProcessSensors()
    {
        int count = 9; // sensors 0..8 used for bar

        // 1) Find minimum value to treat as ambient baseline
        int minVal = int.MaxValue;
        for (int i = 0; i < count; i++)
        {
            if (sensorValues[i] < minVal)
                minVal = sensorValues[i];
        }

        // 2) Compute weighted average of brightness above ambient
        float weightedSum = 0f;
        float totalWeight = 0f;

        for (int i = 0; i < count; i++)
        {
            float w = sensorValues[i] - minVal;
            if (w < 0) w = 0; // clamp

            weightedSum += w * i;
            totalWeight += w;
        }

        float runnerIndex = 4f; // default to center if no light
        if (totalWeight > 0.0001f)
        {
            runnerIndex = weightedSum / totalWeight; // ~0..8
        }

        // 3) Normalize index 0..8 to 0..1
        // 0 = far right, 8 = far left (as per your wiring)
        runner01 = Mathf.InverseLerp(0f, 8f, runnerIndex);

        // 4) Smooth it for nicer motion
        smoothedRunner01 = Mathf.Lerp(smoothedRunner01, runner01, smoothSpeed * Time.deltaTime);

        // 5) Extra button on sensor 9
        extraButtonPressed = sensorValues[9] > extraButtonThreshold;
    }

    private void OnDestroy()
    {
        if(serial != null && serial.IsOpen) serial.Close();
    }
}
