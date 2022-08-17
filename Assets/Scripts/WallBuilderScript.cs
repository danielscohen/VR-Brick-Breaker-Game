using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBuilderScript : MonoBehaviour
{
    class Brick {
        public int id;
        public int posX;
        public int posY;
        public int width;
        public int height;

        public Brick(int id, int posX, int posY, int width, int height) {
            this.id = id;
            this.posX = posX;
            this.posY = posY;
            this.width = width;
            this.height = height;
        }
    }


    [SerializeField] Vector3Int numBricks;
    [SerializeField] Vector3Int numVoxels;
    [SerializeField] GameObject brickPreFab;
    VoxelSpawner voxSpawner;
    [SerializeField] Material voxInternalMat;
    [SerializeField] Material voxEdgeMat;

    int[,] wallMap;


    private void Update() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            transform.position = new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            transform.Rotate(new Vector3(0, 15f, 0));
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            transform.Rotate(new Vector3(0, -15f, 0));
        }
    }

    private void Awake() {
        voxSpawner = GameObject.Find("Voxel Spawner").GetComponent<VoxelSpawner>();
    }


    public void BuildWall(Vector3 wallPos, Vector3 wallSize, Vector3 wallRotation) {
        InitWallMap();

        int brickId = 0;
        List<Brick> bricks = new List<Brick>();

        for (int x = 0; x < numBricks.x; x++) {
            for (int y = 0; y < numBricks.y; y++) {
                int r = Random.Range(0, 3);
                int width = 1;
                int height = 1;
                if(wallMap[x,y] == -1) {
                    wallMap[x, y] = brickId;
                    if (r == 0 && x < numBricks.x - 1) {
                        wallMap[x + 1, y] = brickId;
                        width++;
                    } else if (r == 1 && y < numBricks.y - 1 && wallMap[x, y+1] == -1) {
                        wallMap[x, y + 1] = brickId;
                        height++;
                    }
                    bricks.Add(new Brick(brickId, x, y, width, height));
                    brickId++;
                } else {
                    Brick brick = bricks.Find(b => b.id == wallMap[x, y]);
                    if (r == 0 && x < numBricks.x - 1 && x > 0 && wallMap[x - 1, y] == wallMap[x, y]) {
                        wallMap[x + 1, y] = wallMap[x, y];
                        brick.width++;
                    } else if (r == 1 && y < numBricks.y - 1 && y > 0 && wallMap[x, y - 1] == wallMap[x, y] && wallMap[x, y+1] == -1) {
                        wallMap[x, y + 1] = wallMap[x, y];
                        brick.height++;
                    }
                }
            }
        }



        foreach (var b in bricks) {
            //Vector3 bottomLeftWallPos = transform.TransformPoint(new Vector3(-0.5f, -0.5f, -0.5f));
            Vector3 bottomLeftWallPos = new Vector3(-0.5f, -0.5f, -0.5f);
            Vector3 scale = new Vector3(1f / numBricks.x, 1f / numBricks.y, 1f);
            float posX = bottomLeftWallPos.x + scale.x * (b.posX + 0.5f * b.width);
            float posY = bottomLeftWallPos.y + scale.y * (b.posY + 0.5f * b.height);
            //GameObject brick = Instantiate(brickPreFab, new Vector3(posX, posY, bottomLeftWallPos.z), transform.rotation, transform);
            GameObject brick = Instantiate(brickPreFab);
            brick.transform.parent = transform;
            brick.transform.localPosition = new Vector3(posX, posY, 0);
            brick.transform.localScale = Vector3.Scale(scale, new Vector3(b.width, b.height, 1));
            brick.GetComponent<Renderer>().material.color = new Color32((byte)Random.Range(0, 256), (byte)Random.Range(0, 256), (byte)Random.Range(0, 256), 255);
            CreateVoxels(brick);
        }

        transform.position = wallPos;
        transform.localScale = wallSize;
        transform.Rotate(wallRotation);

    }

    void InitWallMap() {
        wallMap = new int[numBricks.x, numBricks.y];
        for (int x = 0; x < numBricks.x; x++) {
            for (int y = 0; y < numBricks.y; y++) {
                wallMap[x, y] = -1;
            }
        }
    }
    void CreateVoxels(GameObject brick)
    {
        GameObject[,,] voxels = new GameObject[numVoxels.x, numVoxels.y, numVoxels.z];

        for (int x = 0; x < numVoxels.x; x++)
        {
            for (int y = 0; y < numVoxels.y; y++)
            {
                for (int z = 0; z < numVoxels.z; z++)
                {
                    voxels[x,y,z] = MakeVoxel(x, y, z, brick);
                }
            }

        }

        brick.GetComponent<BrickFrag>().voxels = voxels;
    }
    GameObject MakeVoxel(int x, int y, int z, GameObject brick) {

        Vector3 bottomLeftWallPos = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 scale = new Vector3(1f / numVoxels.x, 1f / numVoxels.y, 1f / numVoxels.z);
        float posX = bottomLeftWallPos.x + scale.x * (x + 0.5f);
        float posY = bottomLeftWallPos.y + scale.y * (y + 0.5f);
        float posZ = bottomLeftWallPos.z + scale.z * (z + 0.5f);
        GameObject voxel = voxSpawner.GetNewVoxel();

        voxel.transform.parent = brick.transform;
        voxel.transform.localPosition = new Vector3(posX, posY, posZ);
        voxel.transform.localScale = scale;
        voxel.name = "vox_" + x + "_" + y + "_" + z;
        ColorVoxelGrad(x,y,z,voxel, brick);
        voxel.GetComponent<Collider>().enabled = false;
        voxel.GetComponent<Renderer>().enabled = false;
        return voxel;
    }

    void ColorVoxelGrad(GameObject voxel, GameObject brick) {
        float distToCenter = Vector3.Distance(voxel.transform.TransformPoint(Vector3.zero), brick.transform.TransformPoint(Vector3.zero));
        float cornerCenterDist = Vector3.Distance(brick.transform.TransformPoint(new Vector3(0.5f, 0.5f, 0.5f)), brick.transform.TransformPoint(Vector3.zero));
        voxel.GetComponent<Renderer>().material.color = Color.Lerp(Color.black, brick.GetComponent<Renderer>().material.color, Mathf.Pow(distToCenter / cornerCenterDist, 1f / 6f));
    }

    void ColorVoxelGrad(int x, int y, int z, GameObject voxel, GameObject brick) {
        float midX = (float)(numVoxels.x - 1) / 2f;
        float midY = (float)(numVoxels.y - 1) / 2f;
        float midZ = (float)(numVoxels.z - 1) / 2f;

        float colorPer = Mathf.Max((float)Mathf.Abs(midX - x) / (midX + 1), (float)Mathf.Abs(midY - y) / (midY + 1), (float)Mathf.Abs(midZ - z) / (midZ + 1));
        voxel.GetComponent<Renderer>().material.color = Color.Lerp(Color.black, brick.GetComponent<Renderer>().material.color, colorPer);



    }

}


