using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public SerialController arduinoConnection;
    Queue serialMessages;

    // Start is called before the first frame update
    void Start()
    {
        arduinoConnection = GameObject.Find("SerialController").GetComponent<SerialController>(); ;
    }

    // Update is called once per frame
    void Update()
    {
        string test = arduinoConnection.ReadSerialMessage();
        Debug.Log(test);
    }
}
