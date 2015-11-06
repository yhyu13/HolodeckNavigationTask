using UnityEngine;
using System.Collections;

public class Grabbable : MonoBehaviour
{
    public static KeyCode grabKeyCode = KeyCode.X;
    public static string buttonString = "a";
    public Transform character;
    public Transform originalParent;
    private Collider colliderToMonitor;

    void Start()
    {
        colliderToMonitor = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(character.position, character.forward);
        if (Input.GetKey(KeyCode.X) || Input.GetMouseButton(0) || Input.GetButton(buttonString))
        {
            if (Physics.Raycast(ray, out hit, 2f))
                hit.collider.gameObject.transform.parent = character;
        }
        else
        {
            transform.parent = originalParent;
        }
    }
}
