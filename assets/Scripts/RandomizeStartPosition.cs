using UnityEngine;
using System.Collections;
using System;

public class RandomizeStartPosition : MonoBehaviour {
    public Vector3[] possiblePositions;
    public Vector3[] possibleDirections;
    public int[] randomOrder;
    public bool[] clockwise;
    public string playerPrefsString = "previousRandomTestPositionIndex";
    public DoorStateManager doorManager;
    public DoorItemPlayerStateMachine stateMachine;

	// Use this for initialization
	public void Start () {
        if (!PlayerPrefs.HasKey(playerPrefsString))
            PlayerPrefs.SetInt(playerPrefsString, 0);
        if(PlayerPrefs.GetInt(playerPrefsString) >= randomOrder.Length)
            PlayerPrefs.SetInt(playerPrefsString, 0);
        int locationIndex = randomOrder[PlayerPrefs.GetInt(playerPrefsString)];
        Vector3 startPosition = possiblePositions[locationIndex];
        Vector3 startDirection = possibleDirections[locationIndex];
        this.gameObject.transform.position = startPosition;
        this.gameObject.transform.rotation = Quaternion.Euler(startDirection);
        

        int nextIndex = (locationIndex + 1) % possibleDirections.Length;
        PlayerPrefs.SetInt(playerPrefsString, nextIndex);

        try { stateMachine.directionClockwise = clockwise[locationIndex]; }
        catch (Exception) { }
	}

    public Vector3[] getNextPositionRotation()
    {
        if (!PlayerPrefs.HasKey(playerPrefsString))
            PlayerPrefs.SetInt(playerPrefsString, 0);
        if (PlayerPrefs.GetInt(playerPrefsString) >= randomOrder.Length)
            PlayerPrefs.SetInt(playerPrefsString, 0);
        int locationIndex = randomOrder[PlayerPrefs.GetInt(playerPrefsString)];
        Vector3 startPosition = possiblePositions[locationIndex];
        Vector3 startDirection = possibleDirections[locationIndex];

        int nextIndex = (locationIndex + 1) % possibleDirections.Length;
        PlayerPrefs.SetInt(playerPrefsString, nextIndex);

        try { stateMachine.directionClockwise = clockwise[locationIndex]; }
        catch (Exception) { }

        return new Vector3[] { startPosition, startDirection };
    }
}
