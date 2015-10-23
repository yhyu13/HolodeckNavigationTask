using UnityEngine;
using System.Collections;

public class Raycast : MonoBehaviour 
{
	// Update is called once per frame
	void Update() 
	{
        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire1")) 
		{
			RaycastHit hit;
			Ray ray = new Ray(transform.position, transform.forward);
			if (Physics.Raycast(ray, out hit, 2f))
				if (hit.collider != null)
					hit.collider.enabled = false;
					
					//Destroy(hit.transform.gameObject);
		}
		Debug.DrawRay (transform.position, transform.forward * 2f,Color.green);
	}
}