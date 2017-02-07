using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoaderStateSaver : MonoBehaviour {

    public bool saveSubjectID = true;
    public bool saveTrialNum = false;
    public bool saveMode = true;

    public InputField subIDInputField;
    public Dropdown trialDropDown;
    public Dropdown modeDropdown;

    public string subIDStateString = "LoaderSubID";
    public string trialNumStateString = "LoaderTrialNum";
    public string modeStateString = "LoaderMode";

    private string prevSubIDVal;
    private int prevTrialNumVal;
    private int prevModeVal;

    // Use this for initialization
    void Start () {
        if (saveSubjectID)
        {
            if (PlayerPrefs.HasKey(subIDStateString))
                subIDInputField.text = PlayerPrefs.GetString(subIDStateString);
            else
                PlayerPrefs.SetString(subIDStateString, subIDInputField.text);
            prevSubIDVal = subIDInputField.text;
        }
        if (saveTrialNum)
        {
            if (PlayerPrefs.HasKey(trialNumStateString))
                trialDropDown.value = PlayerPrefs.GetInt(trialNumStateString);
            else
                PlayerPrefs.SetInt(trialNumStateString, trialDropDown.value);
            prevTrialNumVal = trialDropDown.value;
        }
        if (saveMode)
        {
            if (PlayerPrefs.HasKey(modeStateString))
                modeDropdown.value = PlayerPrefs.GetInt(modeStateString);
            else
                PlayerPrefs.SetInt(modeStateString, modeDropdown.value);
            prevModeVal = modeDropdown.value;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (saveSubjectID && subIDInputField.text != prevSubIDVal)
        {
            PlayerPrefs.SetString(subIDStateString, subIDInputField.text);
            prevSubIDVal = subIDInputField.text;
        }
        if (saveTrialNum && trialDropDown.value != prevTrialNumVal)
        {
            PlayerPrefs.SetInt(trialNumStateString, trialDropDown.value);
            prevTrialNumVal = trialDropDown.value;
        }
        if (saveMode && modeDropdown.value != prevModeVal)
        {
            PlayerPrefs.SetInt(modeStateString, modeDropdown.value);
            prevModeVal = modeDropdown.value;
        }
    }
}
