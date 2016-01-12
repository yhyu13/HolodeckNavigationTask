using UnityEngine;
using System.Collections;

public class ChangeTextureOnCollision : MonoBehaviour {

    public MeshRenderer rendererToChange;
    public Collider colliderToMonitor;
    public Texture startTexture;
    public Texture newTexture;
    public bool hasBeenChanged = false;
    public Logger logger;
    private bool changeLatch = false;
    public bool disableColliderOnChange = true;
    public bool deactivateAfterStart = false;
	// Use this for initialization
	void Start () {
	    if(rendererToChange != null)
            rendererToChange.material.mainTexture = startTexture;
        if (deactivateAfterStart)
            this.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (changeLatch && !hasBeenChanged)
        {
            rendererToChange.material.mainTexture = newTexture;
            hasBeenChanged = true;
            if (disableColliderOnChange)
                gameObject.GetComponent<BoxCollider>().enabled = false;
            if (logger != null)
            {
                logger.pushEventLogToSummary("ChangeTextureEvent_ObjectClicked," + name, true);
                logger.pushEventLogToRaw("ChangeTextureEvent_ObjectClicked," + name, true);
            }
            changeLatch = false;
        }
	}

    public void Change()
    {
        changeLatch = true;
    }
}
