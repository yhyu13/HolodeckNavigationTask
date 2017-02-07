using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

public class FormValidator : MonoBehaviour {

    public string subjectIDPlayerPrefsValue = "subjectID";

    public InputField subIDText;
    public Dropdown trialDropDown;
    public Button practiceButton;
    public Button studyButton;
    public Button testButton;
    public Dropdown mode;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        foreach (Camera c in Camera.allCameras)
            c.ResetAspect();
    }

	// Update is called once per frame
	void Update () {
	    if(subIDText.text.Length == 3 && trialDropDown.value != 0)
        {
            practiceButton.interactable = true;
            studyButton.interactable = true;
            testButton.interactable = true;
        }
        else
        {
            practiceButton.interactable = false;
            studyButton.interactable = false;
            testButton.interactable = false;
        }
	}

    private void SetPlayerPrefValues(int phase)
    {
        PlayerPrefs.SetString("sub", subIDText.text.Trim());
        PlayerPrefs.SetString(subjectIDPlayerPrefsValue, subIDText.text.Trim());
        PlayerPrefs.SetInt("trial", trialDropDown.value);
        PlayerPrefs.SetInt("phase", phase);
    }

    public void Begin(int phaseNum, int sceneNum, bool vrEnabled, string configFile)
    {
        VRSettings.enabled = vrEnabled;
        PlayerPrefs.SetString(ConfigurationFileLoader.configFilePlayerPrefsString, configFile);
        SetPlayerPrefValues(phaseNum);
        ConfigurationFileLoader.Init();
        SceneManager.LoadScene(sceneNum);
    }

    public void BeginPractice()
    {
        switch (mode.value)
        {
            case 0:
                Begin(0, 1, true, "SimulationConfigurationvr.config");
                break;
            case 1:
                Begin(3, 1, false, "SimulationConfiguration.config");
                break;
            case 2:
                Begin(6, 4, true, "SimulationConfigurationvr.config");
                break;
            case 3:
                Begin(9, 4, false, "SimulationConfiguration.config");
                break;
        }
    }

    public void BeginStudy()
    {
        switch (mode.value)
        {
            case 0:
                Begin(1, 2, true, "SimulationConfigurationvr.config");
                break;
            case 1:
                Begin(4, 2, false, "SimulationConfiguration.config");
                break;
            case 2:
                Begin(7, 5, true, "SimulationConfigurationvr.config");
                break;
            case 3:
                Begin(10, 5, false, "SimulationConfiguration.config");
                break;
        }
    }

    public void BeginTest()
    {
        switch (mode.value)
        {
            case 0:
                Begin(2, 3, true, "SimulationConfigurationvr.config");
                break;
            case 1:
                Begin(5, 3, false, "SimulationConfiguration.config");
                break;
            case 2:
                Begin(8, 6, true, "SimulationConfigurationvr.config");
                break;
            case 3:
                Begin(11, 6, false, "SimulationConfiguration.config");
                break;
        }
    }
}
