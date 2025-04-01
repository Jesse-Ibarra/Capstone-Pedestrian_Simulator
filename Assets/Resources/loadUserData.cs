using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadUserData : MonoBehaviour
{
    List<savedUserData> session = new List<savedUserData>();
    
    // Start is called before the first frame update
    void Start()
    {
        TextAsset carUserData = Resources.Load<TextAsset>("carUserData");//reads file

        string[] data = carUserData.text.Split(new char[] { '\n' }); //create an array for the entire set of data once per line

        for (int i = 1; i < data.Length - 1; i++)
        {
            string[] row = data[i].Split(new char[] { ',' }); //for each line we create an another array is created which has the comma values in there
            savedUserData s = new savedUserData();

            float.TryParse(row[0], out s.Car_Position_X);
            float.TryParse(row[1], out s.Car_Position_Y);
            float.TryParse(row[2], out s.Right_Edges_X);
            float.TryParse(row[3], out s.Right_Edges_Y);
            float.TryParse(row[4], out s.Left_Edges_X);
            float.TryParse(row[5], out s.Left_Edges_Y);

            session.Add(s);
        }

        foreach (savedUserData s in session) {
            Debug.Log(s.Car_Position_X);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
