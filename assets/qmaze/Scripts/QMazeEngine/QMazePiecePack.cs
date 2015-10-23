using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace qtools.qmaze
{
	[System.Serializable]
	[ExecuteInEditMode]
	public class QMazePiecePack: MonoBehaviour
	{
		[SerializeField] private QMazePiece[] mazePieceArray = new QMazePiece[20];
		[SerializeField] private bool inited = false;	

		private void Awake()
		{
			if (!inited)
			{
				inited = true;
			 
				addPiece(QMazePieceType.Empty, true, false, 0.05f, new QMazeOutput());
				addPiece(QMazePieceType.Line, true, true, 1, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N, QMazeOutputDirection.S }));
				addPiece(QMazePieceType.Deadlock, true, true, 1, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.E }));
				addPiece(QMazePieceType.Triple, true, false, 0.05f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N, QMazeOutputDirection.W, QMazeOutputDirection.S }));
				addPiece(QMazePieceType.Corner, true, true, 1, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.E, QMazeOutputDirection.S }));
				addPiece(QMazePieceType.Crossing, true, false, 0.05f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N, QMazeOutputDirection.E, QMazeOutputDirection.S, QMazeOutputDirection.W }));
				 
				addPiece(QMazePieceType.Start, false, true, 1, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N }));
				addPiece(QMazePieceType.Finish, false, true, 1, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N }));

				addPiece(QMazePieceType.DoubleCorner, false, false, 0.5f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N, QMazeOutputDirection.W }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.E, QMazeOutputDirection.S }));
				addPiece(QMazePieceType.Intersection, false, false, 0.2f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N, QMazeOutputDirection.S }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.W, QMazeOutputDirection.E }));

				addPiece(QMazePieceType.DeadlockCorner, false, false, 0.1f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.S }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.E }));
				addPiece(QMazePieceType.DeadlockLine, false, false, 0.1f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.S }));

				addPiece(QMazePieceType.DeadlockTriple, false, false, 0.1f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.E }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.W }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.S }));
				addPiece(QMazePieceType.DeadlockCrossing, false, false, 0.1f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.S }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.E }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.W }));

				addPiece(QMazePieceType.TripleDeadlock, false, false, 0.1f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N, QMazeOutputDirection.E, QMazeOutputDirection.S }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.W }));
				addPiece(QMazePieceType.LineDeadlock, false, false, 0.1f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N, QMazeOutputDirection.S }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.W }));

				addPiece(QMazePieceType.LineDeadlockLine, false, false, 0.1f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N, QMazeOutputDirection.S }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.E }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.W }));
				addPiece(QMazePieceType.CornerDeadlock1, false, false, 0.1f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.E, QMazeOutputDirection.S }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.W }));

				addPiece(QMazePieceType.CornerDeadlock2, false, false, 0.1f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.E, QMazeOutputDirection.S }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N }));
				addPiece(QMazePieceType.CornerDeadlockCorner, false, false, 0.1f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.E, QMazeOutputDirection.S }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.W }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N }));
			}
		}

		public QMazePiece getPiece(QMazePieceType type)
		{ 
			return mazePieceArray[(int)type];
		}

		public List<QMazePiece> toMazePieceList()
		{
			List<QMazePiece> result = new List<QMazePiece>();
			foreach (QMazePiece piece in mazePieceArray)	
			{
				checkGeometryList(piece);
				result.Add(piece);
			}
			return result;
		}

		private void checkGeometryList(QMazePiece piece)
		{
			List<GameObject> geometryList = piece.geometryList;
			for (int i = 0; i < geometryList.Count; i++)
			{
				if (geometryList[i] == null)				
				{
					geometryList.RemoveAt(i);
					i--;
				}
			}
		}

		private void addPiece(QMazePieceType type, bool require, bool use, float frequency, params QMazeOutput[] mazeOutput)
		{
			List<QMazeOutput> outputs = new List<QMazeOutput>();
			for (int i = 0; i < mazeOutput.Length; i++) outputs.Add(mazeOutput[i]);
			mazePieceArray[(int)type] = new QMazePiece(type, require, use, frequency, outputs);
		}
	}
}
