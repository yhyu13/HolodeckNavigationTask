using UnityEngine;
using UnityEditor;
using System.Collections;

namespace qtools.qmaze
{
	public class QInspectorUtils 
	{	
		public static T getAsset<T>(string filter) where T: Object
		{
			string[] guids = AssetDatabase.FindAssets(filter);
			foreach (string guid in guids)
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				if (path.Contains("QMaze")) return AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
			}
			return null;
		}
	}
}