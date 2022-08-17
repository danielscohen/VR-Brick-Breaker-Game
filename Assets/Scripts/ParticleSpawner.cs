using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OWS.ObjectPooling;

public class ParticleSpawner : MonoBehaviour
{
    public ObjectPool<PoolObject> particlePool;
    [SerializeField] private GameObject particlePrefab;

    void Awake() {
        particlePool = new ObjectPool<PoolObject>(particlePrefab, null, DisableGravity, 50);
    }

    public GameObject GetNewParticle(Vector3 pos, Quaternion rot) {
        return particlePool.PullGameObject(pos, rot);
    }

    void DisableGravity(PoolObject p) {
        p.GetComponent<Rigidbody>().velocity = Vector3.zero;
        p.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        ResetAlpha(p);
    }

    void ResetAlpha(PoolObject p) {
        var mat = p.GetComponent<Renderer>().material;
        Color origColor = mat.color;
        origColor.a = 1f;
        mat.color = origColor;
    }
}
