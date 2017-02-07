using UnityEngine;
using XInputDotNetPure; // Required in C#

public class XInputTestCS : MonoBehaviour
{
    bool playerIndexSet = false;
    PlayerIndex playerIndex;

    GamePadState gamePadState;
    GamePadState prevGamePadState;

    public GameObject pointerRenderObject;
    public Camera lookCamera;

    private float ClickDistance = 2f;
    private float DefaultLookAngle = 0f;

    private float HeadHeight = 0.9f;

    public static string selectButtonString = "a";
    public static int selectMouseButtonInt = 0;
    public static KeyCode selectKeyboardButtonKeyCode = KeyCode.X;
    public static string rotationButtonKeyString = "r";

    public float prevTriggerStateRight = 0f;
    public float prevTriggerStateLeft = 0f;
    private bool prevRotationButtonState = false;
    public bool previousInputState = false;
    public bool prevMouseClickState = false;

    // Use this for initialization
    void Start()
    {
        lookCamera.transform.localRotation = Quaternion.identity;
        UnityEngine.VR.InputTracking.Recenter();
        ClickDistance = PlayerPrefs.GetFloat("ClickDistance", 2f);
        DefaultLookAngle = PlayerPrefs.GetFloat("DefaultLookAngle", 0f);

        Debug.Log("Setting click distance to " + ClickDistance);
    }

    private bool first = true;
    // Update is called once per frame
    void Update()
    {
        if (first)
        {
            Debug.Log("Setting rotation to " + DefaultLookAngle);
            lookCamera.transform.parent.position = new Vector3(transform.position.x, transform.position.y + HeadHeight, transform.position.z);
            lookCamera.transform.parent.rotation = transform.rotation;
            lookCamera.transform.Rotate(DefaultLookAngle, 0f, 0f);
            first = false;
        }
        // Find a PlayerIndex, for a single player game
        // Will find the first controller that is connected ans use it
        if (!playerIndexSet || !prevGamePadState.IsConnected)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                    playerIndex = testPlayerIndex;
                    playerIndexSet = true;
                }
            }
        }

        gamePadState = GamePad.GetState(playerIndex);
        bool mouseClickState = Input.GetMouseButton(selectMouseButtonInt);

        bool inputState = Input.GetKey(selectKeyboardButtonKeyCode) || mouseClickState || Input.GetButton(selectButtonString);
        bool risingEdge = inputState && !previousInputState;
        bool fallingEdge = !inputState && previousInputState;
        // Detect if a button was pressed this frame
        if ((prevGamePadState.Buttons.A == ButtonState.Released && gamePadState.Buttons.A == ButtonState.Pressed) || risingEdge)
        {
            pointerRenderObject.GetComponent<Renderer>().material.color = Color.black;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
            if (Physics.Raycast(ray, out hit, ClickDistance))
                try
                {
                    hit.collider.gameObject.GetComponent<ChangeTextureOnCollision>().Change();
                }
                catch (System.Exception) { }
        }
        // Detect if a button was released this frame
        if (prevGamePadState.Buttons.A == ButtonState.Pressed && gamePadState.Buttons.A == ButtonState.Released || fallingEdge)
            pointerRenderObject.GetComponent<Renderer>().material.color = Color.white;

        //TODO: Fix the below to remove bounce and stutter issue
        // Set vibration according to triggers
        //GamePad.SetVibration(playerIndex, gamePadState.Triggers.Left, gamePadState.Triggers.Right);
        float trigStateRight = gamePadState.Triggers.Right;
        float trigStateLeft = gamePadState.Triggers.Left;
        bool rotationButtonState = Input.GetKey(rotationButtonKeyString);
        bool changeOnRisingEdge = true;
        if ((changeOnRisingEdge && trigStateRight > 0 && prevTriggerStateRight == 0) || (!changeOnRisingEdge && trigStateRight > 0) || (changeOnRisingEdge && rotationButtonState && !prevRotationButtonState))
        {
            float rotationY = UnityEngine.VR.InputTracking.GetLocalRotation(UnityEngine.VR.VRNode.CenterEye).eulerAngles.y;
            transform.rotation = Quaternion.Euler(new Vector3(0f, lookCamera.transform.rotation.eulerAngles.y, 0f));   
        }

        lookCamera.transform.parent.position = new Vector3(transform.position.x, transform.position.y + HeadHeight, transform.position.z);

        prevTriggerStateLeft = trigStateLeft;
        prevTriggerStateRight = trigStateRight;
        prevRotationButtonState = rotationButtonState;
        prevMouseClickState = mouseClickState;
        prevGamePadState = gamePadState;
        previousInputState = inputState;
    }
}
