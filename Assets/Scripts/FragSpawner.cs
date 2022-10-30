using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OWS.ObjectPooling;

public class FragSpawner : MonoBehaviour
{
    public ObjectPool<PoolObject> fragPool;
    [SerializeField] private GameObject fragPrefab;

    private void Awake() {
        fragPool = new ObjectPool<PoolObject>(fragPrefab, null, DisableGravity, 30);
    }

    public GameObject GetNewFrag(Vector3 pos, Quaternion rot) {
        return fragPool.PullGameObject(pos, rot);
    }

    void DisableGravity(PoolObject p) {
        // p.gameObject.GetComponent<FragController>().DisableGravity();
        p.GetComponent<Rigidbody>().velocity = Vector3.zero;
        p.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        p.GetComponent<Rigidbody>().useGravity = false;
    }
}
