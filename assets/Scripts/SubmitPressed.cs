using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.VR;

public class SubmitPressed : MonoBehaviour {
    public int newSceneNumberFlat = 1;
    public int newSceneNumberNotFlat = 2;
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
            PlayerPrefs.SetString(subjectIDPlayerPrefsValue, subjectID.text);
        VRSettings.enabled = true;
        if(flatToggle.isOn)
            Application.LoadLevel(newSceneNumberFlat);
        else
            Application.LoadLevel(newSceneNumberNotFlat);
    }
}
