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
    [SerializeField] GameObject[] _wallLayers;

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
        foreach (GameObject layer in _wallLayers) {
            GameObject wall =  Instantiate(wallPrefab);
            NumBricksRemaining += wall.GetComponent<WallBuilderScript>().BuildWall();
            wall.transform.parent = layer.transform;
            wall.transform.localPosition = Vector3.zero;
            wall.transform.localScale = wallSize;
            walls.Add(wall);
        }
    }

}
