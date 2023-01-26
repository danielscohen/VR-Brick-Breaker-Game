using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilFunctions 
{

    const int rangeMin = 1;
    const int rangeMax = 100;
    public static int CalcDistScore(float dist, Vector3 brickSize) {
        float maxDist = Mathf.Sqrt(brickSize.x * brickSize.x + brickSize.y * brickSize.y + brickSize.z * brickSize.z);
        return (int)((rangeMax - rangeMin) * ((dist - 0) / (maxDist - 0)) + rangeMin);
    }

    public static Vector3 RectToSphereCoordinates(Vector3 rectC) {
        Vector3 sphereC = new Vector3();
        sphereC.x = Mathf.Sqrt(rectC.x * rectC.x + rectC.y * rectC.y + rectC.z * rectC.z);
        sphereC.y = Mathf.Atan2(rectC.x, rectC.z);
        sphereC.z = Mathf.Acos(rectC.y / sphereC.x);
        return sphereC;
    }
    public static Vector3 FindClosestFracPt(Vector3 pos, List<Vector3> fracPts) {
        float minDist = float.MaxValue;
        Vector3 minPt = Vector3.zero;
        foreach (Vector3 fracPt in fracPts) {
            var dist = Vector3.Distance(pos, fracPt);
            if (dist < minDist) {
                minDist = dist;
                minPt = fracPt;
            }
        }
        return minPt;
    }

}
