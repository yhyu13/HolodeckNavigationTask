using UnityEngine;
using System.Collections;

public class RotationController : MonoBehaviour {
    public float minOffset;
    public Camera lookCamera;

	// Use this for initialization
	void Start () {
        enabled = PlayerPrefs.GetInt("ManualRotation", 0) == 1;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (enabled)
        {
            //float rot = UnityEngine.VR.InputTracking.GetLocalRotation(UnityEngine.VR.VRNode.CenterEye).eulerAngles.y;
            float rotBody = transform.rotation.y;
            float offset = Input.GetAxis("Rotate");
            if (Mathf.Abs(offset) >= minOffset)
            {
                float newY = offset + rotBody;
                transform.Rotate(0f, offset, 0f);
                lookCamera.transform.parent.rotation = transform.rotation;
                //UnityEngine.VR.InputTracking.Recenter();
            }
        }
	}
}
