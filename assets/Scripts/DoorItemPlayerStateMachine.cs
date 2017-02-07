using UnityEngine;
using System.Collections;

public class DoorItemPlayerStateMachine : MonoBehaviour {
    public DoorStateManager doorManager;
    public ItemsManager itemsManager;
    public Rect[] contextRectangles;

    public string[] doorOrder;
    private ItemsManager.Context startContext;
    private ItemsManager.Context currentContext;
    private ItemsManager.Context nextContext;
    public bool directionClockwise;
    public enum State
    {
        Start,
        WaitingForItemClicks,
        WaitingForNextContext,
        Done
    };
    private State state;
	// Use this for initialization
	void Start () {
        state = State.Start;
	}

    private ItemsManager.Context getNextContext()
    {
        int nextContext = -1;
        if (directionClockwise) nextContext = ((int)currentContext + 1) % 4;
        else nextContext = ((int)currentContext - 1 + 4) % 4;
        return (ItemsManager.Context) nextContext;
    }
	// Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Start:
                startContext = getContextFromPosition(transform.position);
                currentContext = startContext;
                nextContext = getNextContext();
                Debug.Log("Start Context: " + startContext);
                Debug.Log("Next Context: " + nextContext);
                state = State.WaitingForItemClicks;
                break;
            case State.WaitingForItemClicks:
                if (itemsManager.haveAllObjectsBeenClicked())
                    state = State.Done;
                else if (itemsManager.haveObjectsInContextBeenClicked(currentContext))
                {
                    //Open the correct door
                    doorManager.setDoorState(doorOrder[doorManager.getDoorIndexFromContextBoundary(currentContext, nextContext)], true);
                    state = State.WaitingForNextContext;
                }
                break;
            case State.WaitingForNextContext:
                ItemsManager.Context positionContext = getContextFromPosition(transform.position);
                if (positionContext == nextContext)
                {
                    doorManager.setDoorState(doorOrder[doorManager.getDoorIndexFromContextBoundary(currentContext, nextContext)], false);
                    currentContext = nextContext;
                    nextContext = getNextContext();
                    Debug.Log("Start Context: " + currentContext);
                    Debug.Log("Next Context: " + nextContext);
                    state = State.WaitingForItemClicks;
                }
                break;
            case State.Done:
                DoorState[] doorStates = doorManager.GetAllDoors();
                foreach (DoorState d in doorStates)
                    d.gameObject.GetComponent<MeshRenderer>().material.color = Color.black;
                break;
        }
    }

    private ItemsManager.Context getContextFromPosition(Vector3 position)
    {
        Debug.Log(position);
        for (int i = 0; i < contextRectangles.Length; i++)
            if (contextRectangles[i].Contains(new Vector2(position.x, position.z)))
                return (ItemsManager.Context)i;
        return ItemsManager.Context.None;
    }
}
