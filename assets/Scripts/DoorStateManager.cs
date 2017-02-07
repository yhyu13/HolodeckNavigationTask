using UnityEngine;
using System.Collections;

public class DoorStateManager : MonoBehaviour {
    private DoorState[] doors;

	// Use this for initialization
	void Start () {
        doors = GetComponentsInChildren<DoorState>();
	}

    public DoorState[] GetAllDoors()
    {
        return doors;
    }

    public void setDoorState(string containingObjectName, bool isOpen)
    {
        int doorIndex = getDoorIndexByObjectName(containingObjectName);
        doors[doorIndex].setOpenState(isOpen);
    }

    public void getDoorState(string containingObjectName, bool isOpen)
    {
        int doorIndex = getDoorIndexByObjectName(containingObjectName);
        doors[doorIndex].setOpenState(isOpen);
    }

    public void setNearestDoorState(Vector3 globalPosition, bool isOpen)
    {
        int doorIndex = getNearestDoorIndex(globalPosition);
        doors[doorIndex].setOpenState(isOpen);
    }

    public void getNearestDoorState(Vector3 globalPosition, bool isOpen)
    {
        int doorIndex = getNearestDoorIndex(globalPosition);
        doors[doorIndex].setOpenState(isOpen);
    }
    public string getDoorNameFromContextBoundary(ItemsManager.Context startContext, ItemsManager.Context endContext)
    {
        if (startContext == ItemsManager.Context.Blue && endContext == ItemsManager.Context.Red)
            return "Door_BLU";
        if (startContext == ItemsManager.Context.Red&& endContext == ItemsManager.Context.Green)
            return "Door_RED";
        if (startContext == ItemsManager.Context.Green && endContext == ItemsManager.Context.Yellow)
            return "Door_GRN";
        if (startContext == ItemsManager.Context.Yellow && endContext == ItemsManager.Context.Blue)
            return "Door_YLW";
        if (startContext == ItemsManager.Context.Red && endContext == ItemsManager.Context.Blue)
            return "Door_BLU";
        if (startContext == ItemsManager.Context.Green && endContext == ItemsManager.Context.Red)
            return "Door_RED";
        if (startContext == ItemsManager.Context.Yellow && endContext == ItemsManager.Context.Green)
            return "Door_GRN";
        if (startContext == ItemsManager.Context.Blue && endContext == ItemsManager.Context.Yellow)
            return "Door_YLW";
        return "";
    }
    public int getDoorIndexFromContextBoundary(ItemsManager.Context startContext, ItemsManager.Context endContext)
    {
        return getDoorIndexByObjectName(getDoorNameFromContextBoundary(startContext, endContext));
    }
    private int getNearestDoorIndex(Vector3 globalPosition)
    {
        float minDist = float.MaxValue;
        int minDistIndex = -1;
        for (int i = 0; i < doors.Length; i++) {
            float dist = Vector3.Distance(globalPosition, doors[i].transform.position);
            if (dist <= minDist)
            {
                minDist = dist;
                minDistIndex = i;
            }
        }
        return minDistIndex;
    }

    private int getDoorIndexByObjectName(string name)
    {
        for (int i = 0; i < doors.Length; i++)
            if (doors[i].name == name)
                return i;
        return -1;
    }
}
