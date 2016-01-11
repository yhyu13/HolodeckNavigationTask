using UnityEngine;
using XInputDotNetPure; // Required in C#

public class XInputTestCS : MonoBehaviour
{
    bool playerIndexSet = false;
    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;
    public GameObject renderObj;
    public Camera lookCamera;
    public float prevTriggerStateRight = 0f;
    public float prevTriggerStateLeft = 0f;
<<<<<<< HEAD
    public static string buttonString = "a";
    public bool previousInputState = false;
=======

    public bool mouseClickState = false;
    public bool prevMouseClickState = false;

>>>>>>> origin/master
    // Use this for initialization
    void Start()
    {
        lookCamera.transform.localRotation = Quaternion.identity;
        UnityEngine.VR.InputTracking.Recenter();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Find a PlayerIndex, for a single player game
        // Will find the first controller that is connected ans use it
        if (!playerIndexSet || !prevState.IsConnected)
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

        prevState = state;
        state = GamePad.GetState(playerIndex);
        prevMouseClickState = mouseClickState;
        mouseClickState = Input.GetMouseButton(0);

        bool inputState = Input.GetKey(KeyCode.X) || Input.GetMouseButton(0) || Input.GetButton(buttonString);
        bool risingEdge = inputState && !previousInputState;
        // Detect if a button was pressed this frame
<<<<<<< HEAD
        if ((prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed) || risingEdge)
=======
        if ((prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed) || (!prevMouseClickState && mouseClickState)) 
>>>>>>> origin/master
        {
            renderObj.GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value, 1.0f);
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
            if (Physics.Raycast(ray, out hit, 2f))
                hit.collider.gameObject.GetComponent<ChangeTextureOnCollision>().Change();
        }
        // Detect if a button was released this frame
        if (prevState.Buttons.A == ButtonState.Pressed && state.Buttons.A == ButtonState.Released)
        {
            renderObj.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }

        // Set vibration according to triggers
        GamePad.SetVibration(playerIndex, state.Triggers.Left, state.Triggers.Right);
        float trigStateRight = state.Triggers.Right;
        float trigStateLeft = state.Triggers.Left;
        bool changeOnRisingEdge = true;
        if ((changeOnRisingEdge && trigStateRight > 0 && prevTriggerStateRight == 0) || (!changeOnRisingEdge && trigStateRight > 0))
        {
            // Make the current object turn
            //transform.localRotation *= Quaternion.Euler(0.0f, state.ThumbSticks.Right.X * 150.0f * Time.deltaTime, 0.0f);
            float rotationY = UnityEngine.VR.InputTracking.GetLocalRotation(UnityEngine.VR.VRNode.CenterEye).eulerAngles.y;
            if (!changeOnRisingEdge)
            {
                if (rotationY < 180)
                    rotationY /= 2;
                else
                    rotationY = 360 - ((360 - rotationY) / 2);
            }
            transform.localRotation *= Quaternion.Euler(0.0f, rotationY, 0.0f);
            UnityEngine.VR.InputTracking.Recenter();
            
        }
        if (trigStateLeft > 0 && prevTriggerStateLeft == 0)
        {
            lookCamera.transform.localRotation = Quaternion.identity;
            UnityEngine.VR.InputTracking.Recenter();
        }
        prevTriggerStateLeft = trigStateLeft;
        prevTriggerStateRight = trigStateRight;
    }
}
