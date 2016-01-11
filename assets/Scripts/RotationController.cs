using UnityEngine;
using System.Collections;

public class RotationController : MonoBehaviour {
    public float minOffset;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        float rot = UnityEngine.VR.InputTracking.GetLocalRotation(UnityEngine.VR.VRNode.CenterEye).eulerAngles.y;
        float offset = Input.GetAxis("Rotate");
        if (Mathf.Abs(offset) >= minOffset)
        {
            transform.localRotation *= Quaternion.Euler(0f, rot + offset, 0f);
            UnityEngine.VR.InputTracking.Recenter();
        }
	}
}
