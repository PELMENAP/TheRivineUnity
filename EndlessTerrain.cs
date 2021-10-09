using UnityEngine;
using System.Collections.Generic;
public class EndlessTerrain : MonoBehaviour
{

    const float scale = 5;

    const float viewerMoveThresholdForChunkUpdate = 40f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;
    public static float maxViewDst = 40;
    public Transform viewer;
    public Material mapMaterial;
    public Mesh meshChunk;
    public GameObject[] objnorm;

    public static Vector2 viewerPosition;
    Vector2 viewerPositionOld;
    static MapGenerator mapGenerator;
    int chunkSize;
    int chunksVisibleInViewDst;
    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
        UpdateVisibleChunks();
    }

    void Update()
    {
        viewerPosition = new Vector2(-viewer.position.x, viewer.position.y) / scale;

        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    void UpdateVisibleChunks()
    {

        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform, mapMaterial, meshChunk, objnorm));
                }

            }
        }
    }

    public class TerrainChunk
    {
        GameObject[] norm;
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        MapData mapData;
        bool mapDataReceived;

        public TerrainChunk(Vector2 coord, int size, Transform parent, Material material, Mesh mesh, GameObject[] objnorm)
        {
            norm = objnorm;
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(-position.x, position.y, 0);
            meshObject = new GameObject("Terrain Chunk");
            meshObject.transform.Rotate(new Vector3(-90, 0, 0));
            meshObject.transform.position = positionV3 * scale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * scale * 2;
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            SetVisible(false);
            mapGenerator.RequestMapData(position, OnMapDataReceived);
        }
        public void PlaceObject(int[,] objectMap, int width, int height, Vector3 place, GameObject parent)
        {
            GameObject land = new GameObject("Landmass");
            land.transform.localPosition = place;
            land.transform.SetParent(parent.transform);
            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    if (norm[objectMap[x, y]] != null)
                    {
                        GameObject chunkobj = Instantiate(norm[objectMap[x, y]], place, Quaternion.identity);
                        if (objectMap[x, y] == 4)
                        {
                            chunkobj.transform.localPosition = chunkobj.transform.position + new Vector3(-x * scale + UnityEngine.Random.Range(-3, 3), -y * scale + UnityEngine.Random.Range(-3, 3), 0);
                            goto End;
                        }
                        chunkobj.transform.localPosition = chunkobj.transform.position + new Vector3(-x * scale, -y * scale, 0);
                    End:
                        chunkobj.transform.SetParent(land.transform);
                    }
                }
            }
            land.transform.localPosition = new Vector3(5, 0, 5);
        }

        void OnMapDataReceived(MapData mapData)
        {
            this.mapData = mapData;
            mapDataReceived = true;
            Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colourMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
            meshRenderer.material.mainTexture = texture;
            PlaceObject(mapData.objectMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize, meshObject.transform.position, meshObject);
            UpdateTerrainChunk();
        }
        public void UpdateTerrainChunk()
        {

            if (mapDataReceived)
            {
                float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewerDstFromNearestEdge <= maxViewDst;

                if (visible)
                {
                    terrainChunksVisibleLastUpdate.Add(this);
                }

                SetVisible(visible);
            }
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }
    }
}