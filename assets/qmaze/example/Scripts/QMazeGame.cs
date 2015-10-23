using UnityEngine;
using System.Collections;
using qtools.qmaze;

namespace qtools.qmaze.example
{
	public class QMazeGame : MonoBehaviour 
	{
		public QMazeEngine mazeEngine;
		public QMazeTerrainEngine mazeTerrainEngine;
		
		void Start ()
		{
			mazeEngine.generateMaze();
			mazeTerrainEngine.generateTerrain(mazeEngine.mazeWidth, mazeEngine.mazeHeight, mazeEngine.mazePieceWidth, mazeEngine.mazePieceHeight);
		}

		void OnGUI()
		{
			string progress = " (" + (mazeEngine.getGenerationProgress() * 100).ToString("000") + ")";
			if (GUI.Button(new Rect(20, 20, 200, 24), "Generate New Maze" + progress))
			{
				mazeEngine.useSeed = false;

				mazeEngine.clear();
				mazeTerrainEngine.clear();

				mazeEngine.generateMaze();
				mazeTerrainEngine.generateTerrain(mazeEngine.mazeWidth, mazeEngine.mazeHeight, mazeEngine.mazePieceWidth, mazeEngine.mazePieceHeight);
			}
		}
	} 
}