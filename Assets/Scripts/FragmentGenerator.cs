using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentGenerator : MonoBehaviour
{
    Vector3 brickPos;
    Vector3 brickSize;
    Vector3 brickRotation;
    public Vector3 contactNormal;
    public float contactForce;
    public float gapModifier;

    const int NumFragmentsX = 10;
    const int NumFragmentsY = 10;
    const int NumFragmentsZ = 10;

    const float fragDelta = 0.5f;

    GameObject[,,] fragments;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnCollisionEnter(Collision other) {
        this.gameObject.SetActive(false);
        CreateFragments();

        Vector3 contactPos = other.GetContact(0).point;
        other.rigidbody.AddExplosionForce(5, contactPos, 0.1f);
        foreach (var fragment in fragments) {
            fragment.GetComponent<Rigidbody>().useGravity = true;
        }
    }


    void MakeAllFragsNonKin() {
        for (int x = 0; x < NumFragmentsX; x++)
        {
            for (int y = 0; y < NumFragmentsY; y++)
            {
                for (int z = 0; z < NumFragmentsZ; z++)
                {
                    GameObject frag = fragments[x, y, z];
                    Rigidbody fragRB = frag.GetComponent<Rigidbody>();
                    fragRB.isKinematic = false;
                }
            }

        }

    }



    void CreateFragments()
    {
        brickPos = transform.position;
        brickSize = transform.localScale;
        brickRotation = transform.eulerAngles;

        fragments = new GameObject[NumFragmentsX, NumFragmentsY, NumFragmentsZ];

        for (int x = 0; x < NumFragmentsX; x++)
        {
            for (int y = 0; y < NumFragmentsY; y++)
            {
                for (int z = 0; z < NumFragmentsZ; z++)
                {
                    CreateFragment(x, y, z);
                }
            }

        }

        //for (int x = 0; x < NumFragmentsX; x++)
        //{
        //    for (int y = 0; y < NumFragmentsY; y++)
        //    {
        //        for (int z = 0; z < NumFragmentsZ; z++)
        //        {
        //            ApplyRBToFrag(x, y, z);
        //        }
        //    }

        //}

    }

    void ApplyRBToFrag(int x, int y, int z) {
        GameObject frag = fragments[x, y, z];
        BoxCollider fragBC = frag.GetComponent<BoxCollider>();
        fragBC.size = new Vector3(1, 1, 1);
        frag.AddComponent<Rigidbody>();
        Rigidbody fragRB = frag.GetComponent<Rigidbody>();
        fragRB.isKinematic = false;
        fragRB.mass = 0.10f;
        fragRB.useGravity = false;

        //Debug.Log("Frag_" + x + "_" + y + "_" + z + ": ");

    }

    void CombineFrags() {
        
    }

    void CreateFragment(int x, int y, int z)
    {
        GameObject fragment = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fragment.name = "frag" + x + "_" + y + "_" + z;

        fragment.transform.localScale = new Vector3(brickSize.x / NumFragmentsX, brickSize.y / NumFragmentsY, brickSize.z / NumFragmentsZ);
        Vector3 gapDelta = fragment.transform.localScale * gapModifier;
        fragment.transform.position = new Vector3(brickPos.x - brickSize.x / 2 + x * gapDelta.x + (gapDelta.x / 2),
                                                    brickPos.y - brickSize.y / 2 + y * gapDelta.y + (gapDelta.y / 2),
                                                    brickPos.z - brickSize.z / 2 + z * gapDelta.z + (gapDelta.z / 2));
        fragment.transform.eulerAngles = brickRotation;
        fragment.AddComponent<Rigidbody>();
        Rigidbody fragRB = fragment.GetComponent<Rigidbody>();
        fragRB.isKinematic = false;
        fragRB.mass = 0.10f;
        fragRB.useGravity = false;
        fragments[x, y, z] = fragment;

    }

    GameObject GetClosestFragToImpact(Vector3 contactPos){
        GameObject closestFragToImpact = fragments[0,0,0];
        float closestDistToImpact = float.MaxValue;
        for (int x = 0; x < NumFragmentsX; x++)
        {
            for (int y = 0; y < NumFragmentsY; y++)
            {
                for (int z = 0; z < NumFragmentsZ; z++)
                {
                    GameObject frag = fragments[x,y,z];
                    float dist = Vector3.Distance(frag.transform.position, contactPos);
                    if(dist < closestDistToImpact){
                        closestDistToImpact = dist;
                        closestFragToImpact = frag;
                    }
                }
            }

        }
        
        return closestFragToImpact;
    }

    //void ApplyContactForce(){
    //    GameObject impactedFrag = GetClosestFragToImpact();
    //    Rigidbody fragRB = impactedFrag.GetComponent<Rigidbody>();
    //    fragRB.isKinematic = false;
    //    fragRB.AddForce(contactNormal, ForceMode.Impulse);
    //}
}
