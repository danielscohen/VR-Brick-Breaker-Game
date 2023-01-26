using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OWS.ObjectPooling;

public class VoxelSpawner : MonoBehaviour
{
    public ObjectPool<PoolObject> voxelPool;
    [SerializeField] private GameObject voxelPrefab;

    private void Awake() {
        voxelPool = new ObjectPool<PoolObject>(voxelPrefab, 100);
    }

    public GameObject GetNewVoxel() {
        return voxelPool.PullGameObject();
    }

    void DetachFromParent(PoolObject obj) {
        if (obj.gameObject.transform.parent != null) {
            obj.gameObject.transform.parent = null;
        }
    }


}
