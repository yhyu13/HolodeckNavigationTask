using UnityEngine;
using System.Collections;

public class LookController : MonoBehaviour {
    public Camera lookCamera;

	// Use this for initialization
	void Start () {
	
	}
    private bool down = false;
    private bool prevDown = false;
	// Update is called once per frame
	void Update () {
        down = Input.GetButtonDown("joystick button 0");
        if (!prevDown && down)
        {
        }
        prevDown = down;
	}
}
