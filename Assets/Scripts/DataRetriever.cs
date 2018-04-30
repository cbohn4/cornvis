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
    static int side = 49;
    float xCord = 0;//side * -1;
    float zCord = 0;//side * -1;
    private Vector3 point = new Vector3 (0,0,0);
    float h;
    
    
    // Use this for initialization
    void Start() {
        
        respawnCorn();
        updateSun();
        sunCam.enabled = false;
        cam2.enabled = false;
        cam3.enabled = false;
        cam1.enabled = true;
        UnityEngine.Debug.Log(Application.dataPath);
        generate();
        //printJobsDict();
        
        


        InvokeRepeating("updateCorn", 5.0f, 5.0f);
        InvokeRepeating("rotation", 0.1f, 0.01f);


    }
    private void rotation()
    {
        GameObject rotator = GameObject.Find("MagicalRotationalCylinder");
        rotator.transform.Rotate(0, 0.05f, 0);
    }

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
















    private void resetCorn()
    {
        jobsDict.Clear();
        tempDict.Clear();
        xCord = side * -1;
        zCord = side * -1;
        File.Delete(Application.dataPath + "\\respawn.csv");
        StreamWriter writer = new StreamWriter(Application.dataPath + "\\respawn.csv", true);
        
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


    private float k;








    void spiral()
    {

    }









    private void Update()
    {
        
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

    }   











    void updateCorn()
    {
        updateSun();
        time = System.DateTime.Now.TimeOfDay;
        if (time > new System.TimeSpan(23, 59, 00) && time < new System.TimeSpan(00, 01, 00))
        {
            resetCorn();
            xCord = side * -1;
            zCord = side * -1;

        }


        for (int i = jobsDict.Count - 1; i >= 0; i--)
        {
            
            var item = jobsDict.ElementAt(i);
            var itemKey = item.Key;
            var itemValue = item.Value;
            //Debug.Log(itemKey + itemValue);
            GameObject plant = GameObject.Find(itemKey);


            if (itemValue == "COMPLETED")
            {
                //Debug.Log("COMPLETED");
                try
                {
                    plant.transform.GetComponent<Renderer>().material.color = Color.red;
                }
                catch
                {
                    UnityEngine.Debug.Log("Removing Job: " + itemKey);
                    jobsDict.Remove(itemKey);
                }
            }
            if(itemValue == "RUNNING")
            {
                //Debug.Log("RUNNING");
                try
                {
                    plant.transform.localScale += new Vector3(0, 0.01f, 0);
                }
                catch
                {
                    UnityEngine.Debug.Log("Removing Job: " + itemKey);
                    jobsDict.Remove(itemKey);
                }
            }
        }
        generate();

    }











    void SpawnCorn(string jobName, float height)
    {
        
        
        {
            k = Random.Range(kMin, kMax);
            
            
            if (useRandom == 1)
            {
                spacing = k;
            }

            GameObject plant = Instantiate(cornPrefab, new Vector3(xCord + spacing, 0, zCord + spacing), Quaternion.identity) as GameObject;
            plant.name = jobName;
            plant.transform.localScale += new Vector3(0, height, 0);
            UpdateCords();
        }
    }











    int xLimitLow = -1;
    int xLimitHigh = 1;
    int zLimitLow = -1;
    int zLimitHigh = 1;
    int currentMotion = 0;
    float distanceDelta = 0.5f;

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
        
    }***/

   







    




    void generate()
    {
        try
        {
            var lines = File.ReadAllLines(Application.dataPath + "\\jobs.csv");
            foreach (var line in lines)
            {
                if (!line.Contains(':') && !string.IsNullOrEmpty(line))
                {


                //Debug.Log(tempStringArray[1]);
                if (!jobsDict.ContainsKey(line))
                {
                    //Debug.Log("Job added: " + line);
                    jobsDict.Add(line, "RUNNING");

                    SpawnCorn(line, 0.02f);
                }
                tempDict.Add(line, "RUNNING");
            }


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
            UnityEngine.Debug.Log("Error reading file");
        }
        tempDict.Clear();
    }













    void printJobsDict()
    {
        //jobsDict.ToList().ForEach(x => Debug.Log(x.Key));
        //var t = jobsDict["12934423"][5];
        //Debug.Log(t.ToString());
        for (int i = jobsDict.Count - 1; i >= 0; i--)
        {
            var item = jobsDict.ElementAt(i);
            var itemKey = item.Key;
            var itemValue = item.Value;
            //Debug.Log(itemKey + " --- " + itemValue);
        }
    }

    









    void updateSun()
    {
        
        time = System.DateTime.Now.TimeOfDay;
        xPos = 4.1665f * (System.DateTime.Now.Hour) - 50;
        xPos = (1 / 60) * (System.DateTime.Now.Hour);
        sunCam.transform.LookAt(point);
        sunCam.transform.position = new Vector3(xPos, (-0.01f * xPos + 25), -50.0f);
    }


}
