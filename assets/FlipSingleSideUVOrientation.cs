using UnityEngine;
using System.Collections;

public class FlipSingleSideUVOrientation : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //Get the mesh filter for this cube
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = null;
        if (mf != null)
            mesh = mf.mesh;

        if (mesh == null || mesh.uv.Length != 24)
        {
            Debug.Log("Script needs to be attached to built-in cube");
            return;
        }

        //Get the current UVs (probably all 0,0;1,0;0,1;1,1)
        Vector2[] uvs = mesh.uv;

        // Back side UV flipped
        uvs[10] = new Vector2(0.0f, 0.0f);
        uvs[11] = new Vector2(-1f, 0.0f);
        uvs[6] = new Vector2(0.0f, -1f);
        uvs[7] = new Vector2(-1f, -1f);
        
        // Set the output UV once and it will be fixed for the rest of the object lifetime
        mesh.uv = uvs;
    }
}
