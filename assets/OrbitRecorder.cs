using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class OrbitRecorder : MonoBehaviour
{
    public string filename = "FileNameHere";
    public int fps = 24;
    public float lastFrameTime = 0.0f;
    public int imageIncrement = 0;

    private Camera orbitCam;
    private bool[] cameraStates;
    private Camera[] cameras;

    public Transform center;
    public Vector3 axis = Vector3.up;
    public float radius = 2.0f;
    public float radiusSpeed = 0.5f;
    public float rotationSpeed = 80.0f;

    public KeyCode recordButton = KeyCode.BackQuote;

    void Start()
    {
        transform.position = (transform.position - center.position).normalized * radius + center.position;
        orbitCam = GetComponent<Camera>();
    }

    void Update()
    {
        transform.LookAt(center);
        transform.RotateAround(center.position, axis, rotationSpeed * Time.deltaTime);
        Vector3 desiredPosition = (transform.position - center.position).normalized * radius + center.position;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * radiusSpeed);

        if (Input.GetKeyDown(recordButton))
        {
            List<bool> states = new List<bool>();
            List<Camera> cams = new List<Camera>();
            foreach (Camera c in Camera.allCameras)
            {
                cams.Add(c);
                states.Add(c.enabled);
                c.enabled = false;
            }
            cameraStates = states.ToArray();
            cameras = cams.ToArray();
            orbitCam.enabled = true;
        }

        if (Input.GetKeyUp(recordButton))
        {
            orbitCam.enabled = false;
            for (int i = 0; i < cameras.Length; i++)
                cameras[i].enabled = cameraStates[i];
        }

        if (Input.GetKey(recordButton))
        {
            RecordImages();
            orbitCam.enabled = true;
        }
    }

    void RecordImages()
    {
        if (lastFrameTime < Time.time + (1 / fps))
        {
            Application.CaptureScreenshot(filename + "" + imageIncrement.ToString("D8") + ".png");
            imageIncrement++;
            lastFrameTime = Time.time;
        }
    }
}