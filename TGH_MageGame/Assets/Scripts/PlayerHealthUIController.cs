using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUIController : MonoBehaviour
{
    [SerializeField]
    public Text currentHealthText;
    [SerializeField]
    private PlayerStats playerStats;

    private void Start()
    {
        currentHealthText.text = "Current Health:\n" + playerStats.getCurrentHealth();
    }

    private void OnEnable()
    {
        playerStats.currentHealthChangeEvent.AddListener(updateCurrentHealthText);
    }
    private void OnDisable()
    {
        playerStats.currentHealthChangeEvent.RemoveListener(updateCurrentHealthText);
    }

    private void updateCurrentHealthText(int health)
    {
        currentHealthText.text = "Current Health:\n" + health;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            playerStats.updateCurrentHealth(-5);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            playerStats.updateCurrentHealth(5);
        }
    }
}
