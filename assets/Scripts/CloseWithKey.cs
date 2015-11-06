using UnityEngine;
using System.Collections;

public class CloseWithKey : MonoBehaviour {
    public KeyCode quitKey = KeyCode.Escape;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(quitKey))
        {
            Application.Quit();
        }
	}
}
