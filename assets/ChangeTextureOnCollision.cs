using UnityEngine;
using System.Collections;

public class ChangeTextureOnCollision : MonoBehaviour {

    public MeshRenderer rendererToChange;
    public Collider colliderToMonitor;
    public Texture startTexture;
    public Texture newTexture;
    public bool hasBeenChanged = false;
    public Logger logger;

	// Use this for initialization
	void Start () {
	    if(rendererToChange != null)
            rendererToChange.material.mainTexture = startTexture;
	}
	
	// Update is called once per frame
	void Update () {
        if (!colliderToMonitor.enabled && !hasBeenChanged)
        {
            rendererToChange.material.mainTexture = newTexture;
            hasBeenChanged = true;
            if (logger != null)
            {
                logger.pushEventLogToSummary("ChangeTextureEvent_ObjectClicked," + name, true);
                logger.pushEventLogToRaw("ChangeTextureEvent_ObjectClicked," + name, true);
            }
        }
	}
}
