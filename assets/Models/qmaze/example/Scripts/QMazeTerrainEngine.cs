using UnityEngine;
using System.Collections;
using qtools.qmaze;

namespace qtools.qmaze.example
{
	[ExecuteInEditMode]
	public class QMazeTerrainEngine : MonoBehaviour 
	{
		public Material terrainMaterial;
		public GameObject ground;
		public int scale = 2;
		public int padding = 5;
		public float edgeHeight = 0.25f;
		public float minHeight  = 0.0f;
		public float maxHeight  = 1.0f;
		public float noiseScale = 25f;
		public bool create = false;

		private GameObject terrain;
		private Mesh terrainMesh;

		public void clear()
		{
			if (terrainMesh != null)
			{
				terrainMesh.Clear();
				DestroyImmediate(terrainMesh);
				terrainMesh = null;
				DestroyImmediate(terrain);
			}
		}

		public void Update()
		{
			if (create)
			{
				create = false;
				QMazeEngine me = FindObjectOfType<QMazeEngine>();
				generateTerrain(me.mazeWidth, me.mazeHeight, me.mazePieceWidth, me.mazePieceHeight);
			}
		}

		public void generateTerrain(float mazeWidthF, float mazeHeightF, float mazePieceWidth, float mazePieceHeight)
	    {
			GameObject groundGO = (GameObject)Instantiate(ground);
			groundGO.transform.parent = transform;
			groundGO.transform.localPosition = new Vector3(mazeWidthF * mazePieceWidth / 2.0f - mazePieceWidth / 2, 
			                                               0, 
			                                               - mazeHeightF * mazePieceHeight / 2.0f + mazePieceHeight / 2);
            groundGO.transform.localScale = new Vector3(mazeWidthF, 1, mazeHeightF);
            
            int sizeX = Mathf.FloorToInt(mazeWidthF  / scale);
	        int sizeY = Mathf.FloorToInt(mazeHeightF / scale);
			if (sizeX < 1) sizeX = 1;
			if (sizeY < 1) sizeY = 1;
			sizeX += padding * 2;
			sizeY += padding * 2;
	        Color edgeColor = new Color(1, 1, 1, 1);
	        Color mainColor = new Color(0, 0, 0, 0);

	        Vector3[] terrainVertices   = new Vector3[(sizeX + 1) * (sizeY + 1)];
	        Vector2[] terrainUV         = new Vector2[(sizeX + 1) * (sizeY + 1)];
	        int    [] terrainTriangles  = new int    [(sizeX) * (sizeY) * 6 - (sizeX - padding * 2) * (sizeY - padding * 2) * 6];
	        Color  [] terrainVerticesColor = new Color[(sizeX + 1) * (sizeY + 1)];

	        float   height;
	        int     curTriangle = 0;
	        int     curVertex = 0;
	        Vector2 perlinSource = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));

	        for (int ix = 0; ix <= sizeX; ix++)
	        {
	            for (int iy = 0; iy <= sizeY; iy++)
	            {
	                if (((ix == padding || ix == sizeX - padding) && (iy >= padding && iy <= sizeY - padding)) || 
	                    ((iy == padding || iy == sizeY - padding) && (ix >= padding && ix <= sizeX - padding))) 
	                {
						height = edgeHeight;
	                    terrainVerticesColor[curVertex] = edgeColor;
	                }
	                else 
	                {
						height = minHeight + (maxHeight - minHeight) * Mathf.PerlinNoise(perlinSource.x + (float)ix / sizeX * noiseScale, perlinSource.y + (float)iy / sizeY * noiseScale);
	                    terrainVerticesColor[curVertex] = mainColor;
	                }

	                terrainVertices[curVertex] = new Vector3(ix - padding, height, -iy + padding);
	                terrainUV[curVertex] = new Vector2(terrainVertices[curVertex].x, terrainVertices[curVertex].z);
	                curVertex++;

	                if (ix != 0 && iy != 0 && !((ix > padding && ix <= sizeX - padding) && (iy > padding && iy <= sizeY - padding)))
	                {
	                    terrainTriangles[curTriangle + 0] = (ix - 1) * (sizeY + 1) + (iy - 1);
	                    terrainTriangles[curTriangle + 1] = (ix - 0) * (sizeY + 1) + (iy - 1);
	                    terrainTriangles[curTriangle + 2] = (ix - 0) * (sizeY + 1) + (iy - 0);

	                    terrainTriangles[curTriangle + 3] = (ix - 1) * (sizeY + 1) + (iy - 1);
	                    terrainTriangles[curTriangle + 4] = (ix - 0) * (sizeY + 1) + (iy - 0);
	                    terrainTriangles[curTriangle + 5] = (ix - 1) * (sizeY + 1) + (iy - 0);

	                    curTriangle += 6;
	                }
	            }
	        } 

	        terrainMesh = new Mesh();
	        terrainMesh.vertices = terrainVertices;
	        terrainMesh.colors = terrainVerticesColor;
	        terrainMesh.uv = terrainUV;
	        terrainMesh.triangles = terrainTriangles;
	        terrainMesh.RecalculateNormals();

			terrain = new GameObject("QTerrain");
	        terrain.transform.parent = transform;
	        terrain.transform.localScale = new Vector3(mazeWidthF * mazePieceWidth / (sizeX - padding * 2), 1, mazeHeightF * mazePieceHeight / (sizeY - padding * 2));
			terrain.transform.localPosition = new Vector3(- mazePieceWidth / 2, 0, mazePieceHeight / 2);

			MeshFilter terrainMeshFilter = terrain.AddComponent<MeshFilter>(); 
			terrainMeshFilter.mesh = terrainMesh;

	        terrain.AddComponent<MeshRenderer>();
	        terrain.GetComponent<Renderer>().material = terrainMaterial;        
	    }
	}
}
