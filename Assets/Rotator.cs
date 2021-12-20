using System.IO.Ports;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Rotator : MonoBehaviour {
    public int baudRate = 9600;
    public Button btn_openClose;
    public Dropdown port_choser;
    public Slider deg_or_rad;
    SerialPort connection;

    void Start() {
        port_choser.options.Clear();
        string[] ports = SerialPort.GetPortNames();
        foreach(string p in ports)
        {
            port_choser.options.Add(new Dropdown.OptionData(p));
        }
        port_choser.RefreshShownValue();
        btn_openClose.onClick.AddListener(open);
    }

    void open()
    {
        string port = port_choser.options[port_choser.value].text;
        connection = new SerialPort(port, baudRate);
        connection.ErrorReceived += OnSerialError;
        connection.Open();
        Debug.Log("Open " + port);
        btn_openClose.onClick.RemoveListener(open);
        btn_openClose.onClick.AddListener(close);
        btn_openClose.GetComponentInChildren<Text>().text = "Close";
    }

    void close()
    {
        connection.Close();
        Debug.Log("Close");
        btn_openClose.onClick.RemoveListener(close);
        btn_openClose.onClick.AddListener(open);
        btn_openClose.GetComponentInChildren<Text>().text = "Open";
    }

    private void OnSerialError(object sender, SerialErrorReceivedEventArgs e) {
        Debug.LogError($"Serial error: {e.EventType.ToString()}");
    }

    void Update()
    {
        //transform.rotation = Quaternion.Euler(0,45,0);
        if (connection != null && connection.IsOpen)
        {
            string[] lines = connection.ReadExisting().Split('\n');
            if (lines.Length == 1)
                return;
            string[] coords = lines[lines.Length - 2].Split(' ');
            if (coords.Length == 3)
            {
                float roll = float.Parse(coords[0]);
                float pitch = float.Parse(coords[1]);
                float yaw = float.Parse(coords[2]);
                if (deg_or_rad.value == 1.0)
                {
                    roll *= 180 / 3.141592654f;
                    pitch *= 180 / 3.141592654f;
                    yaw *= 180 / 3.141592654f;
                }


                transform.rotation = Quaternion.Euler(-roll, -yaw, -pitch);
                Debug.Log("X: "+ float.Parse(coords[0]));
            }
        }
    }

    void OnDestroy()
    {
        if (connection != null && connection.IsOpen)
            connection.Close();
    }
}