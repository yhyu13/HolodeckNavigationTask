using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace qtools.qmaze
{
	[CustomEditor(typeof(QMazeEngine))]
	public class QMazeEngineInspector: Editor 
	{
		private Texture2D mazeIcon;
		private Texture2D pieceIcon;

		private bool generateStart;
		private bool generateStartRandom;
		private bool generateFinish;
		private bool generateFinishRandom;
		private bool generateExit;
		private bool generateObstacles;

		private static bool generateStartFoldout = true;
		private static bool generateFinishFoldout = true;
		private static bool generateExitFoldout = true;
		private static bool generateObstaclesFoldout = true;

		SerializedObject mazeEngineSO;
		SerializedProperty startPointList;
		SerializedProperty finishPointList;
		SerializedProperty exitPointList;
		SerializedProperty obstaclePointList; 

		private QMazeEngine mazeEngine;
		private GUIStyle labelCaption;
		private GUIStyle textFieldHC;
		private GUIStyle textFieldVC;
		private Vector2 scrollPosition;

		private void OnEnable() 
		{
			mazeEngine = (QMazeEngine)target;

			mazeIcon   = QInspectorUtils.getAsset<Texture2D>("MazeIcon t:texture2D");
			pieceIcon  = QInspectorUtils.getAsset<Texture2D>("PieceIcon t:texture2D");

			generateStart = mazeEngine.generateStartCount > 0 || mazeEngine.startPointList.Count > 0 ? true : false;
			generateFinish = mazeEngine.generateFinishCount > 0 || mazeEngine.startPointList.Count > 0 ? true : false;
			generateExit = mazeEngine.exitPointList.Count > 0;
			generateObstacles = mazeEngine.obstaclePointList.Count > 0;

			mazeEngineSO = new SerializedObject(target);
			startPointList = mazeEngineSO.FindProperty("startPointList");
			finishPointList = mazeEngineSO.FindProperty("finishPointList");
			exitPointList = mazeEngineSO.FindProperty("exitPointList");
			obstaclePointList = mazeEngineSO.FindProperty("obstaclePointList");
		}

		private void initStyle()
		{
			labelCaption = new GUIStyle(GUI.skin.label); 
			labelCaption.alignment = TextAnchor.MiddleCenter;
			labelCaption.normal.background = null;
			
			textFieldHC = new GUIStyle(GUI.skin.textField); 
			textFieldHC.alignment = TextAnchor.UpperCenter;
			
			textFieldVC = new GUIStyle(GUI.skin.textField); 
            textFieldVC.alignment = TextAnchor.MiddleLeft;
		}

		public override void OnInspectorGUI()
		{
			if (labelCaption == null) initStyle();

			mazeEngineSO.Update();
			bool errorFound = false;

			GUI.changed = false;

			GUILayout.Space(7);
			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginVertical();
				{
					GUILayout.BeginHorizontal();
					{
						GUILayout.Space(28);
						GUILayout.Label("Maze Size", labelCaption, GUILayout.Width(94));
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					{
						GUI.DrawTexture(EditorGUILayout.GetControlRect(GUILayout.Width(120), GUILayout.Height(50)), mazeIcon, ScaleMode.ScaleToFit);
						mazeEngine.mazeHeight = EditorGUILayout.IntField(mazeEngine.mazeHeight, textFieldVC, GUILayout.Height(50), GUILayout.Width(30));
						if (mazeEngine.mazeHeight < 1) mazeEngine.mazeHeight = 1;
					}
					GUILayout.EndHorizontal();
					mazeEngine.mazeWidth = EditorGUILayout.IntField(mazeEngine.mazeWidth, textFieldHC, GUILayout.Width(97));
					if (mazeEngine.mazeWidth < 1) mazeEngine.mazeWidth = 1;
				}
				GUILayout.EndVertical(); 

				GUILayout.BeginVertical();
				{
					GUILayout.BeginHorizontal();
					{
						GUILayout.Space(28);
						GUILayout.Label("Piece Size", labelCaption, GUILayout.Width(94));
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					{
						GUI.DrawTexture(EditorGUILayout.GetControlRect(GUILayout.Width(120), GUILayout.Height(50)), pieceIcon, ScaleMode.ScaleToFit);
						mazeEngine.mazePieceHeight = EditorGUILayout.FloatField(mazeEngine.mazePieceHeight, textFieldVC, GUILayout.Height(50), GUILayout.Width(30));
					}
					GUILayout.EndHorizontal();
					mazeEngine.mazePieceWidth = EditorGUILayout.FloatField(mazeEngine.mazePieceWidth, textFieldHC, GUILayout.Width(97));
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(5);
			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
			GUILayout.Space(5);

			mazeEngine.piecePack = (QMazePiecePack)EditorGUILayout.ObjectField("Maze Pieces Pack", mazeEngine.piecePack, typeof(QMazePiecePack), true);
			if (mazeEngine.piecePack == null) 
			{
				EditorGUILayout.HelpBox("Maze Pieces Pack is Required", MessageType.Warning);
				errorFound = true;
			}

			mazeEngine.mazeGeometryGenerator = (QMazeGeometryGenerator)EditorGUILayout.ObjectField("Maze Geometry Generator", mazeEngine.mazeGeometryGenerator, typeof(QMazeGeometryGenerator), true);

			GUILayout.Space(5);
			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
			GUILayout.Space(5);
					
			generateStart = GUILayout.Toggle(generateStart, "Generate Start Piece");
			EditorGUI.indentLevel++;	
			if (generateStart)
			{
				bool generateStartRandomCur = EditorGUILayout.Toggle("Random Position", generateStartRandom, GUILayout.ExpandWidth(true));
				if (generateStartRandom != generateStartRandomCur)
				{
					generateStartRandom = generateStartRandomCur;
					mazeEngine.generateStartCount = 0;
					startPointList.ClearArray();
				}

				if (generateStartRandom)
				{
					startPointList.ClearArray();
					mazeEngine.startPointList.Clear();
					mazeEngine.generateStartCount = EditorGUILayout.IntField("Count", mazeEngine.generateStartCount);
				}
				else
				{
					generateStartFoldout = showVector2IntList(startPointList, generateStartFoldout);
				}
			}
			else
			{
				mazeEngine.generateStartCount = 0;
				startPointList.ClearArray();
			}
			EditorGUI.indentLevel--;	

			GUILayout.Space(5);
			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
			GUILayout.Space(5);

			generateFinish = GUILayout.Toggle(generateFinish, "Generate Finish Piece");
			EditorGUI.indentLevel++;	
			if (generateFinish)
			{
				bool generateFinishRandomCur = EditorGUILayout.Toggle("Random Position", generateFinishRandom, GUILayout.ExpandWidth(true));
				if (generateFinishRandom != generateFinishRandomCur)
				{
					generateFinishRandom = generateFinishRandomCur;
					mazeEngine.generateFinishCount = 0;
					finishPointList.ClearArray();
				}

				if (generateFinishRandom)
				{
					finishPointList.ClearArray();
					mazeEngine.finishPointList.Clear();
					mazeEngine.generateFinishCount = EditorGUILayout.IntField("Count", mazeEngine.generateFinishCount);
				}
				else
				{
					generateFinishFoldout = showVector2IntList(finishPointList, generateFinishFoldout);
				}
			}
			else
			{
				mazeEngine.generateFinishCount = 0;
				finishPointList.ClearArray();
			}
			EditorGUI.indentLevel--;

			GUILayout.Space(5);
			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
			GUILayout.Space(5);

			generateExit = GUILayout.Toggle(generateExit, "Generate Exit");
			EditorGUI.indentLevel++;	
			if (generateExit)
			{
				generateExitFoldout = showVector2IntList(exitPointList, generateExitFoldout);
			}
			else
			{
				exitPointList.ClearArray();
			}
			EditorGUI.indentLevel--;
			
			GUILayout.Space(5);
			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
			GUILayout.Space(5);

			generateObstacles = GUILayout.Toggle(generateObstacles, "Obstacles List");
			EditorGUI.indentLevel++;	
			if (generateObstacles)
			{
				mazeEngine.obstacleIsEmpty = EditorGUILayout.ToggleLeft("Empty Piece in place obstacles", mazeEngine.obstacleIsEmpty, GUILayout.ExpandWidth(true));
				generateObstaclesFoldout = showVector2IntList(obstaclePointList, generateObstaclesFoldout);
			}
			else
			{
				obstaclePointList.ClearArray();
			}
			EditorGUI.indentLevel--;
			
			GUILayout.Space(5);
			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
			GUILayout.Space(5);

			mazeEngine.useSeed = GUILayout.Toggle(mazeEngine.useSeed, "Use seed for generation");
			if (mazeEngine.useSeed)		
			{
				EditorGUI.indentLevel++;
				mazeEngine.seed = EditorGUILayout.IntField("Seed", mazeEngine.seed);
				EditorGUI.indentLevel--;
			}

			GUILayout.Space(5);
			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
			GUILayout.Space(5);
			
			mazeEngine.onlyWay = GUILayout.Toggle(mazeEngine.onlyWay, "Generating with the only path");

			if (mazeEngine.gameObject.activeInHierarchy)
			{
				GUILayout.Space(5);
				GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
				GUILayout.Space(5);

				if (mazeEngine.transform.childCount == 0) 
				{
					if (errorFound) GUI.enabled = false;
					if (GUILayout.Button("Generate a Maze", GUILayout.ExpandWidth(true)))
					{
						mazeEngine.generateMaze(true, false); 
					}
					GUI.enabled = true;
				}
				else
				{
					if (GUILayout.Button("Destroy the Maze", GUILayout.ExpandWidth(true)))
					{ 
						mazeEngine.clear();
					}
				}
			}

			GUILayout.Space(5);

			mazeEngineSO.ApplyModifiedProperties();
			if (GUI.changed) EditorUtility.SetDirty(mazeEngine);
		}

		private bool showVector2IntList(SerializedProperty list, bool foldout)
		{
			int listSize = list.arraySize;
			listSize = EditorGUILayout.IntField("Count", listSize);

			if(listSize != list.arraySize)
			{
				while(listSize > list.arraySize)
				{
					list.InsertArrayElementAtIndex(list.arraySize);
				}
				while(listSize < list.arraySize)
				{
					list.DeleteArrayElementAtIndex(list.arraySize - 1);
				}
			}

			if (listSize > 0)
			{
				EditorGUI.indentLevel++;
				foldout = EditorGUILayout.Foldout(foldout, "Point List");
				if (foldout)
				{
					for(int i = 0; i < list.arraySize; i++)
					{
						SerializedProperty element = list.GetArrayElementAtIndex(i);
						SerializedProperty x = element.FindPropertyRelative("x");
						SerializedProperty y = element.FindPropertyRelative("y");

						EditorGUILayout.LabelField("Point " + (i + 1));
						EditorGUI.indentLevel++;	
						EditorGUILayout.PropertyField(x);
						EditorGUILayout.PropertyField(y);
						EditorGUI.indentLevel--;	
					}
				}
				EditorGUI.indentLevel--;	
			}

			return foldout;
		}
	}
}