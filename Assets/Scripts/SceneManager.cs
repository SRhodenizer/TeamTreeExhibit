/**
 * Ardity (Serial Communication for Arduino + Unity)
 * Author: Daniel Wilches <dwilches@gmail.com>
 *
 * This work is released under the Creative Commons Attributions license.
 * https://creativecommons.org/licenses/by/2.0/
 */

using UnityEngine;
using System.Collections;

/**
 * Sample for reading using polling by yourself, and writing too.
 */
public class SceneManager : MonoBehaviour
{
    //GameObject for Arduino Connection
    SerialController serialController;

    //GameObjects For Simulation
    GameObject background;
    GameObject foreground;
    GameObject poof;
    GameObject wateringCan;
    GameObject fact;
    TextMesh treeText;
    GameObject hydrationLabel;
    GameObject lightLabel;
    GameObject winText;

    //Alternate Textures for GameObjects
    //background
    public Texture2D daytime;
    public Texture2D nighttime;
    //watering can
    public Texture2D waterIdle;
    public Texture2D waterPour;
    //foreground
    public Texture2D stage1;
    public Texture2D stage2;
    public Texture2D stage3;
    public Texture2D stage4;

    //timer based variables
    int millis = 0;
    int prevTime;
    int factTime;

    //game variables 
    bool planted = false;
    bool watering = false;
    bool dayLight = true;
    int hydration = 0;
    int lightLevel = 0;
    int lvl = 0;
    bool win = false;

    // Initialization
    void Start()
    {
        //get the arduino connection online 
        serialController = GameObject.Find("SerialController").GetComponent<SerialController>();

        //find all the gameobjects needed for the simulation
        background = GameObject.Find("Background");
        foreground = GameObject.Find("Foreground");
        poof = GameObject.Find("PoofEffect");
        fact = GameObject.Find("TreeFacts");
        treeText = GameObject.Find("TreeText").GetComponent<TextMesh>();
        wateringCan = GameObject.Find("WateringCan");
        hydrationLabel = GameObject.Find("Hydration");
        lightLabel = GameObject.Find("LightLevel");
        winText = GameObject.Find("WinScreen");
        
    }

    // Executed each frame
    void Update()
    {
        //update stat labels
        hydrationLabel.GetComponent<TextMesh>().text = "Hydration: " + hydration/10;
        lightLabel.GetComponent<TextMesh>().text = "Light: " + lightLevel / 10;

        //after awhile hydration goes down
        if (millis % 10 == 0 && hydration > 0) {
            hydration--;
        }

        //light level too high
        if (lightLevel > 700 * lvl) {
            
            lightLabel.GetComponent<TextMesh>().color = Color.red;
        }
        else {
            lightLabel.GetComponent<TextMesh>().color = Color.green;
        }

        //if hydration too high
        if (hydration > 700 * lvl) {

            hydrationLabel.GetComponent<TextMesh>().color = Color.red;
        }
        else {

            hydrationLabel.GetComponent<TextMesh>().color = Color.green;
        }

        
        //tree growth steps

        //lvl 2
        if (lvl == 1 && lightLevel > 600 && lightLevel < 800 && hydration > 600 && hydration < 800) {
            prevTime = millis;//set timer to millis
            factTime = millis;//set timer to millis
            lvl = 2;
            fact.GetComponent<SpriteRenderer>().enabled = true;
            treeText.text = "Over the course of its life a \nsingle tree can absorb one \nton of carbon dioxide.";
            poof.GetComponent<SpriteRenderer>().enabled = true;
            poof.GetComponent<AudioSource>().Play();
            foreground.GetComponent<SpriteRenderer>().sprite = Sprite.Create(stage2, new Rect(0, 0, stage2.width, stage2.height), new Vector2(0.5f, 0.5f));

        }

        //lvl 3
        if (lvl == 2 && lightLevel > 1200 && lightLevel < 1500 && hydration > 1200 && hydration < 1500)
        {
            prevTime = millis;//set timer to millis
            factTime = millis;//set timer to millis
            lvl = 3;
            fact.GetComponent<SpriteRenderer>().enabled = true;
            treeText.text = "Properly placed trees can \nreduce a building's air \nconditioning needs by 30%.";
            poof.GetComponent<SpriteRenderer>().enabled = true;
            poof.GetComponent<AudioSource>().Play();
            foreground.GetComponent<SpriteRenderer>().sprite = Sprite.Create(stage3, new Rect(0, 0, stage3.width, stage3.height), new Vector2(0.5f, 0.5f));

        }

        //lvl 4
        if (lvl == 3 && lightLevel > 1800 && lightLevel < 2000 && hydration > 1800 && hydration < 2000)
        {
            prevTime = millis;//set timer to millis
            factTime = millis;//set timer to millis
            lvl = 4;
            fact.GetComponent<SpriteRenderer>().enabled = true;
            treeText.text = "Shade and wind buffering \nprovided by trees reduces annual \nheating and cooling costs by 2.1 \nbillion dollars.";
            poof.GetComponent<SpriteRenderer>().enabled = true;
            poof.GetComponent<AudioSource>().Play();
            foreground.GetComponent<SpriteRenderer>().sprite = Sprite.Create(stage4, new Rect(0, 0, stage4.width, stage4.height), new Vector2(0.5f, 0.5f));

        }

        //trigger a game win screen
        if (lvl == 4 && lightLevel > 1850 && hydration > 1850) {
            win = true;
            winText.GetComponent<TextMesh>().text = "You Have Successfully Grown A Tree!";
            winText.GetComponent<TextMesh>().text = "You Have Successfully Grown A Tree!";
        }

        //timer for the poof animation 
        if (millis >= prevTime + 30)
        {
            poof.GetComponent<SpriteRenderer>().enabled = false;
        }
        //timer for tree facts
        if (millis >= factTime + 400)
        {
            fact.GetComponent<SpriteRenderer>().enabled = false;
            treeText.text = "";
        }

        //if you're watering the tree add water to the hydration stat
        if (watering == true && planted == true)
        {
            hydration++;
            Debug.Log("Hydration: "+hydration);
        }

        //if it's day time add to the light stat
        if (dayLight == true && planted == true) {
            lightLevel++;
            Debug.Log("Light: "+lightLevel);
        }

        //if it's night time subtract from the light stat
        if (dayLight == false && planted == true)
        {
            lightLevel--;
            Debug.Log("Light: " + lightLevel);
        }

        //timer counter incrementing 
        millis++;


        //---------------------------------------------------------------------
        // Receive data from the Arduino 
        //---------------------------------------------------------------------

        string message = serialController.ReadSerialMessage();

        if (message == null)
            return;

        // Check if the message is plain data or a connect/disconnect event.
        if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_CONNECTED))
            Debug.Log("Connection established");
        else if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_DISCONNECTED))
            Debug.Log("Connection attempt failed or disconnection detected");

        //list of messages recieved from the arduino that change the game states
        //user plants seed in pot
        else if (message == "seed planted")
        {
            Debug.Log("Seed Planted");
            planted = true;
            prevTime = millis;//set timer to millis
            factTime = millis;//set timer to millis
            lvl = 1;//lvl up the tree
            fact.GetComponent<SpriteRenderer>().enabled = true;
            treeText.text = "One tree can absorb as much \ncarbon in a year as a car \nproduces while driving \n26,000 miles.";
            poof.GetComponent<SpriteRenderer>().enabled = true;
            poof.GetComponent<AudioSource>().Play();
            foreground.GetComponent<SpriteRenderer>().sprite = Sprite.Create(stage1, new Rect(0, 0, stage1.width, stage1.height), new Vector2(0.5f, 0.5f));
        }
        else if (message == "watering begin")//watering can diverts from original orientation
        {
            Debug.Log("Watering Start");
            watering = true;
            wateringCan.GetComponent<SpriteRenderer>().sprite = Sprite.Create(waterPour, new Rect(0, 0, waterPour.width, waterPour.height), new Vector2(0.5f, 0.5f));
            wateringCan.GetComponent<AudioSource>().Play();
        }
        else if (message == "watering stop")//watering can returns to original orientation 
        {
            Debug.Log("Watering Stop");
            watering = false;
            wateringCan.GetComponent<SpriteRenderer>().sprite = Sprite.Create(waterIdle, new Rect(0, 0, waterIdle.width, waterIdle.height), new Vector2(0.5f, 0.5f));
            wateringCan.GetComponent<AudioSource>().Stop();
        }
        else if (message == "night switch")//switching to night with photoresistor
        {
            Debug.Log("Night Switch");
            dayLight = false;
            background.GetComponent<SpriteRenderer>().sprite = Sprite.Create(nighttime, new Rect(0, 0, nighttime.width, nighttime.height), new Vector2(0.5f, 0.5f));
        }
        else if (message == "day switch")//switching to day with photoresistor
        {
            Debug.Log("Day Switch");
            dayLight = true;
            background.GetComponent<SpriteRenderer>().sprite = Sprite.Create(daytime, new Rect(0, 0, daytime.width, daytime.height), new Vector2(0.5f, 0.5f));

        }
        else
            Debug.Log("Message arrived: " + message);
    }

    //UI Stuff for Buttons 
    private void OnGUI()
    {
        if (win == true) {
            if (GUI.Button(new Rect(Screen.width / 2 - 125, Screen.height / 2 + 50, 250, 60), "Click on this to plant your new tree!"))
            {
                Application.OpenURL("https://teamtrees.org/");
            }
        }
       
    }
}
