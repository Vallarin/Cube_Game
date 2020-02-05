using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    byte[,,] cubeMap = new byte[CubeData.ChunkWidth, CubeData.ChunkHeight, CubeData.ChunkWidth];

    World world;

    void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();
        CreateMeshData();
        PopulateCubeMap();
        CreateMesh ();
    }

    void PopulateCubeMap()
    {
        for (int y = 0; y < CubeData.ChunkHeight; y++)
        {
            for (int x = 0; x < CubeData.ChunkWidth; x++)
            {
                for (int z = 0; z < CubeData.ChunkWidth; z++)
                {
                    if (y < 1)
                        cubeMap[x, y, z] = 1;
                    else if (y == CubeData.ChunkHeight - 1)
                        cubeMap[x, y, z] = 3;
                    else
                        cubeMap[x, y, z] = 1;
                }
            }
        }
    }

    void CreateMeshData()
    {
        for (int y = 0; y < CubeData.ChunkHeight; y++)
            {
                for (int x = 0; x < CubeData.ChunkWidth; x++)
                {
                    for (int z = 0; z < CubeData.ChunkWidth; z++)
                    {
                        AddCubeDataToChunk(new Vector3(x, y, z));
                    }
                }
            }            
    }

    bool CheckCube (Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);
        if (x < 0 || x > CubeData.ChunkWidth - 1 || y < 0 || y > CubeData.ChunkHeight - 1 || z < 0 || z > CubeData.ChunkWidth - 1)
        {
            return false;
        }
        return world.blocktypes[cubeMap[x, y, z]].isSolid;
    }

    void AddCubeDataToChunk (Vector3 pos)
    {
        for (int p = 0; p < 6; p++)
        {
            if (!CheckCube(pos + CubeData.faceChecks[p]))
            {

                byte blockID = cubeMap[(int)pos.x, (int)pos.y, (int)pos.z];

                vertices.Add(pos + CubeData.cubeVerts[CubeData.cubeTris[p, 0]]);
                vertices.Add(pos + CubeData.cubeVerts[CubeData.cubeTris[p, 1]]);
                vertices.Add(pos + CubeData.cubeVerts[CubeData.cubeTris[p, 2]]);
                vertices.Add(pos + CubeData.cubeVerts[CubeData.cubeTris[p, 3]]);

                AddTexture(world.blocktypes[blockID].GetTextureID(p));

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
                vertexIndex += 4;

            }                
        }
    }
        
    void CreateMesh ()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    void AddTexture (int textureID)
    {
        float y = textureID / CubeData.TextureAtlasSizeInBlocks;
        float x = textureID - (y * CubeData.TextureAtlasSizeInBlocks);
        x *= CubeData.NormalizedBlockTexture;
        y *= CubeData.NormalizedBlockTexture;
        y = 1f - y - CubeData.NormalizedBlockTexture;
        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + CubeData.NormalizedBlockTexture));
        uvs.Add(new Vector2(x + CubeData.NormalizedBlockTexture, y));
        uvs.Add(new Vector2(x + CubeData.NormalizedBlockTexture, y + CubeData.NormalizedBlockTexture));
    }

}