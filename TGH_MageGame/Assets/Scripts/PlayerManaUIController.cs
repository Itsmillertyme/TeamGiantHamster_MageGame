using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerManaUIController : MonoBehaviour {
    [SerializeField]
    public TextMeshProUGUI currentManaText;
    [SerializeField]
    private PlayerStats playerStats;

    private void Start() {
        currentManaText.text = "Mana:\n" + playerStats.getCurrentMana();
    }

    private void OnEnable() {
        playerStats.currentManaChangeEvent.AddListener(updateCurrentManaText);
    }
    private void OnDisable() {
        playerStats.currentManaChangeEvent.RemoveListener(updateCurrentManaText);
    }

    private void updateCurrentManaText(int mana) {
        currentManaText.text = "Mana:\n" + mana;
        GetComponent<Image>().fillAmount = mana / 100f;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.N)) {
            playerStats.updateCurrentMana(-5);
        }
        if (Input.GetKeyDown(KeyCode.M)) {
            playerStats.updateCurrentMana(5);
        }
    }
}
