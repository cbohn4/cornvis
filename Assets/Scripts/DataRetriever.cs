using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Linq;
using System.Diagnostics;





public class DataRetriever : MonoBehaviour {

    public GameObject cornPrefab;
    System.TimeSpan time = System.DateTime.Now.TimeOfDay;
    public float spacing;
    
    public Camera cam1;
    public Camera cam2;
    public Camera cam3;
    public Camera sunCam;
    public float kMin;
    public float xPos = 0;
    public float kMax;
    public int useRandom;
    private Dictionary<string, string> jobsDict = new Dictionary<string, string>();
    private Dictionary<string, string> tempDict = new Dictionary<string, string>();
    private Dictionary<string, int> jobsTime = new Dictionary<string, int>();
    static int side = 49;
    float xCord = 0;//side * -1;
    float zCord = 0;//side * -1;
    private Vector3 point = new Vector3 (0,0,0);
    float h;
    
    
    
    // Use this for initialization
    void Start() {
        
        
        //Pull jobs from respawn file
        respawnCorn();
        //Prep "sun" and start default cameras.
        updateSun();
        sunCam.enabled = false;
        cam2.enabled = false;
        cam3.enabled = false;
        cam1.enabled = true;
        generate();
        //Fix corn spawn locations due to potential old jobs in the respawn file.
        resetCorn();
        respawnCorn();
        
        

        //Start corn tracker and orbit cam.
        InvokeRepeating("updateCorn", 5.0f, 5.0f);
        InvokeRepeating("rotation", 0.1f, 0.01f);


    }
    

    //Orbits the orbital camera around an invisible pivot
    private void rotation()
    {
        GameObject rotator = GameObject.Find("MagicalRotationalCylinder");
        rotator.transform.Rotate(0, 0.05f, 0);
    }


    //Kills the current corn and pulls the data from the respawn file.
    void respawnCorn()

    {
        jobsDict.Clear();
        tempDict.Clear();
        GameObject[] cornPlants = GameObject.FindGameObjectsWithTag("Corn");
        foreach (GameObject corn in cornPlants)
        {
            
            GameObject.Destroy(corn);

        }
        string[] tempArray;
        try
        {
            var lines = File.ReadAllLines(Application.dataPath + "\\respawn.csv");
            foreach (var line in lines)
            {
                tempArray = line.Split(',');
                //Debug.Log(tempStringArray[1]);
                if (!jobsDict.ContainsKey(tempArray[0]))
                {
                    //Debug.Log("Job added: " + line);
                    jobsDict.Add(tempArray[0], "RUNNING");
                    h = float.Parse(tempArray[1]);
                    SpawnCorn(tempArray[0], h);
                }
                tempDict.Add(tempArray[0], "RUNNING");


            }
            for (int i = jobsDict.Count - 1; i >= 0; i--)
            {
                var item = jobsDict.ElementAt(i);
                var itemKey = item.Key;
                if (!tempDict.ContainsKey(itemKey))
                {
                    jobsDict[itemKey] = "COMPLETED";
                }
            }
        }
        catch
        {
            UnityEngine.Debug.Log("Error reading file - Respawn");
        }
        tempDict.Clear();


    }


    
    //Kills all corn and stores their data in the respawn file. Resets starting positions for corn.
    private void resetCorn()
    {
        jobsDict.Clear();
        tempDict.Clear();
        xCord = 0;// side * -1;
        zCord = 0;// side * -1;
        File.Delete(Application.dataPath + "\\respawn.csv");
        StreamWriter writer = new StreamWriter(Application.dataPath + "\\respawn.csv", true);

         xLimitLow = -1;
         xLimitHigh = 1;
         zLimitLow = -1;
         zLimitHigh = 1;
         currentMotion = 0;

        GameObject[] cornPlants = GameObject.FindGameObjectsWithTag("Corn");
        foreach (GameObject corn in cornPlants)
        {
            writer.WriteLine(corn.name + "," + corn.transform.localScale.y);
            //UnityEngine.Debug.Log(corn.transform.localScale.y);
            GameObject.Destroy(corn);

        }
        writer.Close();
        //respawnCorn();
    }

    //Handle input from keyboard
    private void Update()
    {
        int height = 768;
        int width = 1024;

        if (Input.GetKeyDown(KeyCode.R)){
            resetCorn();
           // UnityEngine.Debug.Log("Corn Reset");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            respawnCorn();
           // UnityEngine.Debug.Log("Corn Respawn");
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            sunCam.enabled = false;
            cam1.enabled = true;
            cam2.enabled = false;
            cam3.enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            cam1.enabled = false;
            sunCam.enabled = false;
            cam2.enabled = true;
            cam3.enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            sunCam.enabled = false;
            cam1.enabled = false;
            cam2.enabled = false;
            cam3.enabled = true;
            
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            cam1.enabled = false;
            cam2.enabled = false;
            cam3.enabled = false;
            sunCam.enabled = true;
        }
        if (Input.GetKeyDown(KeyCode.F11))
        {
            if (!Screen.fullScreen)
            {
                height = Screen.height;
                width = Screen.width;
                Screen.SetResolution(5760, 3600, true);
            }
            else
            {
                Screen.SetResolution(width, height, false);
            }
        }


    }   


    //Checks status of corn and  determines next state, completed or still running. If its completed, kill it with fire, make it grow if running, but dont feed it after midnight.
    void updateCorn()
    {
        generateHeights();
        updateSun();
        time = System.DateTime.Now.TimeOfDay;
        if (time > new System.TimeSpan(23, 59, 00) && time < new System.TimeSpan(00, 01, 00))
        {
            resetCorn();
            xCord = 0;
            zCord = 0;

        }


        for (int i = jobsDict.Count - 1; i >= 0; i--)
        {
            
            var item = jobsDict.ElementAt(i);
            
            var itemKey = item.Key;
            var itemValue = item.Value;
            GameObject plant = GameObject.Find(itemKey);


            if (itemValue == "COMPLETED")
            {
                try
                {
                    plant.transform.GetComponent<Renderer>().material.color = Color.red;
                    GameObject corn = GameObject.Find(itemKey);
                    GameObject.Destroy(corn);
                }
                catch //occasional error will be a "fake" job that has no name, thus cant be destroyed above due to null value.
                {
                    jobsDict.Remove(itemKey);
                }
            }
            if(itemValue == "RUNNING")
            {
                //try
                {
                    var temp = jobsTime.ElementAt(i).Value / 15120;
                    plant.transform.localScale = new Vector3(temp/1.333f, temp, temp/1.333f);
                }
               // catch
                {
                    //jobsDict.Remove(itemKey);
                }
            }
        }
        generate();// <--- Checks for any new jobs at this point.

    }











    void SpawnCorn(string jobName, float height)
    {
        {
            var k = Random.Range(kMin, kMax);
            
            
            if (useRandom == 1)
            {
                spacing = k;
            }

            GameObject plant = Instantiate(cornPrefab, new Vector3(xCord + spacing, 0, zCord + spacing), Quaternion.identity) as GameObject;
            plant.name = jobName;
            var temp = jobsTime[jobName] / 15120.00f;
            plant.transform.localScale = new Vector3(temp / 1.333f, temp, temp / 1.333f);
            UpdateCords();
        }
    }

    

    // Generate spiral pattern around center
    int xLimitLow = -1;
    int xLimitHigh = 1;
    int zLimitLow = -1;
    int zLimitHigh = 1;
    int currentMotion = 0;
    float distanceDelta = 0.8f;

    void UpdateCords()
    {
        switch (currentMotion)
        {
            case 0: // Go right
                xCord = xCord + distanceDelta;
                if (xCord >= xLimitHigh)
                {
                    currentMotion = 1;
                    xLimitHigh = xLimitHigh + 1;
                }
                break;
            case 1: // Go up
                zCord = zCord + distanceDelta;
                if (zCord >= zLimitHigh)
                {
                    currentMotion = 2;
                    zLimitHigh = zLimitHigh + 1;
                }
                break;
            case 2: // Go left
                xCord = xCord - distanceDelta;
                if (xCord <= xLimitLow)
                {
                    currentMotion = 3;
                    xLimitLow = xLimitLow - 1;
                }
                break;
            case 3: // Go Down
                zCord = zCord - distanceDelta;
                if (zCord <= zLimitLow)
                {
                    currentMotion = 0;
                    zLimitLow = zLimitLow - 1;
                }
                break;
            default:
                UnityEngine.Debug.Log("Something Broke");
                break;

        }

    }
        /***
    {
        if (xCord < side)
            {
                xCord = xCord + 0.6f;
            }
        else
            {
                zCord = zCord + 0.6f;
                xCord = side * -1;
            }
        
    }*/


    //Spawn new corn and mark completed jobs.

    void generateHeights()
    {
        int numCol = 0;
        int numHyp = 0;
        int numSec = 0;
        var lines2 = File.ReadAllLines(Application.dataPath + "\\jobs.csv");
        foreach(var line2 in lines2) {
            if (!string.IsNullOrEmpty(line2))
            {
                var tempArray = line2.Split(' ');
                //UnityEngine.Debug.Log(line2);
                var line = tempArray[1];
                numCol = line.Split(':').Length - 1;
                if (numCol == 0) // SS
                {
                    var temp = line.Split(':');
                    numSec = System.Int32.Parse(temp[0]);
                }
                else if (numCol == 1)
                { // MM:SS
                    var temp = line.Split(':');
                    numSec = System.Int32.Parse(temp[0]) * 60;
                    numSec = numSec + System.Int32.Parse(temp[1]);
                }
                else if (numCol == 2) // HH:MM:SS
                {
                    var temp = line.Split(':');
                    numHyp = line.Split('-').Length - 1;
                    if (numHyp == 1)
                    {
                        var tempHyp = line.Split('-');
                        var tempCol = tempHyp[1].Split(':');

                        numSec = System.Int32.Parse(tempHyp[0]) * 86400; // DD:XX:XX:XX



                        numSec = numSec + System.Int32.Parse(tempCol[0]) * 3600; // HH
                        numSec = numSec + System.Int32.Parse(tempCol[1]) * 60; // MM
                        numSec = numSec + System.Int32.Parse(tempCol[2]) * 1; // SS


                    }
                    else
                    {
                        numSec = System.Int32.Parse(temp[0]) * 3600; // HH
                        numSec = numSec + System.Int32.Parse(temp[1]) * 60; // MM
                        numSec = numSec + System.Int32.Parse(temp[2]) * 1; // SS

                    }



                }


                if (!jobsTime.ContainsKey(tempArray[0]))
                {
                    jobsTime.Add(tempArray[0], numSec);
                }
                else if (jobsTime.ContainsKey(tempArray[0]))
                {
                    jobsTime[tempArray[0]] = numSec;
                }
            }
            
            




        }
        

    }

    void generate()
    {
        try
        {
            var lines = File.ReadAllLines(Application.dataPath + "\\jobs.csv");
            foreach (var line in lines)
            {
                if (line.Contains(':') && !string.IsNullOrEmpty(line))
                {
                    var tempArray = line.Split(' ');

                //Debug.Log(tempStringArray[1]);
                if (!jobsDict.ContainsKey(tempArray[0]))
                {
                    //Debug.Log("Job added: " + line);
                    jobsDict.Add(tempArray[0], "RUNNING");

                    SpawnCorn(tempArray[0], 0.02f);
                }
                tempDict.Add(tempArray[0], "RUNNING");
            }


                }
                for (int i = jobsDict.Count - 1; i >= 0; i--)
                {
                    var item = jobsDict.ElementAt(i);
                    var itemKey = item.Key;
                    if (!tempDict.ContainsKey(itemKey))
                    {
                        jobsDict[itemKey] = "COMPLETED";
                    GameObject corn = GameObject.Find(itemKey);
                    GameObject.Destroy(corn);
                    }
                }
            
        }
        catch
        {
            UnityEngine.Debug.Log("Error reading file");
        }
        tempDict.Clear();
    }



    //Print jobs dict, DEBUGGING
    void printJobsDict()
    {
        //jobsDict.ToList().ForEach(x => Debug.Log(x.Key));
        //var t = jobsDict["12934423"][5];
        //Debug.Log(t.ToString());
        var tempVal = 0;
        for (int i = jobsTime.Count - 1; i >= 0; i--)
        {
            var item = jobsTime.ElementAt(i);
            var itemKey = item.Key;
            var itemValue = item.Value;
            tempVal = tempVal + itemValue;
        }
        UnityEngine.Debug.Log(tempVal);
        UnityEngine.Debug.Log((jobsTime.Count));
    }

    //Update position of sun in order to create realisitc shadows, corn must be evenly cooked.
    void updateSun()
    {
        
        time = System.DateTime.Now.TimeOfDay;
        xPos = 4.1665f * (System.DateTime.Now.Hour) - 50;
        xPos = (1 / 60) * (System.DateTime.Now.Hour);
        sunCam.transform.LookAt(point);
        sunCam.transform.position = new Vector3(xPos, (-0.01f * xPos + 25), -50.0f);
    }


}
