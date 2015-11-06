using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class PositionLogReader : MonoBehaviour {

    public string rootSearchPath;
    public string regexString;

    public GameObject markerPrefab;

    private Vector3[][] mazePositions;
    private Quaternion[][] mazeQuaternion;
    private double[][] mazeTimes;
    private string[][] mazeEvents;
    private Vector3[][] flatPositions;
    private Quaternion[][] flatQuaternion;
    private double[][] flatTimes;
    private string[][] flatEvents;

    private GameObject[] mazeObjects;
    private GameObject[] flatObjects;

    private LineRenderer[] mazeObjectLines;
    private LineRenderer[] flatObjectLines;

    public Material mazeMaterials;
    public Material flatMaterials;

    public float lineWidth;

    private int index;

    public int numIterationsPerUpdate = 1;

    public bool fixHeight = false;
    public float fixedHeight = 1f;
    public bool loop = false;

    private float[] flatDistances;
    private float[] flatArcDistances;

    // Use this for initialization
    void Start () {
        Regex reg = new Regex(regexString);
        var files = Directory.GetFiles(rootSearchPath, "*.csv", SearchOption.AllDirectories).Where(path => reg.IsMatch(path));

        List<string> mazeLogsList = new List<string>();
        List<string> flatLogsList = new List<string>();

        foreach (string filename in files)
            flatLogsList.Add(filename);

        string[] mazeLogs = mazeLogsList.ToArray();
        string[] flatLogs = flatLogsList.ToArray();

        mazePositions = new Vector3[mazeLogs.Length][];
        mazeQuaternion = new Quaternion[mazeLogs.Length][];
        mazeTimes = new double[mazeLogs.Length][];
        mazeEvents = new string[mazeLogs.Length][];
        mazeObjects = new GameObject[mazeLogs.Length];
        mazeObjectLines = new LineRenderer[mazeLogs.Length];
        for (int i = 0; i < mazeLogs.Length; i++)
        {
            Vector3[] positions;
            Quaternion[] rotations;
            double[] times;
            string[] events;
            getCoordinates(mazeLogs[i], out positions, out rotations, out times, out events);
            mazePositions[i] = positions;
            mazeQuaternion[i] = rotations;
            mazeTimes[i] = times;
            mazeEvents[i] = events;

            mazeObjects[i] = Instantiate(markerPrefab);
            mazeObjectLines[i] = mazeObjects[i].AddComponent<LineRenderer>();
            mazeObjectLines[i].SetWidth(lineWidth, lineWidth);
            mazeObjectLines[i].material = mazeMaterials;
        }

        flatPositions = new Vector3[flatLogs.Length][];
        flatQuaternion = new Quaternion[flatLogs.Length][];
        flatTimes = new double[flatLogs.Length][];
        flatEvents = new string[flatLogs.Length][];
        flatObjects = new GameObject[flatLogs.Length];
        flatObjectLines = new LineRenderer[flatLogs.Length];

        flatDistances = new float[flatLogs.Length];
        flatArcDistances = new float[flatLogs.Length];
        bool[] isMazeVals = new bool[flatLogs.Length];
        double[] timeDiffs = new double[flatLogs.Length];
        for (int i = 0; i < flatLogs.Length; i++)
        {
            Vector3[] positions;
            Quaternion[] rotations;
            double[] times;
            string[] events;
            getCoordinates(flatLogs[i], out positions, out rotations, out times, out events);
            flatPositions[i] = positions;
            flatQuaternion[i] = rotations;
            flatTimes[i] = times;
            flatEvents[i] = events;
            flatDistances[i] = calculateTotalDistanceTravelled(flatPositions[i]);
            flatArcDistances[i] = calculateTotalArcDistanceTravelled(flatQuaternion[i]);
            flatObjects[i] = Instantiate(markerPrefab);
            flatObjectLines[i] = flatObjects[i].AddComponent<LineRenderer>();
            flatObjectLines[i].SetWidth(lineWidth, lineWidth);
            flatObjectLines[i].material = flatMaterials;

            timeDiffs[i] = generateTimeDiffs(flatLogs[i]);
            isMazeVals[i] = isMaze(flatLogs[i]);
        }

        string filenames = string.Join(",", flatLogs).Trim();
        filenames.Remove(filenames.Length - 1);
        string dist = string.Join(" ", Array.ConvertAll(flatDistances, element => element.ToString())).Trim().Replace(" ", ",");
        string arc = string.Join(" ", Array.ConvertAll(flatArcDistances, element => element.ToString())).Trim().Replace(" ", ",");
        string areMazes = string.Join(" ", Array.ConvertAll(isMazeVals, element => element.ToString())).Trim().Replace(" ", ",");
        string timeDiffsStr = string.Join(" ", Array.ConvertAll(timeDiffs, element => element.ToString())).Trim().Replace(" ", ",");

        StreamWriter writer = new StreamWriter(@"C:\Users\Kevin\Desktop\summary.csv");
        writer.WriteLine(filenames);
        writer.WriteLine(dist);
        writer.WriteLine(arc);
        writer.WriteLine(areMazes);
        writer.WriteLine(timeDiffsStr);
        writer.Close();

        index = 0;
	}

	// Update is called once per frame
	void FixedUpdate () {
        for (int k = 0; k < numIterationsPerUpdate; k++)
        {
            for (int i = 0; i < flatObjects.Length; i++)
            {
                if (!loop && index >= flatPositions[i].Length)
                    continue;
                int objIndex = index % flatPositions[i].Length;
                flatObjects[i].transform.position = flatPositions[i][objIndex];
                flatObjects[i].transform.rotation = flatQuaternion[i][objIndex];
                flatObjectLines[i].SetVertexCount(objIndex + 1);
                flatObjectLines[i].SetPosition(objIndex, flatPositions[i][objIndex]);
            }

            for (int i = 0; i < mazeObjects.Length; i++)
            {
                if (!loop && index >= mazePositions[i].Length)
                    continue;
                int objIndex = index % mazePositions[i].Length;
                mazeObjects[i].transform.position = mazePositions[objIndex][i];
                mazeObjects[i].transform.rotation = mazeQuaternion[objIndex][i];
                mazeObjectLines[i].SetVertexCount(objIndex + 1);
                mazeObjectLines[i].SetPosition(objIndex, mazePositions[objIndex][i]);
            }

            index++;
        }
	}

    float calculateTotalDistanceTravelled(Vector3[] positions)
    {
        float dist = 0;
        for(int i = 0; i < positions.Length-1; i++)
            dist += Vector3.Distance(positions[i], positions[i + 1]);
        return dist;
    }

    float calculateTotalArcDistanceTravelled(Quaternion[] rotations)
    {
        float dist = 0;
        for (int i = 0; i < rotations.Length - 1; i++)
            dist += Vector3.Distance(rotations[i].eulerAngles, rotations[i + 1].eulerAngles);
        return dist;
    }

    bool isMaze(string rawFilename)
    {
        string summaryFilename = rawFilename.Replace("Raw", "Summary");
        StreamReader reader = new StreamReader(summaryFilename);
        string line = reader.ReadLine();
        reader.Close();
        return line.ToLower().Contains("maze");
    }

    double generateTimeDiffs(string rawFilename)
    {
        string summaryFilename = rawFilename.Replace("Raw", "Summary");
        StreamReader reader = new StreamReader(summaryFilename);
        reader.ReadLine();
        reader.ReadLine();
        string firstLine = reader.ReadLine();
        string lastLine = reader.ReadLine();
        while (!reader.EndOfStream)
        {
            string tmpLine = reader.ReadLine();
            if (tmpLine.Trim() != "")
                lastLine = tmpLine;
        }
        reader.Close();

        double firstTime = double.Parse(firstLine.Split(new char[] { ',' })[0]);
        double lastTime = double.Parse(lastLine.Split(new char[] { ',' })[0]);

        return Math.Abs(lastTime - firstTime);
    }

    void getCoordinates(string filename, out Vector3[] positions, out Quaternion[] rotations, out double[] times, out string[] events)
    {
        List<Vector3> posList = new List<Vector3>();
        List<Quaternion> rotList = new List<Quaternion>();
        List<double> timesList = new List<double>();
        List<string> eventsList = new List<string>();
        StreamReader reader = new StreamReader(filename);

        int lineNum = 0;
        bool prevLineWasTime = false;
        Vector3 prevPoint = Vector3.zero;
        Quaternion prevQuat = Quaternion.identity;
        while (!reader.EndOfStream)
        {
            string l = reader.ReadLine();
            if (l.Substring(0, 12) == "End of Trial")
                continue;
            else if (l.Substring(0, 3) == "End")
                continue;
            if (l.Substring(0, 1) == "-") {
                if (l.Contains(" ")) {  // This means there was an event
                    string[] eventSplit = l.Split(new char[] { ' ' });
                    timesList.Add(double.Parse(eventSplit[0]));
                    eventsList.Add(eventSplit[1]);
                }
                else {
                    timesList.Add(double.Parse(l));
                    eventsList.Add("");
                }
                if (lineNum != 0) {
                    timesList[timesList.Count - 1] = (timesList[0] - Mathf.Abs((float)timesList[timesList.Count - 1])) * 0.0000001;
                }
                else {
                    timesList[0] = Mathf.Abs((float)timesList[0]);
                }
                if (prevLineWasTime) {
                    posList.Add(prevPoint);
                    rotList.Add(prevQuat);
                }
                prevLineWasTime = true;
            }
            else {
                string[] lineSplit = l.Split(new char[] { ':' });
                string[] pointSplit = lineSplit[1].Split(new char[] { ',' });
                Vector3 pos = new Vector3(float.Parse(pointSplit[0]), float.Parse(pointSplit[1]), float.Parse(pointSplit[2]));
                if (fixHeight)
                    pos.y = fixedHeight;
                Quaternion rot = new Quaternion(float.Parse(pointSplit[3]), float.Parse(pointSplit[4]), float.Parse(pointSplit[5]), float.Parse(pointSplit[6]));
                prevPoint = pos;
                prevQuat = rot;
                posList.Add(pos);
                rotList.Add(rot);
                prevLineWasTime = false;
            }
            lineNum += 1;
        }
        timesList[0] = 0;

        positions = posList.ToArray();
        rotations = rotList.ToArray();
        times = timesList.ToArray();
        events = eventsList.ToArray();
    }
}
