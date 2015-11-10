using UnityEngine;
using System.Collections;

namespace qtools.qmaze.example
{
	[RequireComponent(typeof(Camera))]
	public class QTPSCamera : MonoBehaviour 
	{
		public Transform targetTransform;
		public Vector3 offset = new Vector3(1.0f, 5.0f, 1.0f);
		public float lerp = 0.4f;

		void Update ()
		{
			transform.position = Vector3.Lerp(transform.position, targetTransform.position + offset, lerp * Time.deltaTime);
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetTransform.position - transform.position), lerp * Time.deltaTime);
		}
	}
}