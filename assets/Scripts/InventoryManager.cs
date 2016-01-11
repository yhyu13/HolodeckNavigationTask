using UnityEngine;
using System.Collections;

public class InventoryManager : MonoBehaviour {

    public GameObject displayObject;
    private MeshRenderer displayObjectRenderer;
    public Material emptyMaterial;
    public static KeyCode grabKeyCode = KeyCode.X;
    public static string buttonString = "a";
    public Transform originalParent;
    public bool holdingObject = false;
    public bool previousInputState = false;
    public GameObject objectBeingHeld;
    public float placeDistance = 1f;
    // Use this for initialization
    void Start () {
        displayObjectRenderer = displayObject.GetComponent<MeshRenderer>();
        displayObjectRenderer.material = emptyMaterial;
	}

    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        bool inputState = Input.GetKey(KeyCode.X) || Input.GetMouseButton(0) || Input.GetButton(buttonString);
        bool risingEdge = inputState && !previousInputState;
        if (risingEdge)
        {
            if (!holdingObject)
            {
                if (Physics.Raycast(ray, out hit, 2f))
                {
                    if (hit.collider.gameObject.GetComponent<Grabbable>() != null)
                    {
                        //hit.collider.gameObject.transform.parent = transform;
                        SetMaterial(hit.collider.gameObject.GetComponent<MeshRenderer>().material);
                        hit.collider.gameObject.SetActive(false);
                        holdingObject = true;
                        objectBeingHeld = hit.collider.gameObject;
                    }
                }
            }
            else
            {
                //objectBeingHeld.transform.parent = originalParent;
                objectBeingHeld.transform.position = new Vector3(transform.position.x, objectBeingHeld.transform.position.y, transform.position.z) + (new Vector3(transform.forward.x, 0f, transform.forward.z) * placeDistance);
                ResetMaterial();
                objectBeingHeld.SetActive(true);
                holdingObject = false;
                objectBeingHeld = null;
            }
        }
        previousInputState = inputState;
    }

    public void SetMaterial(Material m)
    {
        displayObjectRenderer.material = m;
    }

    public void ResetMaterial()
    {
        displayObjectRenderer.material = emptyMaterial;
    }
}
