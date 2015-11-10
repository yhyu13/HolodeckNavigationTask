using UnityEngine;
using System.Collections;

namespace qtools.qmaze.example
{
	public class QMazeSelector : MonoBehaviour 
	{
		private static QMazeSelector instance;
		public static QMazeSelector getInstance()
		{
			return instance;
		}
		
		void Start () 
		{
			instance = this;
		}
	}
}