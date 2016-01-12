using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HoldAllInventoryManager : MonoBehaviour
{
    public GameObject ItemsObject;
    public GameObject displayObject;
    private MeshRenderer displayObjectRenderer;
    public Material emptyMaterial;
    public static KeyCode grabKeyCode = KeyCode.X;
    public static string buttonString = "a";

    public static KeyCode advanceInventoryCode = KeyCode.I;
    public static string advanceInventoryString = "i";

    public int currentItemIndex = 0;

    public bool previousInputState = false;
    private bool previousAdvanceInputState = false;
    private GameObject[] objects;
    private List<int> heldObjectIndicies;

    public float placeDistance = 1f;

    private bool onFirstUpdate = true;

    public Logger logger;

    // Use this for initialization
    void Start()
    {
        int numItems = ItemsObject.transform.childCount;
        objects = new GameObject[numItems];
        heldObjectIndicies = new List<int>();
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i] = ItemsObject.transform.GetChild(i).gameObject;
            heldObjectIndicies.Add(i);
        }
        displayObjectRenderer = displayObject.GetComponent<MeshRenderer>();
        DisplayItemAtIndex(0);
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        bool inputState = Input.GetKey(KeyCode.X) || Input.GetMouseButton(0) || Input.GetButton(buttonString);
        bool risingEdge = inputState && !previousInputState;
        if (risingEdge)
        {
            if (Physics.Raycast(ray, out hit, 2f))
            {
                if (hit.collider.gameObject.GetComponent<Grabbable>() != null)
                {
                    //PICK UP ITEM AND ADD TO END OF INVENTORY

                    //Make the object invisible
                    GameObject pickupObject = hit.collider.gameObject;
                    hit.collider.gameObject.SetActive(false);

                    //Add it to the inventory system and display
                    for (int i = 0; i < objects.Length; i++)
                        if (pickupObject == objects[i])
                        {
                            heldObjectIndicies.Add(i);
                            break;
                        }
                    currentItemIndex = heldObjectIndicies.Count - 1;
                    DisplayItemAtIndex(currentItemIndex);

                    logger.pushEventLogToSummary("Object_Picked_Up," + pickupObject.name + " : " + pickupObject.transform.position.ToString(), true);
                }
            }
            else
            {
                //DROP CURRENT ITEM IN INVENTORY
                
                if (heldObjectIndicies.Count > 0)
                {
                    //If there are items left
                    //Get object and remove its index from the held list
                    currentItemIndex = (currentItemIndex) % heldObjectIndicies.Count;
                    GameObject currentObject = objects[heldObjectIndicies[currentItemIndex]];
                    heldObjectIndicies.RemoveAt(currentItemIndex);
                    currentObject.transform.position = new Vector3(transform.position.x, currentObject.transform.position.y, transform.position.z) + (new Vector3(transform.forward.x, 0f, transform.forward.z) * placeDistance);
                    currentObject.SetActive(true);

                    logger.pushEventLogToSummary("Object_Placed," + currentObject.name + " : " + currentObject.transform.position.ToString(), true);
                }
                else
                    currentItemIndex = -1;

                DisplayItemAtIndex(currentItemIndex);
            }
        }

        bool advanceInputState = Input.GetKey(advanceInventoryCode) || Input.GetMouseButton(1) || Input.GetButton(advanceInventoryString);
        bool advanceRisingEdge = advanceInputState && !previousAdvanceInputState;
        if (advanceRisingEdge)
        {
            //GO TO NEXT ITEM IN INVENTORY
            currentItemIndex = (currentItemIndex + 1) % heldObjectIndicies.Count;
            DisplayItemAtIndex(currentItemIndex);
        }
        previousAdvanceInputState = advanceInputState;
        previousInputState = inputState;
    }

    private void DisplayItemAtIndex(int index)
    {
        if (heldObjectIndicies.Count == 0)
            ResetMaterial();
        else
        {
            GameObject displayObject = objects[heldObjectIndicies[index % heldObjectIndicies.Count]];
            SetMaterial(displayObject.GetComponent<MeshRenderer>().material);
        }
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
