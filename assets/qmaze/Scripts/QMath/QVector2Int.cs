using System;
using UnityEngine;

namespace qtools.qmaze
{
	[System.Serializable]
	public class QVector2Int
	{
		public int x;
		public int y;
		
		public QVector2Int(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public bool equal(QVector2Int otherPoint)
	    {
	        return x == otherPoint.x && y == otherPoint.y;
	    }

		public QVector2Int clone()
	    {
			return new QVector2Int(x, y); 
	    }

	    public void set(int nx, int ny)
	    {
	        this.x = nx;
	        this.y = ny;
	    }

	    public Vector2 toVector2()
	    {
	        return new Vector2(x, y);
	    }

	    public float sqrMagnitude(QVector2Int otherPoint)
	    {
	        float tx = x - otherPoint.x;
	        float ty = y - otherPoint.y;
	        return tx * tx + ty * ty;
	    }
	}
}