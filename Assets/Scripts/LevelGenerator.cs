using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] plataformaPrefabs;
    public RectTransform canvasArea; 

    private List<Bounds> plataformasCreadas = new List<Bounds>();

    void Start()
    {
        PlatformsGenerator();
    }

    void PlatformsGenerator()
    {
        if (plataformaPrefabs.Length == 0)
        {
            Debug.LogError("No hay prefabs asignados.");
            return;
        }

        // Obtener las dimensiones del RectTransform en espacio mundial
        Vector3[] worldCorners = new Vector3[4];
        canvasArea.GetWorldCorners(worldCorners);
        Vector2 min = worldCorners[0]; // Inferior izquierda
        Vector2 max = worldCorners[2]; // Superior derecha

        float gridSpacing = 3f;
        int maxPlataformas = 15;
        List<Vector2> posicionesUsadas = new List<Vector2>();

        int intentosMaximos = 100;
        int generadas = 0;

        while (generadas < maxPlataformas && intentosMaximos-- > 0)
        {
            float x = Mathf.Round(Random.Range(min.x, max.x) / gridSpacing) * gridSpacing;
            float y = Mathf.Round(Random.Range(min.y, max.y) / gridSpacing) * gridSpacing;
            Vector2 spawnPos = new Vector2(x, y);

            bool muyCerca = false;
            foreach (var pos in posicionesUsadas)
            {
                if (Vector2.Distance(pos, spawnPos) < gridSpacing)
                {
                    muyCerca = true;
                    break;
                }
            }

            if (muyCerca)
                continue;

            GameObject prefab = plataformaPrefabs[Random.Range(0, plataformaPrefabs.Length)];
            GameObject nuevaPlataforma = Instantiate(prefab, spawnPos, Quaternion.identity);

            //Asegurarse de que esté en la Sorting Layer correcta
            SpriteRenderer[] renderers = nuevaPlataforma.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sr in renderers)
            {
                sr.sortingLayerName = "Platforms"; 
                sr.sortingOrder = 0;
            }

            posicionesUsadas.Add(spawnPos);
            generadas++;
        }
    }
}
