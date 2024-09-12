using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUIController : MonoBehaviour {
    [SerializeField]
    public TextMeshProUGUI currentHealthText;
    [SerializeField]
    private PlayerStats playerStats;

    private void Start() {
        currentHealthText.text = "HP:\n" + playerStats.getCurrentHealth();
    }

    private void OnEnable() {
        playerStats.currentHealthChangeEvent.AddListener(updateCurrentHealthText);
    }
    private void OnDisable() {
        playerStats.currentHealthChangeEvent.RemoveListener(updateCurrentHealthText);
    }

    private void updateCurrentHealthText(int health) {
        currentHealthText.text = "HP:\n" + health;
        GetComponent<Image>().fillAmount = health / 100f;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.K)) {
            playerStats.updateCurrentHealth(-5);
        }
        if (Input.GetKeyDown(KeyCode.L)) {
            playerStats.updateCurrentHealth(5);
        }
    }
}
