using UnityEngine;
using System.Collections;

namespace qtools.qmaze.example
{
	public class QFPSController : MonoBehaviour
	{		
		public float mouseSensitivityX = 1.0f;
		public float moveScaleX = 1.0f;
		public float moveScaleY = 1.0f;
		public float moveMaxSpeed = 1.0f;
		public float moveLerp = 0.9f;

		private Rigidbody rigidBody;
		private Quaternion rotationTarget;

		void Start () 
		{
			rotationTarget = transform.rotation;
			rigidBody = GetComponent<Rigidbody>();
		}

		void Update () 
		{
			Vector3 velocity = (transform.right   * Input.GetAxis("Horizontal") * moveScaleX);
					velocity+= (transform.forward * Input.GetAxis("Vertical")   * moveScaleY);
			velocity = Vector3.ClampMagnitude(velocity, moveMaxSpeed);
			velocity *= moveLerp;
			velocity.y = rigidBody.velocity.y;
			rigidBody.velocity = velocity;

			rotationTarget *= Quaternion.Euler (0, Input.GetAxis("Mouse X") * mouseSensitivityX, 0f);
			transform.localRotation = rotationTarget;		
		}
	}
}