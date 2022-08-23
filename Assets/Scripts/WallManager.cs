using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    [SerializeField] int MaxNumWallLayers = 4;
    [SerializeField] float distBetweenLayers = 2f;
    [SerializeField] float distPlayerFirstWall = 2f;
    [SerializeField] Vector3 wallSize;
    [SerializeField] Vector3 wallRotation;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject[] wallBounds;

    List<GameObject> walls = new List<GameObject>();

    int _numBricksRemaining = 0;
    public int NumBricksRemaining {
        get { return _numBricksRemaining; }
        set {
            _numBricksRemaining = value;
            if(_numBricksRemaining == 0) {
                GameController.Instance.EndGame(GameOverReason.GameWon);
            }
        }
    }

    void Start() {
        CreateWalls();
    }

    void CreateWalls() {
        int numWalls = Mathf.Min(MaxNumWallLayers, (int)((transform.localScale.z - distPlayerFirstWall) / (wallSize.z + distBetweenLayers)));
        for (int i = 0; i < numWalls; i++) {
            GameObject wall =  Instantiate(wallPrefab);
            NumBricksRemaining += wall.GetComponent<WallBuilderScript>().BuildWall(new Vector3(0, 0, i * (wallSize.z + distBetweenLayers) + distPlayerFirstWall), wallSize, wallRotation);
            wall.transform.parent = transform;
            wall.transform.localPosition = new Vector3(0, 0, i * (wallSize.z + distBetweenLayers));
            walls.Add(wall);
        }

    }

}
