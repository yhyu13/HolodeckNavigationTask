using UnityEngine;
using System.Collections;

public class ScreenRecorder : MonoBehaviour {

    public string directory;
    private int frame;
	// Use this for initialization
	void Start () {
        frame = 0;
	}
	
	// Update is called once per frame
	void Update () {
        Application.CaptureScreenshot(directory + "\\" + "Screenshot" + frame.ToString("D4") + ".png");
        frame++;
    }
}
