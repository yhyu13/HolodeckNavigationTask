using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace qtools.qmaze
{
	[ExecuteInEditMode]
	public class QMazeEngine : MonoBehaviour 
	{
		public QMazePiecePack piecePack;
		public QMazeGeometryGenerator mazeGeometryGenerator;

		public int mazeWidth = 25;
		public int mazeHeight = 25;

		public float mazePieceWidth = 10;
		public float mazePieceHeight = 10;

		public int generateStartCount = 0;
		public List<QVector2Int> startPointList = new List<QVector2Int>();

		public int generateFinishCount = 0;
		public List<QVector2Int> finishPointList = new List<QVector2Int>();

		public List<QVector2Int> exitPointList = new List<QVector2Int>();

		public bool obstacleIsEmpty = false;
		public List<QVector2Int> obstaclePointList = new List<QVector2Int>();

		public bool useSeed = false;
		public bool onlyWay = false;
		public int seed = 0;	
		 
		[SerializeField] private List<QMazeOutput>[][] mazeArray;	
		[SerializeField] private bool inited = false;	   
		private float generationProgress;
		private List<CheckTask> checkTaskList = new List<CheckTask>();
		private List<QVector2Int> path;
		private QMazeOutputDirection lastDirection;

		private void Awake()		
		{
			if (!inited)
			{
				inited = true;

				if (gameObject.GetComponent<QMazeGeometryGenerator>() == null)			
					mazeGeometryGenerator = gameObject.AddComponent<QMazeGeometryGenerator>();
				
				if (gameObject.GetComponent<QMazePiecePack>() == null)
					piecePack = gameObject.AddComponent<QMazePiecePack>();
			}
		}

		public void generateMaze(bool generateWithGeometry = true, bool asynchronous = true, float maxTime = 0.1f)
		{
			if (useSeed) QMath.setSeed(seed);

			if (asynchronous)
			{
				StartCoroutine(generate(generateWithGeometry, asynchronous, maxTime));				 
			}
			else
			{
				IEnumerator generationEnumerator = generate(generateWithGeometry, asynchronous, maxTime);
				while (generationEnumerator.MoveNext());
			}
		}

		public void clear()
		{
			while (transform.childCount > 0)
			{
				DestroyImmediate(transform.GetChild(0).gameObject);
			}
		}
		
	    public float getGenerationProgress()
	    {
			if (mazeGeometryGenerator != null)
				return (generationProgress / (mazeWidth * mazeHeight) + mazeGeometryGenerator.getGenerationProgress()) / 2;
            else 
				return generationProgress;
	    }

		public List<QMazeOutput>[][] getMazeData()
		{
			return mazeArray;
		}

		private IEnumerator generate(bool generateWithGeometry = true, bool asynchronous = true, float maxTime = 0.1f)
		{
			generationProgress = 0;

			generateStartPoint();
			generateFinishPoint();			

			QVector2Int startGenerationPoint = new QVector2Int(QMath.getRandom(0, mazeWidth), QMath.getRandom(0, mazeHeight));
			while (QListUtil.has(startPointList, startGenerationPoint) || 
			       QListUtil.has(finishPointList, startGenerationPoint) || 
			       QListUtil.has(obstaclePointList, startGenerationPoint))
			{
				startGenerationPoint.x = QMath.getRandom(0, mazeWidth);
				startGenerationPoint.y = QMath.getRandom(0, mazeHeight);
			}

			path = new List<QVector2Int>();
			mazeArray = new List<QMazeOutput>[mazeWidth][];
			for (int px = 0; px < mazeWidth; px++) mazeArray[px] = new List<QMazeOutput>[mazeHeight];
			
			lastDirection = QMazeOutputDirection.None;
			QVector2Int currentPosition = new QVector2Int(startGenerationPoint.x, startGenerationPoint.y);
			
			QMazeOutput output = new QMazeOutput();
			mazeArray[currentPosition.x][currentPosition.y] = new List<QMazeOutput>();
			mazeArray[currentPosition.x][currentPosition.y].Add(output);
			
			path.Add(new QVector2Int(currentPosition.x, currentPosition.y));

			checkTaskList.Clear();
			if (startPointList.Count > 0 || finishPointList.Count > 0) checkTaskList.Add(checkStartFinishPoint);
			checkTaskList.Add(checkStandard);
			if (piecePack.getPiece(QMazePieceType.Intersection		  ).use) checkTaskList.Add(checkUnder);
			if (piecePack.getPiece(QMazePieceType.Crossing			  ).use && !onlyWay) checkTaskList.Add(checkCrossing);
			if (piecePack.getPiece(QMazePieceType.Triple			  ).use && !onlyWay) checkTaskList.Add(checkTripple);
			if (piecePack.getPiece(QMazePieceType.DoubleCorner		  ).use) checkTaskList.Add(checkDoubleCorner);
			if (piecePack.getPiece(QMazePieceType.DeadlockCorner	  ).use) checkTaskList.Add(checkDeadlockCorner);
			if (piecePack.getPiece(QMazePieceType.DeadlockLine	      ).use) checkTaskList.Add(checkDeadlockLine);
			if (piecePack.getPiece(QMazePieceType.DeadlockTriple	  ).use) checkTaskList.Add(checkDeadlockTriple);
			if (piecePack.getPiece(QMazePieceType.DeadlockCrossing	  ).use) checkTaskList.Add(checkDeadlockCrossing);
			if (piecePack.getPiece(QMazePieceType.TripleDeadlock	  ).use) checkTaskList.Add(checkTripleDeadlock);
			if (piecePack.getPiece(QMazePieceType.LineDeadlock		  ).use) checkTaskList.Add(checkLineDeadlock);
			if (piecePack.getPiece(QMazePieceType.LineDeadlockLine	  ).use) checkTaskList.Add(checkLineDeadlockLine);
			if (piecePack.getPiece(QMazePieceType.CornerDeadlock1	  ).use) checkTaskList.Add(checkCornerDeadlock1);
			if (piecePack.getPiece(QMazePieceType.CornerDeadlock2	  ).use) checkTaskList.Add(checkCornerDeadlock2);
			if (piecePack.getPiece(QMazePieceType.CornerDeadlockCorner).use) checkTaskList.Add(checkCornerDeadlockCorner);
			if (piecePack.getPiece(QMazePieceType.Empty				  ).use) checkTaskList.Add(checkEmpty);

			float time = Time.realtimeSinceStartup;

			do
			{
				int lastPathIndex = path.Count - 1;
				currentPosition.set(path[lastPathIndex].x, path[lastPathIndex].y);             
				
				lastDirection = QMazeOutputDirection.None;
				QMazeOutput outputArray = QMazeOutput.getShuffleOutput();

				foreach (QMazeOutputDirection dir in outputArray.outputDirList)
				{
					QVector2Int newPosition = new QVector2Int(currentPosition.x + QMazeOutput.dx[dir], currentPosition.y + QMazeOutput.dy[dir]);
					if (pointInMaze(newPosition))
					{
						if (mazeArray[currentPosition.x][currentPosition.y].Count == 1)
						{
							List<QMazeOutput> newPositionOutputs = mazeArray[newPosition.x][newPosition.y];

							int checkResult = 0;
							for (int i = 0; i < checkTaskList.Count; i++)
							{
	                            CheckTask checkTask = checkTaskList[i];
								checkResult = checkTask(currentPosition, newPosition, newPositionOutputs, dir);
								if (checkResult != CHECK_FAILED) break;
							}

							if (checkResult == CHECK_CONTINUE) continue;
							if (checkResult == CHECK_BREAK) 
							{
								generationProgress++;
								break;
							}
						}
					}
					else if (QListUtil.has(exitPointList, newPosition))
					{
						if (!mazeArray[currentPosition.x][currentPosition.y][0].outputDirList.Contains(dir))
							mazeArray[currentPosition.x][currentPosition.y][0].outputDirList.Add(dir);
					}
				}
				
				if (lastDirection == QMazeOutputDirection.None)            
					path.RemoveAt(path.Count - 1);   

				if (asynchronous && Time.realtimeSinceStartup - time > maxTime)
				{
					time = Time.realtimeSinceStartup;
					yield return null;
				}
			}
			while (path.Count > 0);		

			if (generateWithGeometry && mazeGeometryGenerator != null)			
			{
				if (asynchronous)
				{
					yield return StartCoroutine(mazeGeometryGenerator.generateGeometry(this, mazeArray, maxTime));
				}
				else
				{
					IEnumerator geometryCreatorEnumerator = mazeGeometryGenerator.generateGeometry(this, mazeArray, maxTime);
					while (geometryCreatorEnumerator.MoveNext());
				}
			}				
		}

		private void generateStartPoint()
		{
			if (generateStartCount > 0) startPointList.Clear();
			for (int i = 0; i < generateStartCount; i++)
			{
				QVector2Int newStartPoint = new QVector2Int(QMath.getRandom(0, mazeWidth), QMath.getRandom(0, mazeHeight));
				while (QListUtil.has(startPointList, newStartPoint) || QListUtil.has(finishPointList, newStartPoint))
				{
					newStartPoint.x = QMath.getRandom(0, mazeWidth);
					newStartPoint.y = QMath.getRandom(0, mazeHeight);
				}				
				startPointList.Add(newStartPoint);
			}
		}
		
		private void generateFinishPoint()
		{
			if (generateFinishCount > 0) finishPointList.Clear();
			for (int i = 0; i < generateFinishCount; i++)
			{
				QVector2Int newFinishPoint = new QVector2Int(QMath.getRandom(0, mazeWidth), QMath.getRandom(0, mazeHeight));
				while (QListUtil.has(startPointList, newFinishPoint) || QListUtil.has(finishPointList, newFinishPoint))
				{
					newFinishPoint.x = QMath.getRandom(0, mazeWidth);
					newFinishPoint.y = QMath.getRandom(0, mazeHeight);
				}
				finishPointList.Add(newFinishPoint);
			}
		}

	    public bool pointInMaze(QVector2Int point)
	    {
			bool inMaze = point.x >= 0 && point.x < mazeWidth && point.y >= 0 && point.y < mazeHeight;
			bool notInObstacle = !QListUtil.has(obstaclePointList, point);
			return inMaze && notInObstacle;
	    }

		private delegate int CheckTask(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir);
		private const int CHECK_CONTINUE = 0;
		private const int CHECK_BREAK    = 1;
		private const int CHECK_FAILED   = 2;
		 
		private int checkStartFinishPoint(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QListUtil.has(startPointList, newPosition) || QListUtil.has(finishPointList, newPosition)) 
			{
				if (mazeArray[newPosition.x][newPosition.y] == null)
				{
					QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y][mazeArray[currentPosition.x][currentPosition.y].Count - 1];
					output.outputDirList.Add(dir);
					
					output = new QMazeOutput();
					output.outputDirList.Add(QMazeOutput.opposite[dir]);
					mazeArray[newPosition.x][newPosition.y] = new List<QMazeOutput>();
					mazeArray[newPosition.x][newPosition.y].Add(output);
				}
				return CHECK_CONTINUE;
			}
			else if (QListUtil.has(startPointList, currentPosition) || QListUtil.has(finishPointList, currentPosition)) 
			{
				return CHECK_BREAK;
			}
			return CHECK_FAILED;
		}

		private int checkStandard(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (mazeArray[newPosition.x][newPosition.y] == null)
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y][mazeArray[currentPosition.x][currentPosition.y].Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y] = new List<QMazeOutput>();
				mazeArray[newPosition.x][newPosition.y].Add(output);
				
				path.Add(new QVector2Int(newPosition.x, newPosition.y));
				lastDirection = dir;

				return CHECK_BREAK;
			}
			return CHECK_FAILED;
		}

		private int checkUnder(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
	    {
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.Intersection).frequency &&
			    newPositionOutputs != null && 
			    newPositionOutputs.Count == 1 && 
			    newPositionOutputs[0].outputDirList.Count == 2 &&
			    !newPositionOutputs[0].outputDirList.Contains(dir) && 
			    !newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]))
			{
				QVector2Int newPosition2 = newPosition.clone();
				newPosition2.x += QMazeOutput.dx[dir];
				newPosition2.y += QMazeOutput.dy[dir];

				if (pointInMaze(newPosition2) && 
				    mazeArray[newPosition2.x][newPosition2.y] == null) 
	            {
					QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y][mazeArray[currentPosition.x][currentPosition.y].Count - 1];
					output.outputDirList.Add(dir);
					
					output = new QMazeOutput();
					output.outputDirList.Add(dir);
					output.outputDirList.Add(QMazeOutput.opposite[dir]);
					mazeArray[newPosition.x][newPosition.y].Add(output);							
					
					output = new QMazeOutput();
					output.outputDirList.Add(QMazeOutput.opposite[dir]);
					mazeArray[newPosition2.x][newPosition2.y] = new List<QMazeOutput>();
					mazeArray[newPosition2.x][newPosition2.y].Add(output);
					
					path.Add(new QVector2Int(newPosition2.x, newPosition2.y));
					lastDirection = dir;
					
					return CHECK_BREAK;
	            }		        
			}
			return CHECK_FAILED;
	    }

		private int checkCrossing(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.Crossing).frequency && 
			    newPositionOutputs != null && 
			    newPositionOutputs.Count == 1 && 
			    newPositionOutputs[0].outputDirList.Count == 2 &&
			    !newPositionOutputs[0].outputDirList.Contains(dir) && 
			    !newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]))
			{
				QVector2Int newPosition2 = newPosition.clone();
				newPosition2.x += QMazeOutput.dx[dir];
				newPosition2.y += QMazeOutput.dy[dir];

				if (pointInMaze(newPosition2) && 
				    mazeArray[newPosition2.x][newPosition2.y] == null)
				{
					QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y][mazeArray[currentPosition.x][currentPosition.y].Count - 1];
					output.outputDirList.Add(dir);

					mazeArray[newPosition.x][newPosition.y][0].outputDirList.Add(dir);
					mazeArray[newPosition.x][newPosition.y][0].outputDirList.Add(QMazeOutput.opposite[dir]);
					
					output = new QMazeOutput();
					output.outputDirList.Add(QMazeOutput.opposite[dir]);
					mazeArray[newPosition2.x][newPosition2.y] = new List<QMazeOutput>();
					mazeArray[newPosition2.x][newPosition2.y].Add(output);
					
					path.Add(new QVector2Int(newPosition2.x, newPosition2.y));
					lastDirection = dir;
					
					return CHECK_BREAK;					
				}
			}
			return CHECK_FAILED;
		}

		private int checkTripple(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.Triple).frequency && 
			    newPositionOutputs.Count == 1 && 
			    newPositionOutputs[0].outputDirList.Count == 2 && 
			    newPositionOutputs[0].outputDirList.Contains(dir) && 
			    !newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y][mazeArray[currentPosition.x][currentPosition.y].Count - 1];
				output.outputDirList.Add(dir);

				newPositionOutputs[newPositionOutputs.Count - 1].outputDirList.Add(QMazeOutput.opposite[dir]);

				return CHECK_CONTINUE;
			}
			return CHECK_FAILED;
		}

		private int checkDoubleCorner(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
	    {
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.DoubleCorner).frequency && 
			    newPositionOutputs.Count == 1 && 
			    newPositionOutputs[0].outputDirList.Count == 2 && 
			    newPositionOutputs[0].outputDirList.Contains(dir) && 
			    !newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]))
	        {
	            QVector2Int newPos1 = new QVector2Int(newPosition.x + QMazeOutput.dx[QMazeOutput.rotateCW[dir]],
	                                                  newPosition.y + QMazeOutput.dy[QMazeOutput.rotateCW[dir]]);
	            QVector2Int newPos2 = new QVector2Int(newPosition.x + QMazeOutput.dx[QMazeOutput.rotateCCW[dir]],
	                                                  newPosition.y + QMazeOutput.dy[QMazeOutput.rotateCCW[dir]]);

				if ((pointInMaze(newPos1) && mazeArray[newPos1.x][newPos1.y] == null && newPositionOutputs[0].outputDirList.Contains(QMazeOutput.rotateCCW[dir])) ||
				    (pointInMaze(newPos2) && mazeArray[newPos2.x][newPos2.y] == null && newPositionOutputs[0].outputDirList.Contains(QMazeOutput.rotateCW [dir])))
	            {
					QMazeOutputDirection dir2 = dir;
					
					QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y][mazeArray[currentPosition.x][currentPosition.y].Count - 1];
					output.outputDirList.Add(dir);
					
					output = new QMazeOutput();
					output.outputDirList.Add(QMazeOutput.opposite[dir]);
					if (!mazeArray[newPosition.x][newPosition.y][0].outputDirList.Contains(QMazeOutput.rotateCW[dir]))
					{
						output.outputDirList.Add(QMazeOutput.rotateCW[dir]);
						dir2 = QMazeOutput.rotateCW[dir];
					}
					else 
					{
						output.outputDirList.Add(QMazeOutput.rotateCCW[dir]);
						dir2 = QMazeOutput.rotateCCW[dir];
					}
					mazeArray[newPosition.x][newPosition.y].Add(output);
					
					newPosition.x += QMazeOutput.dx[dir2];
					newPosition.y += QMazeOutput.dy[dir2];
					
					output = new QMazeOutput();
					output.outputDirList.Add(QMazeOutput.opposite[dir2]);
					mazeArray[newPosition.x][newPosition.y] = new List<QMazeOutput>();
					mazeArray[newPosition.x][newPosition.y].Add(output);
					
					path.Add(new QVector2Int(newPosition.x, newPosition.y));
					lastDirection = dir2;
					
					return CHECK_BREAK;
	            }
	        }
			return CHECK_FAILED;
	    }

		private int checkDeadlockCorner(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{		
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.DeadlockCorner).frequency && 
			    newPositionOutputs != null && 
			    newPositionOutputs.Count == 1 && 
			    newPositionOutputs[0].outputDirList.Count == 1 &&
			    (newPositionOutputs[0].outputDirList.Contains(QMazeOutput.rotateCW[dir]) || newPositionOutputs[0].outputDirList.Contains(QMazeOutput.rotateCCW[dir])))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y][mazeArray[currentPosition.x][currentPosition.y].Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y].Add(output);
				
				return CHECK_CONTINUE;
			}
			return CHECK_FAILED;
		}

		private int checkDeadlockLine(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.DeadlockLine).frequency && 
			    newPositionOutputs != null && 
			    newPositionOutputs.Count == 1 &&
			    newPositionOutputs[0].outputDirList.Count == 1 && 
			    newPositionOutputs[0].outputDirList.Contains(dir))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y][mazeArray[currentPosition.x][currentPosition.y].Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y].Add(output);
				
				return CHECK_CONTINUE;
			}
			return CHECK_FAILED;
		}

		private int checkDeadlockTriple(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.DeadlockTriple).frequency && 
			    newPositionOutputs != null && 
			    newPositionOutputs.Count == 2 &&
			    newPositionOutputs[0].outputDirList.Count == 1 && 
			    !newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]) &&
			    newPositionOutputs[1].outputDirList.Count == 1 &&
			    !newPositionOutputs[1].outputDirList.Contains(QMazeOutput.opposite[dir]))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y][mazeArray[currentPosition.x][currentPosition.y].Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y].Add(output);
				
				return CHECK_CONTINUE;
			}
			return CHECK_FAILED;
		}

		private int checkDeadlockCrossing(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.DeadlockCrossing).frequency && 
			    newPositionOutputs != null && 
			    newPositionOutputs.Count == 3 &&
			    newPositionOutputs[0].outputDirList.Count == 1 && 
			    !newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]) &&
			    newPositionOutputs[1].outputDirList.Count == 1 && 
			    !newPositionOutputs[1].outputDirList.Contains(QMazeOutput.opposite[dir]) &&
			    newPositionOutputs[2].outputDirList.Count == 1 &&
			    !newPositionOutputs[2].outputDirList.Contains(QMazeOutput.opposite[dir]))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y][mazeArray[currentPosition.x][currentPosition.y].Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y].Add(output);
				
				return CHECK_CONTINUE;
			}
			return CHECK_FAILED;
		}

		private int checkTripleDeadlock(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.TripleDeadlock).frequency && 
			    newPositionOutputs != null && 
			    newPositionOutputs.Count == 1 &&
			    newPositionOutputs[0].outputDirList.Count == 3 && 
			    !newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y][mazeArray[currentPosition.x][currentPosition.y].Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y].Add(output);
				
				return CHECK_CONTINUE;
			}
			return CHECK_FAILED;
		}

		private int checkLineDeadlock(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.LineDeadlock).frequency && 
			    newPositionOutputs != null && 
			    newPositionOutputs.Count == 1 &&
			    newPositionOutputs[0].outputDirList.Count == 2 && 
			    !newPositionOutputs[0].outputDirList.Contains(dir) && 
			    !newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y][mazeArray[currentPosition.x][currentPosition.y].Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y].Add(output);
				
				return CHECK_CONTINUE;
			}
			return CHECK_FAILED;
		}

		private int checkLineDeadlockLine(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.LineDeadlockLine).frequency && 
			    newPositionOutputs != null && 
			    newPositionOutputs.Count == 2 && 
			    !newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]) &&
				!newPositionOutputs[1].outputDirList.Contains(QMazeOutput.opposite[dir]) && 
				((newPositionOutputs[0].outputDirList.Count == 2 && newPositionOutputs[1].outputDirList.Count == 1 && newPositionOutputs[0].outputDirList[0] == QMazeOutput.opposite[newPositionOutputs[0].outputDirList[1]]) || 
				 (newPositionOutputs[0].outputDirList.Count == 1 && newPositionOutputs[1].outputDirList.Count == 2 && newPositionOutputs[1].outputDirList[0] == QMazeOutput.opposite[newPositionOutputs[1].outputDirList[1]])))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y][mazeArray[currentPosition.x][currentPosition.y].Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y].Add(output);
				
				return CHECK_CONTINUE;											
			}
			return CHECK_FAILED;
		}

		private int checkCornerDeadlock1(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.CornerDeadlock1).frequency && 
			    newPositionOutputs != null && 
			    newPositionOutputs.Count == 1 &&
			    newPositionOutputs[0].outputDirList.Count == 2 && 
			    newPositionOutputs[0].outputDirList.Contains(dir) && 
			    newPositionOutputs[0].outputDirList.Contains(QMazeOutput.rotateCW[dir]))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y][mazeArray[currentPosition.x][currentPosition.y].Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y].Add(output);
				
				return CHECK_CONTINUE;						
			}
			return CHECK_FAILED;
		}

		private int checkCornerDeadlock2(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.CornerDeadlock2).frequency && 
			    newPositionOutputs.Count == 1 &&
			    newPositionOutputs[0].outputDirList.Count == 2 && 
			    newPositionOutputs[0].outputDirList.Contains(dir) && 
			    newPositionOutputs[0].outputDirList.Contains(QMazeOutput.rotateCCW[dir]))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y][mazeArray[currentPosition.x][currentPosition.y].Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y].Add(output);
				
				return CHECK_CONTINUE;			
			}
			return CHECK_FAILED;
		}

		private int checkCornerDeadlockCorner(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.CornerDeadlockCorner).frequency && 
			    newPositionOutputs.Count == 2 && 
			    !newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]) && 
				!newPositionOutputs[1].outputDirList.Contains(QMazeOutput.opposite[dir]) &&
				((newPositionOutputs[0].outputDirList.Count == 2 && newPositionOutputs[1].outputDirList.Count == 1 && newPositionOutputs[0].outputDirList[0] != QMazeOutput.opposite[newPositionOutputs[0].outputDirList[1]]) ||
				 (newPositionOutputs[0].outputDirList.Count == 1 && newPositionOutputs[1].outputDirList.Count == 2 && newPositionOutputs[1].outputDirList[0] != QMazeOutput.opposite[newPositionOutputs[1].outputDirList[1]])))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y][mazeArray[currentPosition.x][currentPosition.y].Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y].Add(output);
				
				return CHECK_CONTINUE;											
			}
			return CHECK_FAILED;
		}

		private int checkEmpty(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.Empty).frequency && 
			    newPositionOutputs.Count == 1 && 
			    newPositionOutputs[0].outputDirList.Count == 1 && 
			    newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]))
			{
				newPositionOutputs.Clear();
				newPositionOutputs.Add(new QMazeOutput());

				List<QMazeOutput> currentOutputs = mazeArray[currentPosition.x][currentPosition.y];
				for (int i = 0; i < currentOutputs.Count; i++)
				{
					currentOutputs[i].outputDirList.Remove(dir);
				}

				return CHECK_BREAK;
			}
			return CHECK_FAILED;
		}
	}
}
