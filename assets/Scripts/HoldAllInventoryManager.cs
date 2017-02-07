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

    public bool randomizeStartOrder = true;

    public bool anonymousMode = false;
    private bool readyForIdentityPhase = false;
    public Material anonymousMaterial;
    private bool anonymousModeActive = false;

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
        if (randomizeStartOrder)
            ShuffleHeldObjects();
        displayObjectRenderer = displayObject.GetComponent<MeshRenderer>();
        DisplayItemAtIndex(0);
    }

    private System.Random rng = new System.Random();

    private void ShuffleHeldObjects()
    {
        int n = heldObjectIndicies.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            int value = heldObjectIndicies[k];
            heldObjectIndicies[k] = heldObjectIndicies[n];
            heldObjectIndicies[n] = value;
        }
    }

    void Update()
    {
        if (!anonymousModeActive && anonymousMode && readyForIdentityPhase && Input.GetKey(KeyCode.A))
        {
            anonymousModeActive = true;
            int numItems = ItemsObject.transform.childCount;
            objects = new GameObject[numItems];
            heldObjectIndicies = new List<int>();
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i] = ItemsObject.transform.GetChild(i).gameObject;
                heldObjectIndicies.Add(i);
            }
            if (randomizeStartOrder)
                ShuffleHeldObjects();
            currentItemIndex = 0;
            DisplayItemAtIndex(currentItemIndex);

            //Reactivate random start position
            Vector3[] posRot = gameObject.transform.parent.gameObject.GetComponent<RandomizeStartPosition>().getNextPositionRotation();
            this.transform.localPosition = new Vector3(posRot[0].x, 1.05f, posRot[0].z);
            this.transform.localRotation = Quaternion.Euler(posRot[1]);
            this.transform.parent.GetChild(1).transform.localRotation = Quaternion.Euler(posRot[1]);
        }
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
                    if (anonymousModeActive)
                    {
                        //Get the object being replaced's information
                        GameObject pickupObject = hit.collider.gameObject;
                        //If the item has already been identified
                        if (pickupObject.GetComponent<MeshRenderer>().material.mainTexture != anonymousMaterial.mainTexture)
                        {
                            //Add it to the inventory system and display
                            for (int i = 0; i < objects.Length; i++)
                                if (pickupObject == objects[i])
                                {
                                    heldObjectIndicies.Add(i);
                                    break;
                                }
                            currentItemIndex = heldObjectIndicies.Count - 1;
                            DisplayItemAtIndex(currentItemIndex);
                            SetTexture(pickupObject, anonymousMaterial.mainTexture);
                            logger.pushEventLogToSummary("Object_Identity_Removed," + pickupObject.name + " : " + pickupObject.transform.position.ToString(), true);
                        }
                        //If there are items in the inventory and we're in a good state - and the item being replaced has a dot texture
                        else if (currentItemIndex != -1 && heldObjectIndicies.Count > 0)
                        {
                            //Get the objects to be swapped
                            
                            currentItemIndex = (currentItemIndex) % heldObjectIndicies.Count;
                            GameObject placeObject = objects[heldObjectIndicies[currentItemIndex]];
                            heldObjectIndicies.RemoveAt(currentItemIndex);

                            //Swap their positions
                            Vector3 pickupObjectOldPosition = pickupObject.transform.position;
                            pickupObject.transform.position = placeObject.transform.position;
                            placeObject.transform.position = pickupObjectOldPosition;

                            SetTexture(placeObject, placeObject.GetComponent<ChangeTextureOnCollision>().startTexture);

                            DisplayItemAtIndex(currentItemIndex);
                            logger.pushEventLogToSummary("Object_Identity_Set," + placeObject.name + " : " + placeObject.transform.position.ToString(), true);
                        }
                    }
                    else
                    {
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
            }
            else
            {
                //DROP CURRENT ITEM IN INVENTORY
                
                if (heldObjectIndicies.Count > 0 && !anonymousModeActive)
                {
                    //If there are items left
                    //Get object and remove its index from the held list
                    currentItemIndex = (currentItemIndex) % heldObjectIndicies.Count;
                    GameObject currentObject = objects[heldObjectIndicies[currentItemIndex]];
                    heldObjectIndicies.RemoveAt(currentItemIndex);
                    currentObject.transform.position = new Vector3(transform.position.x, currentObject.transform.position.y, transform.position.z) + (new Vector3(transform.forward.x, 0f, transform.forward.z) * placeDistance);
                    if (anonymousMode)
                        currentObject.GetComponent<MeshRenderer>().material = anonymousMaterial;
                    currentObject.SetActive(true);

                    logger.pushEventLogToSummary("Object_Placed," + currentObject.name + " : " + currentObject.transform.position.ToString(), true);
                }
                else if (!anonymousModeActive)
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
        {
            if (anonymousMode)
                readyForIdentityPhase = true;
            ResetMaterial();
        }
        else
        {
            if (anonymousMode && !anonymousModeActive)
            {
                readyForIdentityPhase = false;
                SetMaterial(anonymousMaterial);
            }
            else
            {
                GameObject displayObject = objects[heldObjectIndicies[index % heldObjectIndicies.Count]];
                SetMaterial(displayObject.GetComponent<ChangeTextureOnCollision>().startTexture);
            }
        }
    }

    public void SetTexture(GameObject o, Texture t)
    {
        Material m = new Material(o.GetComponent<MeshRenderer>().material);
        m.mainTexture = t;
        o.GetComponent<MeshRenderer>().material = m;
    }

    public void SetMaterial(Texture t)
    {
        Material m = new Material(displayObjectRenderer.material);
        m.mainTexture = t;
        displayObjectRenderer.material = m;
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
