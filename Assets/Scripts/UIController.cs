using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    int playerHealth;
    [SerializeField] TextMeshProUGUI playerHealthText;
    void OnEnable() {
        PlayerCollider.fragCollideWithPlayer += DecreasePlayerHealthText;
        
    }

    void OnDisable() {
        PlayerCollider.fragCollideWithPlayer -= DecreasePlayerHealthText;
    }

    void Start() {
        playerHealth = GameController.Instance.PlayerHealth;
        playerHealthText.text = playerHealth.ToString();
    }

    void DecreasePlayerHealthText(int healthDecVal) {
        playerHealth -= healthDecVal;
        playerHealthText.text = playerHealth.ToString();
    }
}
