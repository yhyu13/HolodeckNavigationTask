using UnityEngine;

public class CrosshairCS : MonoBehaviour {
    private Texture2D CrosshairTexture;
    public Texture2D NormalTexture;
    public Texture2D SelectTexture;
    public bool ShowCrosshair = true;
    private bool Select = false;
    void Start()
    {
        Screen.lockCursor = true;
        CrosshairTexture = NormalTexture;
    }

    void OnGUI()
    {
        if (ShowCrosshair == true)
        {
            GUI.Label(new Rect((Screen.width - CrosshairTexture.width) / 2, (Screen.height -
                            CrosshairTexture.height) / 2, CrosshairTexture.width, CrosshairTexture.height), CrosshairTexture);
        }
        
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        SetSelectState(false);
        if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f)), out hit, 2f))
            if (hit.collider.gameObject.GetComponent<Grabbable>() != null)
                SetSelectState(true);
                
    }

    public void SetSelectState(bool select)
    {
        Select = select;
        if (Select)
            CrosshairTexture = SelectTexture;
        else
            CrosshairTexture = NormalTexture;
    }
}
