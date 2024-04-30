using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class SerialController : MonoBehaviour
{
    public static SerialController s;
    static SerialPort serialPort = new("COM10", 9600);
    static bool serialPortError = false;
    public GameObject SerialErrorMessage;
    public static event EventHandler<ButtonPressEventArgs> SerialError;
    public static event EventHandler<ButtonPressEventArgs> ButtonPress;

    void Awake()
    {
        GameObject[] serials = GameObject.FindGameObjectsWithTag("Serial");
        foreach (GameObject ser in serials) {
            SerialController tmp = ser.GetComponent<SerialController>();
            if (tmp != this) {
                Destroy(tmp.SerialErrorMessage);
                Destroy(ser);
            }
        }

        s = this;
        SerialErrorMessage.SetActive(false);
        AttemptSerialOpen();
        DontDestroyOnLoad(gameObject);
    }

    void AttemptSerialOpen() {
        serialPort.ReadTimeout = 1;
        serialPort.DtrEnable = true;

        try
        {
            if (!serialPort.IsOpen)
            {
                serialPort.Open();
            }
            
            if (serialPort.IsOpen)
            {
                SerialErrorMessage.SetActive(false);
                Debug.Log("Serial port is open!");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Serial Port Open Error: {ex.Message}");
            FireErrorEvent();
            serialPortError = true;
        }
    }

    public static bool SerialPortIsActive() {
        bool result = serialPort.IsOpen && !serialPortError;

        if (result) {
            s.SerialErrorMessage.SetActive(false);
        } else {
            FireErrorEvent();
        }

        return result;
    }

    static void FireErrorEvent() {
        s.SerialErrorMessage.SetActive(true);
        ButtonPressEventArgs args = new ButtonPressEventArgs();
        OnSerialError(args);
    }

    static void OnSerialError(ButtonPressEventArgs e) {
        EventHandler<ButtonPressEventArgs> handler = SerialError;
        if (handler != null) {
            handler(s, e);
        }
    }

    static void OnButtonPressed(ButtonPressEventArgs e) {
        EventHandler<ButtonPressEventArgs> handler = ButtonPress;
        if (handler != null) {
            handler(s, e);
        }
    }


    public static List<ButtonMap> ReadLine() {
        try
        {
            string read = serialPort.ReadLine().Trim();
            List<ButtonMap> pressed = new List<ButtonMap>();
            foreach (char k in read) {
                if (k != '-') {
                    pressed.Add((ButtonMap)k);
                }
            }
            
            //Fire any events
            ButtonPressEventArgs args = new ButtonPressEventArgs();
            args.buttons = pressed;
            OnButtonPressed(args);

            return pressed;
        }
        catch (TimeoutException)
        {
            //This just means no data was read
            return new List<ButtonMap>();
        }
    }
}

public class ButtonPressEventArgs {
    public List<ButtonMap> buttons { get; set; }
}
