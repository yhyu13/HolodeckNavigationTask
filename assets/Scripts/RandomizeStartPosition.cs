using UnityEngine;
using System.Collections;

public class RandomizeStartPosition : MonoBehaviour {
    public Vector3[] possiblePositions;
    public Vector3[] possibleDirections;
    public int[] randomOrder;
    public bool[] clockwise;
    public string playerPrefsString = "previousRandomPositionIndex";
    public DoorStateManager doorManager;
    public DoorItemPlayerStateMachine stateMachine;

	// Use this for initialization
	void Start () {
        if (!PlayerPrefs.HasKey(playerPrefsString))
            PlayerPrefs.SetInt(playerPrefsString, 0);

        int locationIndex = randomOrder[PlayerPrefs.GetInt(playerPrefsString)];
        Vector3 startPosition = possiblePositions[locationIndex];
        Vector3 startDirection = possibleDirections[locationIndex];
        this.gameObject.transform.position = startPosition;
        this.gameObject.transform.rotation = Quaternion.Euler(startDirection);
        stateMachine.directionClockwise = clockwise[locationIndex];

        int nextIndex = (locationIndex + 1) % possibleDirections.Length;
        PlayerPrefs.SetInt(playerPrefsString, nextIndex);

        
	}
}
