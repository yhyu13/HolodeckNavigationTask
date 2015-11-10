using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;

namespace qtools.qmaze
{
	[System.Serializable]
	public enum QMazePieceType
	{
		Empty = 0, 
		Line = 1,
		Deadlock = 2,
		Triple = 3,
		Corner = 4,
		Crossing = 5,
		Start = 6,
		Finish = 7,
		DoubleCorner = 8,
		Intersection = 9,
		DeadlockCorner = 10,
		DeadlockLine = 11,
		DeadlockTriple = 12,
		DeadlockCrossing = 13,
		TripleDeadlock = 14,
		LineDeadlock = 15,
		LineDeadlockLine = 16,
		CornerDeadlock1 = 17,
		CornerDeadlock2 = 18,
		CornerDeadlockCorner = 19
	}

	[System.Serializable]
	public class QMazePiece
	{
		// PUBLIC
		[SerializeField] public QMazePieceType type;	
		[SerializeField] public bool require;
		[SerializeField] public bool use;
		[SerializeField] public float frequency;
		[SerializeField] public List<GameObject> geometryList = new List<GameObject>();
		[SerializeField] public List<QMazeOutput> outputList;	

		private float rotation;	

		// CONSTRUCTOR
		public QMazePiece(QMazePieceType type, bool require, bool use, float frequency, List<QMazeOutput> outputList)
		{
			this.type = type;
			this.require = require;
			this.use = use;
			this.frequency = frequency;
			this.outputList = outputList;
		}

		// PUBLIC
		public bool checkFit(List<QMazeOutput> sourceOutputs)
		{
			if (sourceOutputs == null)
			{
				if (outputList.Count > 0) return false;
				else return true;
			}

			rotation = 0;
			for (int i = 0; i < 4; i++)
			{
				if (check(sourceOutputs)) return true;
				rotation += 90;
				rotate(sourceOutputs);
			}
			return false;
		}

		public float getRotation()		
		{
			return rotation;
		}

		// PRIVATE
	    private void rotate(List<QMazeOutput> sourceOutputs)
	    {
	        int sourceOutputsCount = sourceOutputs.Count;
	        for (int i = 0; i < sourceOutputsCount; i++)
	        {
	            QMazeOutput sourceOutput = sourceOutputs[i];
	            List<QMazeOutputDirection> directions = sourceOutput.outputDirList;
	            int directionCount = directions.Count;
	            for (int j = 0; j < directionCount; j++)
	            {
	                directions[j] = QMazeOutput.rotateCW[directions[j]];
	            }
	        }
	    }

	    private bool check(List<QMazeOutput> sourceOutputs)
	    {
			if (outputList.Count != sourceOutputs.Count) return false;

	        int found = 0;
	        int outputCount = outputList.Count;
	        int sourceOutputsCount = sourceOutputs.Count;
	        for (int oi = 0; oi < outputCount; oi++)
	        {
	            List<QMazeOutputDirection> outputDirections = outputList[oi].outputDirList;
	            for (int si = 0; si < sourceOutputsCount; si++)
	            {
	                List<QMazeOutputDirection> sourceOutputDirections = sourceOutputs[si].outputDirList;
	                if (outputDirections.Count == sourceOutputDirections.Count)
	                {
	                    int contains = 0;
	                    int outputDirectionsCount = outputDirections.Count;
	                    for (int di = 0; di < outputDirectionsCount; di++)
	                    {
	                        if (outputDirections.Contains(sourceOutputDirections[di])) contains++;
	                    }
	                    if (contains == outputDirectionsCount) found++;
	                }
	            }
	        }

	        if (found == outputList.Count) return true;
	        else return false;
	    }
	}
}