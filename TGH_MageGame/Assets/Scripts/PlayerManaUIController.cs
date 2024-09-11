using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerManaUIController : MonoBehaviour
{
    [SerializeField]
    public Text currentManaText;
    [SerializeField]
    private PlayerStats playerStats;

    private void Start()
    {
        currentManaText.text = "Current Mana:\n" + playerStats.getCurrentMana();
    }

    private void OnEnable()
    {
        playerStats.currentManaChangeEvent.AddListener(updateCurrentManaText);
    }
    private void OnDisable()
    {
        playerStats.currentManaChangeEvent.RemoveListener(updateCurrentManaText);
    }

    private void updateCurrentManaText(int mana)
    {
        currentManaText.text = "Current Mana:\n" + mana;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            playerStats.updateCurrentMana(-5);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            playerStats.updateCurrentMana(5);
        }
    }
}
