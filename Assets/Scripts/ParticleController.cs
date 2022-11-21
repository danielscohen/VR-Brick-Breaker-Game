using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    Rigidbody particleRb;
    Vector3 playerPos;
    [SerializeField] float Gravity = 1.0f;
    [SerializeField] float duration = 0.4f;

    void Awake() {
        particleRb = GetComponent<Rigidbody>();
    }

    public void Init(Vector3 playerPos) {
        this.playerPos = playerPos;
    }

    // void FixedUpdate() {
    //     Vector3 dir = (playerPos - transform.position).normalized;
    //     particleRb.AddForce(dir * Gravity);

    // }
    public IEnumerator FadeOut() {
        Material mat = GetComponent<Renderer>().material;
        float time = 0f;
        Color startColor = mat.color;
        Color endColor = startColor;
        endColor.a = 0;
       
        while(time < duration) {
                mat.color = Color.Lerp(startColor, endColor, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
