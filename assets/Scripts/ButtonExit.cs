using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonExit : MonoBehaviour {

    public KeyCode button;
    private bool prevState;
    public int escapeScene = 0;
    public enum CloseMode { Quit = 0, EscapeToScene = 1 };
    public CloseMode closeMode = CloseMode.EscapeToScene;
	// Use this for initialization
	void Start () {
        prevState = false;
    }
	
	// Update is called once per frame
	void Update () {
        bool state = Input.GetKey(button);
        if (!state && prevState)
        {
            if (closeMode == CloseMode.EscapeToScene)
            {
                try { this.gameObject.GetComponent<Logger>().Finish(); } catch (System.Exception) { };
                SceneManager.LoadScene(escapeScene);
            }
            else
            {
                Application.Quit();
            }
        }
        prevState = state;
	}
}
