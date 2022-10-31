using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragPointsController : MonoBehaviour
{
    public void DestryOnAnimEnd(){
        Destroy(gameObject.transform.parent.gameObject);
        Destroy(gameObject);
    }
}
