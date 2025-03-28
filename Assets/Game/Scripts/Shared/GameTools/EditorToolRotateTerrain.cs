using UnityEditor;
using UnityEngine;

namespace Shared.GameTools
{
public class EditorToolRotateTerrain : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Game Tools/Rotate Terrain 90 Deg")]
    static void RotateTerrain90Degrees()
    {
        // Get the active terrain
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null)
        {
            Debug.LogError("No active terrain found!");
            return;
        }

        TerrainData terrainData = terrain.terrainData;

        // Rotate Heightmap
        RotateHeightmap(terrainData);

        // Rotate Splatmap (Textures)
        RotateSplatmap(terrainData);

        // Rotate Trees
        RotateTrees(terrain, terrainData);

        // Rotate Detail Layers (Grass, etc.)
        RotateDetailLayers(terrainData);

        Debug.Log("Terrain rotated by 90 degrees!");
    }

    static void RotateHeightmap(TerrainData terrainData)
    {
        int width  = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;

        float[,] heights    = terrainData.GetHeights(0, 0, width, height);
        float[,] newHeights = new float[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                newHeights[height - 1 - y, x] = heights[x, y];
            }
        }

        terrainData.SetHeights(0, 0, newHeights);
    }

    static void RotateSplatmap(TerrainData terrainData)
    {
        int width  = terrainData.alphamapWidth;
        int height = terrainData.alphamapHeight;
        int layers = terrainData.alphamapLayers;

        float[,,] alphaMaps    = terrainData.GetAlphamaps(0, 0, width, height);
        float[,,] newAlphaMaps = new float[width, height, layers];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int l = 0; l < layers; l++)
                {
                    newAlphaMaps[height - 1 - y, x, l] = alphaMaps[x, y, l];
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, newAlphaMaps);
    }

    static void RotateTrees(Terrain terrain, TerrainData terrainData)
    {
        TreeInstance[] trees       = terrainData.treeInstances;
        Vector3        terrainSize = terrainData.size;

        for (int i = 0; i < trees.Length; i++)
        {
            Vector3 position = trees[i].position;
            position = new Vector3(1 - position.z, position.y, position.x);

            // Adjust height to match the new position
            position.y = terrainData.GetInterpolatedHeight(position.x, position.z) / terrainSize.y;

            trees[i].position = position;
        }

        terrainData.treeInstances = trees;
    }

    static void RotateDetailLayers(TerrainData terrainData)
    {
        int detailWidth  = terrainData.detailWidth;
        int detailHeight = terrainData.detailHeight;
        int layers       = terrainData.detailPrototypes.Length;

        for (int l = 0; l < layers; l++)
        {
            int[,] detailLayer    = terrainData.GetDetailLayer(0, 0, detailWidth, detailHeight, l);
            int[,] newDetailLayer = new int[detailWidth, detailHeight];

            for (int y = 0; y < detailHeight; y++)
            {
                for (int x = 0; x < detailWidth; x++)
                {
                    newDetailLayer[detailHeight - 1 - y, x] = detailLayer[x, y];
                }
            }

            terrainData.SetDetailLayer(0, 0, l, newDetailLayer);
        }
    }
#endif
}
}