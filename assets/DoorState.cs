using UnityEngine;
using System.Collections;

public class DoorState : MonoBehaviour {
    public bool Open = true;
    public Vector3 openStateVector;
    public Vector3 closedStateVector;
    private bool previous;

    void Start()
    {
        previous = Open;
        setOpenStateInstant(Open);
    }

	void Update () {
        if (Open!=previous)
        {
            setOpenState(Open);
        }
        previous = Open;
	}

    public void setOpenState(bool isOpen)
    {
        Open = isOpen;
        if (Open)
            iTween.MoveTo(gameObject, iTween.Hash("position", openStateVector, "time", 2f, "islocal", true));
        else
            iTween.MoveTo(gameObject, iTween.Hash("position", closedStateVector, "time", 2f, "islocal", true));
    }

    private void setOpenStateInstant(bool isOpen)
    {
        Open = isOpen;
        if (Open)
            iTween.MoveTo(gameObject, iTween.Hash("position", openStateVector, "time", 0f, "islocal", true));
        else
            iTween.MoveTo(gameObject, iTween.Hash("position", closedStateVector, "time", 0f, "islocal", true));
    }
}
