using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.VR;
using UnityEngine.SceneManagement;

public class SubmitPressed : MonoBehaviour {
    public int newSceneNumber;
    public string sliderPlayerPrefsValue = "taskTime";
    public string subjectIDPlayerPrefsValue = "subjectID";
    public Slider slider;
    public InputField subjectID;
    public Toggle flatToggle;

    void Start()
    {
        Button b = gameObject.GetComponent<Button>();
        if(b!=null)
            b.onClick.AddListener(delegate() { OnClick(); });
    }

    void OnClick()
    {
        Debug.Log("Submitting and Changing Scenes...");
        if (slider != null)
            PlayerPrefs.SetFloat(sliderPlayerPrefsValue, slider.value);
        if (subjectID != null)
            if (subjectID.text.Trim() != "")
            {
                PlayerPrefs.SetString(subjectIDPlayerPrefsValue, subjectID.text.Trim());
                VRSettings.enabled = true;
                Debug.Log("Loading scene " + newSceneNumber);
                SceneManager.LoadScene(newSceneNumber);
            }
    }
}
