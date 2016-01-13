using UnityEngine;
using System.Collections;

public class CullLayerByDistance : MonoBehaviour {

    public int layer = 0;
    public int dist = 1000;
    public bool cullSpherically = true;
    private Camera cam;
    private float[] distances;

	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
        cam.layerCullSpherical = cullSpherically;
        distances = new float[32];
	}
	
	// Update is called once per frame
	void Update () {
        cam.layerCullSpherical = cullSpherically;
        distances[layer] = dist;
        cam.layerCullDistances = distances;
	}
}
