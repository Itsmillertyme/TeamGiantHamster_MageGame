using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatTester : MonoBehaviour
{

    [SerializeField] private Canvas canvas;
    [SerializeField] private Text text;

    public PlayerStats playerStats;

    bool isActive = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            isActive = !isActive;
            canvas.enabled = isActive;
        }
    }

    public void levelUp()
    {
        playerStats.updateExperience(playerStats.getExperienceForNextLevel());
    }
    public void levelUpFiveTimes()
    {
        for (int i = 0; i < 5; i++)
        {
            playerStats.updateExperience(playerStats.getExperienceForNextLevel());
        }
    }
    public void gain100Exp()
    {
        playerStats.updateExperience(100);
    }
    public void gainFiveBonusHealthFree()
    {
        playerStats.updateBonusHealth(5);
    }
    public void spendSkillPointsOnFiveBonusHealth()
    {
        playerStats.spendSkillpoint();
        playerStats.updateBonusHealth(5);
    }
    public void gainFiveBonusManaFree()
    {
        playerStats.updateBonusMana(5);
    }
    public void spendSkillPointsOnFiveBonusMana()
    {
        playerStats.spendSkillpoint();
        playerStats.updateBonusMana(5);
    }
    public void gainFiveBonusAttackDamageFree()
    {
        playerStats.updateBonusAttackDamage(5);
    }
    public void spendSkillPointsOnFiveAttackDamage()
    {
        playerStats.spendSkillpoint();
        playerStats.updateBonusAttackDamage(5);
    }
    public void gainFiveBonusAttackSpeedFree()
    {
        playerStats.updateBonusAttackSpeed(5);
    }
    public void spendSkillPointsOnFiveAttackSpeed()
    {
        playerStats.spendSkillpoint();
        playerStats.updateBonusAttackSpeed(5);
    }
    public void gainFiveBonusDefenceFree()
    {
        playerStats.updateBonusDefence(5);
    }
    public void spendSkillPointsOnFiveDefence()
    {
        playerStats.spendSkillpoint();
        playerStats.updateBonusDefence(5);
    }
    public void gainFiveBonusMovementSpeedFree()
    {
        playerStats.updateBonusMovementSpeed(5);
    }
    public void spendSkillPointsOnFiveMovementSpeed()
    {
        playerStats.spendSkillpoint();
        playerStats.updateBonusMovementSpeed(5);
    }
    public void takeFiveDamage()
    {
        playerStats.updateCurrentHealth(-5);
    }
    public void healFiveDamage()
    {
        playerStats.updateCurrentHealth(5);
    }
    public void loseFiveMana()
    {
        playerStats.updateCurrentMana(-5);
    }
    public void restoreFiveMana()
    {
        playerStats.updateCurrentMana(5);
    }
    public void resetStatsToDefault()
    {
        playerStats.resetToDefault();
    }
    public void updateText()
    {
        text.text = "Level: " + playerStats.getLevel() +"\nSkill Points: " + playerStats.getSkillPoints();
        text.text += "\nExperience: " + playerStats.getExperience() + "\nExp. For Next Level: " + playerStats.getExperienceForNextLevel();
        text.text += "\nMax Health: " + playerStats.getMaxHealth() + "\nCurrent Health: " + playerStats.getCurrentHealth();
        text.text += "\nMax Mana: " + playerStats.getMaxMana() + "\nCurrent Mana: " + playerStats.getCurrentMana();
        text.text += "\nMax Attack Damage: " + playerStats.getMaxAttackDamage() + "\nMax Attack Speed: " + playerStats.getMaxAttackSpeed();
        text.text += "\nMax Defence: " + playerStats.getMaxDefence() + "\nMax Movement Speed: " + playerStats.getMaxMovementSpeed();
    }
}
