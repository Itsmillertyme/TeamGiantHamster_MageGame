using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // The base stats the player initially will have at the start of a run.
    public int baseHealth = 100;
    public int baseAttackDamage = 5;
    public int baseAttackSpeed = 5;
    public int baseDefence = 5;
    public int baseMovementSpeed = 5;

    // Stores the total equipment modifier that will be added to each stat for the final calculations.
    /*
     * Waiting to see how we handle equipment before doing anything with this
     * since depending on how we handle equipment this could drastically change.
     */

    // Stores the amount stats upgrade on level up.
    public int healthUpgrade = 5;
    public int attackDamageUpgrade = 1;
    public int attackSpeedUpgrade = 1;
    public int defenceUpgrade = 1;
    public int movementSpeedUpgrade = 1; // It may be better to have movement speed upgrades be intentional only instead of every level up increasing movement speed.

    // Keeps track of current level, experience, and skill points.
    int level = 1;
    int experience = 0;
    int experienceForNextLevel = 100;
    int skillPoints = 0;

    // Stores the bonus amount of each stat.
    int bonusHealth = 0;
    int bonusAttackDamage = 0;
    int bonusAttackSpeed = 0;
    int bonusDefence = 0;
    int bonusMovementSpeed = 0;

    // These variables will store the maximum possible of stats at a given moment.
    int maxHealth;
    int maxAttackDamage;
    int maxAttackSpeed;
    int maxDefence;
    int maxMovementSpeed;

    // Stores the current health
    int currentHealth;

    private void Start()
    {
        // initialzes the max value of each stat at the beginning of the game.
        maxHealth = baseHealth;
        maxAttackDamage = baseAttackDamage;
        maxAttackSpeed = baseAttackSpeed;
        maxDefence = baseDefence;
        maxMovementSpeed = baseMovementSpeed;
        currentHealth = maxHealth;
    }

    // Methods that will be called whenever the maximum value of a stat should change
    void updateMaxHealth()
    {
        int oldMax = maxHealth;
        maxHealth = baseHealth + (healthUpgrade * (level - 1)) + bonusHealth;

        if (currentHealth == oldMax)
        {
            currentHealth = maxHealth;
        }
    }
    void updateMaxAttackDamage()
    {
        maxAttackDamage = baseAttackDamage + (attackDamageUpgrade * (level - 1)) + bonusAttackDamage;
    }
    void updateMaxAttackSpeed()
    {
        maxAttackSpeed = baseAttackSpeed + (attackSpeedUpgrade * (level - 1)) + bonusAttackSpeed;
    }
    void updateMaxDefence()
    {
        maxDefence = baseDefence + (defenceUpgrade * (level - 1)) + bonusDefence;


        // Right now the idea for defence is that it will reduce a percent of damage so 100 will be the max since that will reduce 100% of damage
        if (maxDefence < 100)
        {
            maxDefence = 100;
        }
    }
    void updateMaxMovementSpeed()
    {
        maxMovementSpeed = baseMovementSpeed + (movementSpeedUpgrade * (level - 1)) + bonusMovementSpeed;
    }

    // Methods for applying the bonus from the skill tree or other sources
    void updateBonusHealth(int bonus)
    {
        bonusHealth += bonus;
        updateMaxHealth();
    }
    void updateBonusAttackDamage(int bonus)
    {
        bonusAttackDamage += bonus;
        updateMaxAttackDamage();
    }
    void updateBonusAttackSpeed(int bonus)
    {
        bonusAttackSpeed += bonus;
        updateMaxAttackSpeed();
    }
    void updateBonusDefence(int bonus)
    {
        bonusDefence += bonus;
        updateMaxDefence();
    }
    void updateBonusMovementSpeed(int bonus)
    {
        bonusMovementSpeed += bonus;
        updateMaxMovementSpeed();
    }
    // The value of amount will be equal to the amount healed or damage dealt
    void updateCurrentHealth(int amount)
    {
        if (amount >= 0)
        {
            currentHealth += amount;
        }
        else if (amount < 0)
        {
            {
                currentHealth += (maxDefence / 100) * amount;
            }
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }

        // The function for updating experience and levels
        void updateExperience(int exp)
        {
            experience += exp;

            if (experience >= experienceForNextLevel)
            {
                experience = experience - experienceForNextLevel;
                experienceForNextLevel = experienceForNextLevel * 2;

                level++;
                skillPoints += 3;

                // Calls function again if there is enough exp for another level up

                if (experience >= experienceForNextLevel)
                {
                    updateExperience(0);
                }
            }

            updateMaxHealth();
            updateMaxAttackDamage();
            updateMaxAttackSpeed();
            updateMaxDefence();
            updateMaxMovementSpeed();
        }
    }
}
