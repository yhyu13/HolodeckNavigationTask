using UnityEngine;
using UnityEditor;
using System.Collections;

namespace qtools.qmaze
{
	[CustomEditor(typeof(QMazePiecePack))]
	public class QMazePiecePackInspector : Editor 	
	{
		private QMazePiecePack mazePiecePack;
		private SerializedProperty customPieces;

		private Texture2D pieceIconEmpty;
		private Texture2D pieceIconLine;
		private Texture2D pieceIconDeadlock;		
		private Texture2D pieceIconTriple;						
		private Texture2D pieceIconCorner;
		private Texture2D pieceIconCrossing;
		private Texture2D pieceIconStart;
		private Texture2D pieceIconFinish;
		private Texture2D pieceIconDoubleCorner;
		private Texture2D pieceIconIntersection;		
		private Texture2D pieceIconDeadlockCorner;		
		private Texture2D pieceIconDeadlockLine;		
		private Texture2D pieceIconDeadlockTriple;		
		private Texture2D pieceIconDeadlockCrossing;		
		private Texture2D pieceIconTripleDeadlock;		
		private Texture2D pieceIconLineDeadlock;		
		private Texture2D pieceIconLineDeadlockLine;		
		private Texture2D pieceIconCornerDeadlock1;		
		private Texture2D pieceIconCornerDeadlock2;		
		private Texture2D pieceIconCornerDeadlockCorner;	

		private Texture2D addButton;
		private Texture2D removeButton;

		private GUIStyle imageButton;

		private Vector2 scrollPosition;
		private static bool basePiecesFoldout = true;
		private static bool additionalPiecesFoldout = false;

		private void OnEnable()
		{
			mazePiecePack = (QMazePiecePack)target;

			pieceIconEmpty = QInspectorUtils.getAsset<Texture2D>("PieceIconEmpty t:texture2D");
			pieceIconLine = QInspectorUtils.getAsset<Texture2D>("PieceIconLine t:texture2D");

			pieceIconDeadlock = QInspectorUtils.getAsset<Texture2D>("PieceIconDeadLock t:texture2D");
			pieceIconTriple = QInspectorUtils.getAsset<Texture2D>("PieceIconTripple t:texture2D");

			pieceIconCorner = QInspectorUtils.getAsset<Texture2D>("PieceIconCorner t:texture2D");
			pieceIconCrossing = QInspectorUtils.getAsset<Texture2D>("PieceIconCrossing t:texture2D");

			pieceIconStart = QInspectorUtils.getAsset<Texture2D>("PieceIconStart t:texture2D");
			pieceIconFinish = QInspectorUtils.getAsset<Texture2D>("PieceIconFinish t:texture2D");

			pieceIconDoubleCorner = QInspectorUtils.getAsset<Texture2D>("PieceIconDoubleCorner t:texture2D");
			pieceIconIntersection = QInspectorUtils.getAsset<Texture2D>("PieceIconIntersection t:texture2D");

			pieceIconDeadlockCorner = QInspectorUtils.getAsset<Texture2D>("PieceIconDeadlockCorner t:texture2D");
			pieceIconDeadlockLine = QInspectorUtils.getAsset<Texture2D>("PieceIconDeadlockLine t:texture2D");

			pieceIconDeadlockTriple = QInspectorUtils.getAsset<Texture2D>("PieceIconDeadlockTripple t:texture2D");
			pieceIconDeadlockCrossing = QInspectorUtils.getAsset<Texture2D>("PieceIconDeadlockCrossing t:texture2D");

			pieceIconTripleDeadlock = QInspectorUtils.getAsset<Texture2D>("PieceIconTrippleDeadlock t:texture2D");
			pieceIconLineDeadlock = QInspectorUtils.getAsset<Texture2D>("PieceIconLineDeadlock t:texture2D");

			pieceIconLineDeadlockLine = QInspectorUtils.getAsset<Texture2D>("PieceIconLineDeadlockLine t:texture2D");
			pieceIconCornerDeadlock1 = QInspectorUtils.getAsset<Texture2D>("PieceIconCornerDeadlock t:texture2D");

			pieceIconCornerDeadlock2 = QInspectorUtils.getAsset<Texture2D>("PieceIconCornerDeadlock2 t:texture2D");
			pieceIconCornerDeadlockCorner = QInspectorUtils.getAsset<Texture2D>("PieceIconCornerDeadlockCorner t:texture2D");

			addButton = QInspectorUtils.getAsset<Texture2D>("AddButton t:texture2D");
			removeButton = QInspectorUtils.getAsset<Texture2D>("RemoveButton t:texture2D");
		}

		private void initStyle()
		{
			imageButton = new GUIStyle(GUI.skin.button); 
			imageButton.normal.background = null;		
		}

		public override void OnInspectorGUI()
		{
			if (imageButton == null) initStyle();

			GUI.changed = false;

			GUILayout.Space(5);

			basePiecesFoldout = EditorGUILayout.Foldout(basePiecesFoldout, "Base Pieces");
			if (basePiecesFoldout)
			{			
				GUILayout.BeginHorizontal();
					drawPiece(QMazePieceType.Empty, "Empty", pieceIconEmpty, true);
					drawPiece(QMazePieceType.Line, "Line", pieceIconLine);            
	            GUILayout.EndHorizontal();
	            
	            GUILayout.Space(5);

	            GUILayout.BeginHorizontal();
					drawPiece(QMazePieceType.Deadlock, "Deadlock", pieceIconDeadlock);
					drawPiece(QMazePieceType.Triple, "Triple", pieceIconTriple, true);			
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
					drawPiece(QMazePieceType.Corner, "Corner", pieceIconCorner);
					drawPiece(QMazePieceType.Crossing, "Crossing", pieceIconCrossing, true);          			
				GUILayout.EndHorizontal();

				GUILayout.Space(5);

				GUILayout.BeginHorizontal();
					drawPiece(QMazePieceType.Start, "Start", pieceIconStart);
					drawPiece(QMazePieceType.Finish, "Finish", pieceIconFinish);
				GUILayout.EndHorizontal();

				GUILayout.Space(5);
			}

			additionalPiecesFoldout = EditorGUILayout.Foldout(additionalPiecesFoldout, "Additional Pieces");
			if (additionalPiecesFoldout)
			{
				GUILayout.BeginHorizontal();
					drawPiece(QMazePieceType.DoubleCorner, "Double Corner", pieceIconDoubleCorner, true);
					drawPiece(QMazePieceType.Intersection, "Intersection", pieceIconIntersection, true);   
				GUILayout.EndHorizontal();

				GUILayout.Space(5);

				GUILayout.BeginHorizontal();
				drawPiece(QMazePieceType.DeadlockCorner, "DL Corner", pieceIconDeadlockCorner, true);
				drawPiece(QMazePieceType.DeadlockLine, "DL Line", pieceIconDeadlockLine, true);   
				GUILayout.EndHorizontal();
				
				GUILayout.Space(5);

				GUILayout.BeginHorizontal();
				drawPiece(QMazePieceType.DeadlockTriple, "DL Triple", pieceIconDeadlockTriple, true);
				drawPiece(QMazePieceType.DeadlockCrossing, "DL Crossing", pieceIconDeadlockCrossing, true);   
				GUILayout.EndHorizontal();
				
				GUILayout.Space(5);

				GUILayout.BeginHorizontal();
				drawPiece(QMazePieceType.TripleDeadlock, "Triple DL", pieceIconTripleDeadlock, true);
				drawPiece(QMazePieceType.LineDeadlock, "Line DL", pieceIconLineDeadlock, true);   
				GUILayout.EndHorizontal();
				
				GUILayout.Space(5);

				GUILayout.BeginHorizontal();
				drawPiece(QMazePieceType.LineDeadlockLine, "Line DL Line", pieceIconLineDeadlockLine, true);
				drawPiece(QMazePieceType.CornerDeadlock1, "Corner DL 1", pieceIconCornerDeadlock1, true);   
				GUILayout.EndHorizontal();
				
				GUILayout.Space(5);

				GUILayout.BeginHorizontal();
				drawPiece(QMazePieceType.CornerDeadlock2, "Corner DL 2", pieceIconCornerDeadlock2, true);
				drawPiece(QMazePieceType.CornerDeadlockCorner, "Corner DL 3", pieceIconCornerDeadlockCorner, true);   
				GUILayout.EndHorizontal();
				
				GUILayout.Space(5);
			}

			if (GUI.changed) EditorUtility.SetDirty(mazePiecePack);
		}

		private void drawPiece(QMazePieceType pieceType, string pieceName, Texture2D pieceIcon, bool specOptions = false)
		{
			QMazePiece piece = mazePiecePack.getPiece(pieceType);
			
			GUILayout.Box(pieceIcon);
			GUILayout.BeginVertical();
			{
				drawPieceGeometryList(pieceName, piece);
	
				bool found = false;
				bool errorFound = false;
				for (int i = 0; i < piece.geometryList.Count; i++)
				{
					if (piece.geometryList[i] != null) 
					{
						found = true;
					}
					else
					{
						errorFound = true;
					}
				}

				if (piece.require && !found) EditorGUILayout.HelpBox("Piece geometry is required", MessageType.Warning);		
				else if (errorFound) EditorGUILayout.HelpBox("One of the elements is null", MessageType.Warning);		

				if (specOptions)
				{
					GUILayout.BeginHorizontal();
					{
						GUILayout.Space(20);
						GUILayout.Label("Use");
						piece.use = EditorGUILayout.Toggle(piece.use, GUILayout.Width(40));
					}
					GUILayout.EndHorizontal();
					
					if (piece.use)
					{
						GUILayout.BeginHorizontal();
						{
							GUILayout.Space(20);
							GUILayout.Label("Frequency", GUILayout.MinWidth(40));
							piece.frequency = Mathf.Clamp01(EditorGUILayout.FloatField(piece.frequency, GUILayout.Width(40)));	                            
						}
						GUILayout.EndHorizontal();
					}
				} 
			}
			GUILayout.EndVertical();
		}
		
		private void drawPieceGeometryList(string pieceName, QMazePiece piece)
		{
			GUILayout.BeginHorizontal();
			{
				if (GUILayout.Button(addButton, GUIStyle.none, GUILayout.Width(16), GUILayout.Height(20)))				
					piece.geometryList.Add(null);
				GUILayout.Label(pieceName);
			}
			GUILayout.EndHorizontal();
			if (piece.geometryList.Count > 0)
			{
				for(int i = 0; i < piece.geometryList.Count; i++)
				{
					GUILayout.BeginHorizontal();
					{
						bool remove = GUILayout.Button(removeButton,  GUIStyle.none, GUILayout.Width(16), GUILayout.Height(20));
						piece.geometryList[i] = (GameObject)EditorGUILayout.ObjectField(piece.geometryList[i], typeof(GameObject), true, GUILayout.MinWidth(90));
						if (remove)
						{
							piece.geometryList.RemoveAt(i);
							i--;
						}
					}
					GUILayout.EndHorizontal();
				}
			}
			else
			{
				GUILayout.BeginHorizontal();
				{
					GUILayout.Space(20);
					GameObject gameObject = (GameObject)EditorGUILayout.ObjectField(null, typeof(GameObject), true, GUILayout.MinWidth(90));
					if (gameObject != null)	piece.geometryList.Add(gameObject);				
				}
				GUILayout.EndHorizontal();
			}
		}
	}
}