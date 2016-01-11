using UnityEngine;
using System.Collections;

public class ItemsManager : MonoBehaviour {
    public ChangeTextureOnCollision[] yellowObjects;
    public ChangeTextureOnCollision[] redObjects;
    public ChangeTextureOnCollision[] greenObjects;
    public ChangeTextureOnCollision[] blueObjects;

    public enum Context{
        Blue=0, Red, Green, Yellow, None
    };

    public bool haveObjectsInContextBeenClicked(Context c)
    {
        switch (c)
        {
            case Context.Yellow:
                return haveObjectsBeenClicked(yellowObjects);
            case Context.Red:
                return haveObjectsBeenClicked(redObjects);
            case Context.Green:
                return haveObjectsBeenClicked(greenObjects);
            case Context.Blue:
                return haveObjectsBeenClicked(blueObjects);
        }
        return false;
    }

    private bool haveObjectsBeenClicked(ChangeTextureOnCollision[] objs)
    {
        bool clicked = true;
        for (int i = 0; i < objs.Length; i++)
            clicked &= objs[i].hasBeenChanged;
        return clicked;
    }

    public bool haveAllObjectsBeenClicked()
    {
        return haveObjectsBeenClicked(yellowObjects) && 
            haveObjectsBeenClicked(redObjects) && 
            haveObjectsBeenClicked(greenObjects) && 
            haveObjectsBeenClicked(blueObjects);
    }
}
