using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using qtools.qmaze;

namespace qtools.qmaze.example
{
	public class QMazePieceRotator : MonoBehaviour
	{
		private float targetAngle;
		private Quaternion targetRotation;

		void Awake () 
		{
			targetAngle = transform.eulerAngles.y;
			targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.up);	     
		}

		private float mouseDownTime;
		private Vector3 mousePosition;

		void OnMouseDown()
		{
			mouseDownTime = Time.realtimeSinceStartup;
			mousePosition = Input.mousePosition;
		}

		void OnMouseOver()
		{
			Vector3 position = QMazeSelector.getInstance().transform.position;
			position.x = transform.position.x;
			position.z = transform.position.z;
			QMazeSelector.getInstance().transform.position = position;
		}

		void OnMouseUp()
		{
			if (Time.realtimeSinceStartup - mouseDownTime < 0.300 && Vector3.SqrMagnitude(Input.mousePosition - mousePosition) < 10)
			{
				targetAngle = (targetAngle + 90) % 360;
				targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.up);	     
			}
		}

		private void Update()
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
		}
	}
}