using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Linq;
using System.Diagnostics;


public class DataRetriever : MonoBehaviour {

    public GameObject cornPrefab;

    public float spacing;
    public Camera cam1;
    public Camera cam2;
    public Camera cam3;
    public float kMin;
    public float kMax;
    public int useRandom;
    private Dictionary<string, string> jobsDict = new Dictionary<string, string>();
    private Dictionary<string, string> tempDict = new Dictionary<string, string>();
    static int side = 49;
    float xCord = side * -1;
    float zCord = side * -1;
    
    // Use this for initialization
    void Start() {
        /**
        string batchPath = Application.dataPath + "\\getData.bat";
        File.Delete(batchPath);
        StreamWriter writer = new StreamWriter(batchPath, true);
        writer.WriteLine("C:\\cygwin64\\bin\\ssh root@crane-head -i " + Application.dataPath + "\\cornFieldKey > " + Application.dataPath + "\\jobs.csv");
        writer.Close();
        
        //writer = new StreamWriter(Application.dataPath+"\\jobs.csv", true);
        //writer.WriteLine("LIFE");
        //writer.Close();
        //System.Threading.Thread.Sleep(2000);**/


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

    private void resetCorn()
    {
        GameObject[] cornPlants = GameObject.FindGameObjectsWithTag("Corn");
        foreach (GameObject corn in cornPlants)
            GameObject.Destroy(corn);
    }


    private float k;

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.R)){
            resetCorn();
            //Debug.Log("Corn Reset");
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            cam1.enabled = true;
            cam2.enabled = false;
            cam3.enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            cam1.enabled = false;
            cam2.enabled = true;
            cam3.enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            cam1.enabled = false;
            cam2.enabled = false;
            cam3.enabled = true;
        }

    }   


    void updateCorn()
    {



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
                plant.transform.GetComponent<Renderer>().material.color = Color.red;
            }
            if(itemValue == "RUNNING")
            {
                //Debug.Log("RUNNING");
                plant.transform.localScale += new Vector3(0, 0.01f, 0);
            }
        }
        generate();

    }

    void SpawnCorn(string jobName)
    {
        
        
        {
            k = Random.Range(kMin, kMax);
            
            
            if (useRandom == 1)
            {
                spacing = k;
            }

            GameObject plant = Instantiate(cornPrefab, new Vector3(xCord + spacing, 0, zCord + spacing), Quaternion.identity) as GameObject;
            plant.name = jobName;
            plant.transform.localScale += new Vector3(0, 0.02f, 0);
            UpdateCords();
        }
    }

    void UpdateCords()
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
        
    }

   

    void generate()
    {
        //Process.Start("startData.bat");

        //Process myProcess = new Process();
       // try
       // {
            //myProcess.StartInfo.UseShellExecute = false;
            //myProcess.StartInfo.FileName = Application.dataPath + "\\getData.bat";
            //myProcess.StartInfo.FileName = "C:\\cygwin64\\bin\\ssh root@crane-head -i cornFieldKey > " + Application.dataPath + "\\jobs.csv";
            //UnityEngine.Debug.Log("C:\\cygwin64\\bin\\ssh root@crane-head -i cornFieldKey > " + Application.dataPath + "\\jobs.csv");
            //myProcess.StartInfo.CreateNoWindow = true;
           // myProcess.Start();

        //}
        //catch
        //{
           // UnityEngine.Debug.Log("Error in getting data.");
        //}


        try
        {
            var lines = File.ReadAllLines(Application.dataPath + "\\jobs.csv");
            foreach (var line in lines)
            {

                //Debug.Log(tempStringArray[1]);
                if (!jobsDict.ContainsKey(line))
                {
                    //Debug.Log("Job added: " + line);
                    jobsDict.Add(line, "RUNNING");

                    SpawnCorn(line);
                }
                tempDict.Add(line, "RUNNING");


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


}
