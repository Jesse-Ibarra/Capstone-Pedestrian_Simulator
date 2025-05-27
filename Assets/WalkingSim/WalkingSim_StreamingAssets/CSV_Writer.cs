using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class CSV_Writer : MonoBehaviour {

    public GameObject car;
    public GameObject leftSensor;
    public GameObject rightSensor;

    private List<string[]> rowData = new List<string[]>();


    // Use this for initialization
    void Start()
    {
        Save();
    }

    void Save()
    {

        // Creating First row of titles manually..
        string[] rowDataTemp = new string[6];
        rowDataTemp[0] = "Car_Position_X";
        rowDataTemp[1] = "Car_Position_Y";
        rowDataTemp[2] = "Right_Edges_X";
        rowDataTemp[3] = "Right_Edges_Y";
        rowDataTemp[4] = "Left_Edges_X";
        rowDataTemp[5] = "Left_Edges_Y";
        rowData.Add(rowDataTemp);

        // You can add up the values in as many cells as you want.
        for (int i = 0; i < 10 ; i++)
        {
            rowDataTemp = new string[6];
            rowDataTemp[0] = car.transform.position.x.ToString(); // car-position --> car.transform x-coordinate
            rowDataTemp[1] = car.transform.position.y.ToString(); // car-position --> car.transform y-coordinate
            rowDataTemp[2] = "Data" + UnityEngine.Random.Range(5000, 10000); // raycast.hit.transform, right side, 
            rowDataTemp[3] = "Data" + i;
            rowDataTemp[4] = "Data" + i;
            rowDataTemp[5] = "Data" + i;
            rowData.Add(rowDataTemp);
        }

        string[][] output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));


        string filePath = getPath();

        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();
    }

    // Following method is used to retrive the relative path as device platform
    private string getPath()
    {
#if UNITY_EDITOR
        return Application.dataPath + "/Resources/" + "carUserData.csv";
#elif UNITY_ANDROID
        return Application.persistentDataPath+"carUserData.csv";
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+"carUserData.csv";
#else
        return Application.dataPath +"/"+"carUserData.csv";
#endif
    }
}
