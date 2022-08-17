using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    [SerializeField] int MaxNumWallLayers = 4;
    [SerializeField] float distBetweenLayers = 2f;
    [SerializeField] Vector3 wallSize;
    [SerializeField] Vector3 wallRotation;
    [SerializeField] GameObject wallPrefab;

    List<GameObject> walls = new List<GameObject>();

    void CreateWalls() {
        int numWalls = Mathf.Min(MaxNumWallLayers, (int)(transform.localScale.z / (wallSize.z + distBetweenLayers)));
        for (int i = 0; i < numWalls; i++) {
            GameObject wall =  Instantiate(wallPrefab, this.transform);
            wall.GetComponent<WallBuilderScript>().BuildWall(new Vector3(0, 0, i * (wallSize.z + distBetweenLayers)), wallSize, wallRotation);
            walls.Add(wall);
        }

    }
}
