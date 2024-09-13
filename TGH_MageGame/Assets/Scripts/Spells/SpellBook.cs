using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpellBook : MonoBehaviour
{
    [Header("Spell List")]
    [SerializeField] private List<Spell> spellBook;

    [Header("Spawn Position")]
    [SerializeField] private Transform spawnPosition;

    [Header("Unity Events")]
    [SerializeField] private UnityEvent ActiveSpellSwitched;

    [Header("Player Stats")]
    [SerializeField] PlayerStats playerStats;

    [Header("Mouse Position Tracker")]
    [SerializeField] MousePositionTracking mousePositionTracker;

    // GETTERS
    public int ActiveSpell => activeSpell;

    // DRIVEN
    #region DRIVEN
    [Header("Debugging")]
    private float scrollValue;
    private Coroutine castDelay;
    private bool isReadyToCast;
    private int activeSpell;
    #endregion

    private void Start()
    {
        isReadyToCast = true;
    }

    void Update()
    {
        scrollValue = Input.mouseScrollDelta.y;

        // TEMP WORKAROUND UNTIL INPUT SYSTEM METHOD IS PRESENT
        if (Input.GetKeyDown(KeyCode.L) || Input.GetMouseButtonDown(0))
        {
            Cast();
        }
        if (scrollValue != 0) 
        { 
            SetSpell(); 
        }
    }

    private void Cast()
    {
        spellBook[activeSpell].SetTargetPosition(mousePositionTracker.GetMousePosition());
        if (isReadyToCast && playerStats.getCurrentMana() >= spellBook[activeSpell].ManaCost)
        {
            spellBook[activeSpell].Cast(spawnPosition);
            playerStats.updateCurrentMana(-spellBook[activeSpell].ManaCost);
            castDelay = StartCoroutine(CastDelay());
        }
    }

    public IEnumerator CastDelay()
    {
        isReadyToCast = false;
        yield return new WaitForSeconds(spellBook[ActiveSpell].CastDelayTime);
        isReadyToCast = true;
    }

    // SPELL INVENTORY CYCLING
    private void SetSpell()
    {
        // if 
        if (scrollValue < 0f)
        {
            do
            {
                activeSpell++;

                if (activeSpell >= spellBook.Count)
                {
                    activeSpell = 0;
                }
            }
            while (!spellBook[activeSpell].IsUnlocked);
        }

        else if (scrollValue > 0f)
        {
            do
            {
                activeSpell--;

                if (activeSpell < 0)
                {
                    activeSpell = spellBook.Count - 1;
                }
            }
            while (!spellBook[activeSpell].IsUnlocked) ;
        }

        // RAISE AN EVENT THAT THE SPELL SELECTION HAS CHANGED
        ActiveSpellSwitched.Invoke();
    }

    // GETS ACTIVE SPELL TO USE IN UI TEXT
    public string GetSpellUIData()
    {
        return $"{spellBook[activeSpell].Name}";
    }
    // update active spell
    // shoot using enum to determine method

    // GETTER FOR ACTIVE SPELL ANIMATION
    public AnimationClip GetSpellAnimation()
    {
        return spellBook[activeSpell].CastAnimation;
    }
}
