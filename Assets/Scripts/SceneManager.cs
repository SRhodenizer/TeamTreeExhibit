using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public GameObject arduinoConnection;

    // Start is called before the first frame update
    void Start()
    {
        arduinoConnection = GameObject.Find("SerialController");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
