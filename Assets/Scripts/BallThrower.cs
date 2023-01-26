using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BallThrower : MonoBehaviour
{
    [Header("References")]
    public GameObject ball;
    public TextMeshProUGUI powerText;


    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0;
    public float throwForce;
    public float throwUpwordForce;

    float timeDown;
    float timePressed;
    bool keyPressed = false;

    bool readyToThrow;
    // Start is called before the first frame update
    void Start()
    {
        readyToThrow = true;
        UpdateThrowPowerText(0);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(throwKey) && readyToThrow) {
            keyPressed = true;
            timeDown = Time.time;
        }

        if(Input.GetKeyUp(throwKey) && readyToThrow) {
            keyPressed = false;
            timePressed = Time.time - timeDown;
            Throw(timePressed);
        }

        if (keyPressed) {
            UpdateThrowPowerText(Time.time - timeDown);
        }
        
    }


    private void Throw(float timePressed) {
        //readyToThrow = false;

        GameObject projectile = Instantiate(ball, transform.position, transform.rotation);

        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        Vector3 forceToAdd = (transform.forward * throwForce  + transform.up * throwUpwordForce) * timePressed;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);


    }

    void UpdateThrowPowerText(float power) {
        powerText.text = string.Format("{0:N2}", power);
    }
}
