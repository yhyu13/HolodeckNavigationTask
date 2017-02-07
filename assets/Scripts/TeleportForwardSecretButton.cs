using UnityEngine;
using System.Collections;

public class TeleportForwardSecretButton : MonoBehaviour {
    
    public KeyCode secretButton = KeyCode.KeypadPlus;
    private bool previousState = false;

    public float teleportDistance = 2f;
	// Update is called once per frame
	void Update () {
        bool currentState = Input.GetKey(secretButton);
        bool risingEdge = !previousState && currentState;
        if (risingEdge)
            transform.position += transform.forward * teleportDistance;
        previousState = currentState;
	}
}
