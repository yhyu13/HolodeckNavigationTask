using UnityEngine;
using System.Collections;

public class Grabber : MonoBehaviour {
    public LayerMask interactLayer;
    private float dist;
    public KeyCode grabKey = KeyCode.X;
    private Transform target;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(grabKey))
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out hit, 2f))
                if (hit.collider != null)
                    hit.collider.enabled = false;
        }
        Debug.DrawRay(transform.position, transform.forward * 2f, Color.green);
    }
}
