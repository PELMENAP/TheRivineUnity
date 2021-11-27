using UnityEngine;
using System.Collections.Generic;
public class EndlessTerrain : MonoBehaviour
{

    const int scale = 5;
    const float sqrViewerMoveThresholdForChunkUpdate = 18f;
    public static float maxViewDst = 15;
    public Transform viewer;
    public Material mapMaterial;
    public Mesh meshChunk;
    public Objnorm[] objnorm;
    public GameObject reflectWater;
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
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform, mapMaterial, meshChunk, objnorm, reflectWater));
                }

            }
        }
    }

    public class TerrainChunk
    {
        Objnorm[] objnorm;
        GameObject refWater;
        GameObject meshObject;
        GameObject mapFragment;
        Vector2 position;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        MapData mapData;
        bool mapDataReceived;

        public TerrainChunk(Vector2 coord, int size, Transform parent, Material material, Mesh mesh, Objnorm[] objs, GameObject reflectWater)
        {
            refWater = reflectWater;
            objnorm = objs;
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            meshObject = new GameObject("Terrain Chunk");
            mapFragment = new GameObject("Map Fragment");
            meshObject.transform.Rotate(new Vector2(-90, 0));
            mapFragment.transform.Rotate(new Vector2(-90, 0));
            meshObject.transform.position = new Vector2((int)-position.x, (int)position.y) * scale;
            mapFragment.transform.position = new Vector3((int)-position.x, (int)position.y, 20) * scale;
            meshObject.transform.parent = mapFragment.transform.parent = parent;
            meshObject.transform.localScale = mapFragment.transform.localScale = Vector3.one * scale * 2;
            meshRenderer = mapFragment.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
            meshFilter = mapFragment.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            SetVisible(false);
            mapGenerator.RequestMapData(position, OnMapDataReceived);
        }
        public void PlaceObject(int[,] objectMap, int width, int height, Vector2Int place, GameObject parent)
        {
            int j = 0;
            GameObject land = new GameObject("Landmass");
            GameObject liquid = new GameObject("Water");
            land.transform.localPosition = liquid.transform.localPosition = new Vector2(place.x, place.y);
            land.transform.parent = liquid.transform.parent = parent.transform;
            land.isStatic = liquid.isStatic = true;
            if (objectMap[0, 0] < 2 && objectMap[20, 20] < 2 && objectMap[0, 20] < 2 && objectMap[20, 0] < 2 && objectMap[10, 0] < 2 && objectMap[0, 10] < 2 && objectMap[10, 10] < 2 && objectMap[20, 10] < 2 && objectMap[10, 20] < 2)
            {
                GameObject water = Instantiate(refWater, liquid.transform.position + new Vector3(2.5f, 0), Quaternion.identity);
                water.transform.SetParent(parent.transform);
                Destroy(liquid);
                j++;
            }
            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    Objnorm listObj = objnorm[objectMap[x, y]];
                    if (listObj.objList != null)
                    {
                        for (int i = objectMap[x, y] < 2 ? j : 0; i < listObj.objList.Length; i++)
                        {
                            if (UnityEngine.Random.Range(0, 100) < listObj.chance[i])
                            {
                                GameObject chunkobj = Instantiate(listObj.objList[i], new Vector2((int)(place.x + -x * scale), (int)(place.y + -y * scale)), Quaternion.identity);
                                if (objectMap[x, y] < 2 && i == 0)
                                {
                                    chunkobj.transform.SetParent(liquid.transform);
                                    chunkobj.transform.localPosition += new Vector3(0, -2.5f, 0);
                                }
                                else
                                {
                                    if (listObj.spread[i])
                                    {
                                        chunkobj.transform.localPosition += new Vector3(UnityEngine.Random.Range(-3, 3), UnityEngine.Random.Range(-3, 3));
                                        Destroy(chunkobj.GetComponent<WaterReflectableScript>());
                                    }
                                    chunkobj.transform.SetParent(land.transform);
                                }
                                chunkobj.isStatic = true;
                            }
                        }
                    }
                }
            }
            land.transform.localPosition = liquid.transform.localPosition = new Vector3(5, 0, 5);
            // SimpleSpriteCombine lol = land.AddComponent<SimpleSpriteCombine>();
            // lol.CombineSprites();
        }

        void OnMapDataReceived(MapData mapData)
        {
            this.mapData = mapData;
            mapDataReceived = true;
            Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colourMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
            meshRenderer.material.mainTexture = texture;
            PlaceObject(mapData.objectMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize, new Vector2Int((int)meshObject.transform.position.x, (int)meshObject.transform.position.y), meshObject);
            meshObject.isStatic = mapFragment.isStatic = true;
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
    [System.Serializable]
    public struct Objnorm
    {
        public GameObject[] objList;
        public int[] chance;
        public bool[] spread;
    }
}
