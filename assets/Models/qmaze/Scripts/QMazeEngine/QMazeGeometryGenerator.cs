using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

namespace qtools.qmaze
{
	public class QMazeGeometryGenerator : MonoBehaviour 
	{
		public bool randomPiecesRotation;

		private float instantiatingProgress;

		public float getGenerationProgress()
		{
			return instantiatingProgress;
		}

		public IEnumerator generateGeometry(QMazeEngine mazeEngine, List<QMazeOutput>[][] mazeArray, float maxTime = 0.1f)
		{
			Vector3 transformPosition = transform.position;
			Quaternion transformRotation = transform.rotation;
			transform.rotation = Quaternion.identity;

			float time = Time.realtimeSinceStartup;
			int mazeWidth = mazeEngine.mazeWidth;
			int mazeHeight = mazeEngine.mazeHeight;
			float mazeSize = mazeWidth * mazeHeight;
			float count = 0;
			instantiatingProgress = 0;
			bool wasError = false;

			QMazePiecePack piecePack = mazeEngine.piecePack;
			List<QMazePiece> pieces = piecePack.toMazePieceList();
			for (int i = 0; i < pieces.Count; i++)
			{
				if ((!pieces[i].use && !pieces[i].require) || 
				    pieces[i].type == QMazePieceType.Start || 
				    pieces[i].type == QMazePieceType.Finish)
				{
					pieces.RemoveAt(i);
					i--;
				}
			}

			for (int ix = 0; ix < mazeWidth; ix++) 
			{          
				for (int iy = 0; iy < mazeHeight; iy++) 
				{
					List<QMazeOutput> mazeOutputData = mazeArray[ix][iy];

					QMazePiece targetPiece = null;

					if (QListUtil.has(mazeEngine.startPointList, ix, iy) && mazeOutputData != null && piecePack.getPiece(QMazePieceType.Start).checkFit(mazeOutputData))
					{
						targetPiece = piecePack.getPiece(QMazePieceType.Start);
					}
					else if (QListUtil.has(mazeEngine.finishPointList, ix, iy) && mazeOutputData != null && piecePack.getPiece(QMazePieceType.Finish).checkFit(mazeOutputData))
					{
						targetPiece = piecePack.getPiece(QMazePieceType.Finish);
					}
					else
					{
						QListUtil.Shuffle<QMazePiece>(pieces);
						for (int i = 0; i < pieces.Count; i++)
						{
							if (pieces[i].checkFit(mazeOutputData))
							{
								targetPiece = pieces[i];
								break;
							}
						} 
					}

					if (targetPiece == null)
					{
						if (mazeEngine.pointInMaze(new QVector2Int(ix, iy)) || mazeEngine.obstacleIsEmpty)
						{
							targetPiece = piecePack.getPiece(QMazePieceType.Empty);
						}
						else
						{
							continue;
						}
					}
					else if (targetPiece.geometryList.Count == 0)
					{
						if (mazeEngine.pointInMaze(new QVector2Int(ix, iy)))
						{
							if (!wasError)
							{
								wasError = true;
								Debug.LogWarning("QMaze: Geometry for " + targetPiece.type + " piece is not found. Please check that geometry is specified for it in the piece pack.");		
							}
						}
						continue;
					}

					GameObject prefab = targetPiece.geometryList[QMath.getRandom(0, targetPiece.geometryList.Count)];
					GameObject go;
					#if UNITY_EDITOR
					if (Application.isPlaying)
					{
						go = (GameObject)GameObject.Instantiate(prefab, new Vector3(), Quaternion.AngleAxis(randomPiecesRotation ? ((int)(UnityEngine.Random.value * 360 / 90)) * 90 : -targetPiece.getRotation(), Vector3.up));
					}
					else
					{

						go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
						go.transform.rotation = Quaternion.AngleAxis(randomPiecesRotation ? ((int)(UnityEngine.Random.value * 360 / 90)) * 90 : -targetPiece.getRotation(), Vector3.up);
					}	
					#else
						go = (GameObject)GameObject.Instantiate(prefab, new Vector3(), Quaternion.AngleAxis(randomPiecesRotation ? ((int)(UnityEngine.Random.value * 360 / 90)) * 90 : -targetPiece.getRotation(), Vector3.up));
					#endif
					go.transform.position = transformPosition + new Vector3(ix * mazeEngine.mazePieceWidth, 0, -iy * mazeEngine.mazePieceHeight);
					Vector3 scale = go.transform.localScale;
					go.transform.parent = transform;
					go.transform.localScale = scale;

					count++;
					instantiatingProgress = count / mazeSize;
					
					if (Time.realtimeSinceStartup - time > maxTime)
					{
						time = Time.realtimeSinceStartup;
						yield return null;
					}
				}
			}

			transform.rotation = transformRotation;
		}
	}
}
