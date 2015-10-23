using UnityEngine;
using System.Collections.Generic;

namespace qtools.qmaze
{
	[System.Serializable]
	public enum QMazeOutputDirection
	{
		N, S, W, E, None
	}

	[System.Serializable]
	public class QMazeOutput
	{ 
	    public static Dictionary<QMazeOutputDirection, int> dx = new Dictionary<QMazeOutputDirection,int>() { { QMazeOutputDirection.N, 0 }, { QMazeOutputDirection.E, 1 }, { QMazeOutputDirection.S, 0 }, { QMazeOutputDirection.W, -1 } };
	    public static Dictionary<QMazeOutputDirection, int> dy = new Dictionary<QMazeOutputDirection,int>() { { QMazeOutputDirection.N,-1 }, { QMazeOutputDirection.E, 0 }, { QMazeOutputDirection.S, 1 }, { QMazeOutputDirection.W,  0 } };  	
	    public static Dictionary<QMazeOutputDirection, QMazeOutputDirection> opposite  = new Dictionary<QMazeOutputDirection, QMazeOutputDirection>() { { QMazeOutputDirection.N, QMazeOutputDirection.S }, { QMazeOutputDirection.E, QMazeOutputDirection.W }, { QMazeOutputDirection.S, QMazeOutputDirection.N }, { QMazeOutputDirection.W, QMazeOutputDirection.E } };	
	    public static Dictionary<QMazeOutputDirection, QMazeOutputDirection> rotateCW  = new Dictionary<QMazeOutputDirection, QMazeOutputDirection>() { { QMazeOutputDirection.N, QMazeOutputDirection.E }, { QMazeOutputDirection.E, QMazeOutputDirection.S }, { QMazeOutputDirection.S, QMazeOutputDirection.W }, { QMazeOutputDirection.W, QMazeOutputDirection.N } };
	    public static Dictionary<QMazeOutputDirection, QMazeOutputDirection> rotateCCW = new Dictionary<QMazeOutputDirection, QMazeOutputDirection>() { { QMazeOutputDirection.N, QMazeOutputDirection.W }, { QMazeOutputDirection.W, QMazeOutputDirection.S }, { QMazeOutputDirection.S, QMazeOutputDirection.E }, { QMazeOutputDirection.E, QMazeOutputDirection.N } };
		
		[SerializeField] public List<QMazeOutputDirection> outputDirList;

		public QMazeOutput()
		{
			outputDirList = new List<QMazeOutputDirection>();
		}

		public QMazeOutput(List<QMazeOutputDirection> direction)
	    {
			outputDirList = direction;
	    }
		
	    public static QMazeOutput getShuffleOutput()
	    {
	        QMazeOutput mazeOutput = new QMazeOutput();
	        mazeOutput.outputDirList = new List<QMazeOutputDirection>();
	        mazeOutput.outputDirList.Add(QMazeOutputDirection.N);
	        mazeOutput.outputDirList.Add(QMazeOutputDirection.E);
	        mazeOutput.outputDirList.Add(QMazeOutputDirection.S);
	        mazeOutput.outputDirList.Add(QMazeOutputDirection.W);
	        QListUtil.Shuffle(mazeOutput.outputDirList);
	        return mazeOutput;
	    }
	}
}