using UnityEngine;
using System.Collections;

public class RandomizeStartPosition : MonoBehaviour {
    public Vector3[] possiblePositions;
    public Vector3[] possibleDirections;
    public int[] randomOrder;
    public string playerPrefsString = "previousRandomPositionIndex";
    public GameObject[] doors;
	// Use this for initialization
	void Start () {
        if (!PlayerPrefs.HasKey(playerPrefsString))
            PlayerPrefs.SetInt(playerPrefsString, 0);

        int locationIndex = randomOrder[PlayerPrefs.GetInt(playerPrefsString)];
        Vector3 startPosition = possiblePositions[locationIndex];
        Vector3 startDirection = possibleDirections[locationIndex];
        this.gameObject.transform.position = startPosition;
        this.gameObject.transform.rotation = Quaternion.Euler(startDirection);

        int nextIndex = (locationIndex + 1) % possibleDirections.Length;
        PlayerPrefs.SetInt(playerPrefsString, nextIndex);

        float minDist = float.MaxValue;
        int minIndex = -1;
        for (int i = 0; i < doors.Length; i++)
        {
            float dist = Vector3.Distance(startPosition, doors[i].transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                minIndex = i;
            }
            Debug.Log(doors[i].name + " : " + dist);
        }
        Debug.Log(doors[minIndex].name);
        doors[minIndex].GetComponent<iTweenEvent>().Play();
	}
}
