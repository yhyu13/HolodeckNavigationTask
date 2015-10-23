using UnityEngine;
using System.Collections;
using qtools.qmaze;

namespace qtools.qmaze.example
{
	public class QMazeGameCamera : MonoBehaviour 
	{	
		// PUBLIC IDE
		public int padding = 1;
		public float panMoveScale = 0.1f;
		public float panLerp = 0.2f;
		public float deltaZoomScale = 0.5f;
		public float zoomLerp = 4.0f;
		public float minZoom = 4.0f;
		public float maxZoom = 1.5f;
		
		// PRIVATE
		private Rect bounds = new Rect();
		private Vector3 currentPosition;
		private Vector3 targetPosition;
		private float targetZoom;
		
		// CONSTRUCTOR
		public void Start()
		{
			targetPosition = transform.position;
			targetZoom = (maxZoom + minZoom) / 2;
			Camera.main.orthographicSize = targetZoom;

			QMazeEngine mazeEngine = FindObjectOfType<QMazeEngine>();
			if (mazeEngine != null)
			{
				setBounds(0, 0, mazeEngine.mazeWidth * mazeEngine.mazePieceWidth, mazeEngine.mazeHeight * mazeEngine.mazePieceHeight);
				setPositionOnPiece(new QVector2Int((int)(mazeEngine.mazeWidth * mazeEngine.mazePieceWidth / 2), (int)(mazeEngine.mazeHeight * mazeEngine.mazePieceHeight / 2)));
			}
		}
		
		// PUBLIC
		public void setBounds(float x, float y, float width, float height)
		{
			bounds.x = x + padding;
			bounds.y = y + padding;
			bounds.width  = width  - padding * 2;
			bounds.height = height - padding * 2;
		}
		
		public void setPositionOnPiece(QVector2Int piecePosition)
		{
			Vector3 pos = currentPosition;
			pos.x = piecePosition.x;
			pos.z = - piecePosition.y;
			currentPosition = checkPosition(pos);
			targetPosition = currentPosition;
		}
		
		public void setPosition(Vector3 newPosition)
		{
			Vector3 pos = currentPosition;
			pos.x = newPosition.x;
			pos.z = newPosition.z;
			targetPosition = checkPosition(pos);
		}

		private Vector3 lastMousePosition;
		private Vector2 deltaPosition;
		
		public void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				deltaPosition.Set(0, 0);
				lastMousePosition = Input.mousePosition;
			}

			if (Input.GetMouseButton(0))
			{
				Vector3 newDeltaPosition = (lastMousePosition - Input.mousePosition) * panMoveScale;
				deltaPosition.x += newDeltaPosition.x;
				deltaPosition.y += newDeltaPosition.y;
				lastMousePosition = Input.mousePosition;
			}

			Vector3 forward = Camera.main.transform.forward;
			forward.y = 0;
			forward.Normalize();
			
			Vector3 right = Vector3.Cross(forward, Vector3.up);

			setPosition(targetPosition + deltaPosition.y * forward - deltaPosition.x * right);
			currentPosition = Vector3.Lerp(currentPosition, targetPosition, panLerp * Time.unscaledDeltaTime);

			transform.position = currentPosition + new Vector3(-8, 8, -8);

			deltaPosition *= 0.0f;

			targetZoom = Mathf.Clamp(targetZoom - Input.mouseScrollDelta.y * deltaZoomScale, maxZoom, minZoom);
			Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, zoomLerp * Time.unscaledDeltaTime);
		}
		
		// PRIVATE
		private Vector3 checkPosition(Vector3 position)
		{
			if (position.x < bounds.x) position.x = bounds.x;
			else if (position.x > bounds.width) position.x = bounds.width;
			
			if (position.z > - bounds.y) position.z = - bounds.y;
			else if (position.z < - bounds.height) position.z = - bounds.height;
			
			return position;
		}
	}
}