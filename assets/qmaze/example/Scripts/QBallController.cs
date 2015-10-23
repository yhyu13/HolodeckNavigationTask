using UnityEngine;
using System.Collections;

namespace qtools.qmaze.example
{
	public class QBallController : MonoBehaviour 
	{
		public float moveScaleX = 1.0f;
		public float moveScaleY = 1.0f;
		public float moveMaxSpeed = 1.0f;
		public float moveLerp = 0.9f;
		
		private Rigidbody rigidBody;
		
		void Start () 
		{
			rigidBody = GetComponent<Rigidbody>();
		}
		
		void Update () 
		{
			Vector3 velocity = (Vector3.right   * Input.GetAxis("Horizontal") * moveScaleX);
			velocity+= (Vector3.forward * Input.GetAxis("Vertical")   * moveScaleY);
			velocity = Vector3.ClampMagnitude(velocity, moveMaxSpeed);
			velocity *= moveLerp;
			rigidBody.velocity = velocity;	
		}
	}
}