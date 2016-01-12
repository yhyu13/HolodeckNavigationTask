using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;

public class NavigationTestStateMachine : MonoBehaviour
{
    public GameObject character;
    public GameObject itemsRootObject;
    public float radius = 5f;

	// Use this for initialization
	void Start () {
        SetPositionsCenter();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Q))
            SetPositionsCenter();
    }

    void SetPositionsCenter()
    {
        Vector3 center = new Vector3(27.5f, 6.5f, 37.5f);
        //character.transform.position = center;

        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in itemsRootObject.transform)
            children.Add(child.gameObject);

        Shuffle(children);

        for (int i = 0; i < children.Count; i++)
        {
            float angle = ((float)i / children.Count) * Mathf.PI * 2;
            children[i].transform.position = new Vector3(center.x + radius * Mathf.Sin(angle), center.y, center.z + radius * Mathf.Cos(angle));
        }

        transform.localRotation *= Quaternion.Euler(0.0f, UnityEngine.VR.InputTracking.GetLocalRotation(UnityEngine.VR.VRNode.CenterEye).eulerAngles.y, 0.0f);
        UnityEngine.VR.InputTracking.Recenter();
    }

    
    IList<GameObject> Shuffle(IList<GameObject> list)
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = list.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            GameObject value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }
}
