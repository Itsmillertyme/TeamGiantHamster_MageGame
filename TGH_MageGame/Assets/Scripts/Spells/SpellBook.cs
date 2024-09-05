using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpellBook : MonoBehaviour
{
    [SerializeField] private List<Spell> spellBook;
    [SerializeField] private int activeSpell;

    [SerializeField] private Transform spawnPosition;

    [SerializeField] private UnityEvent ActiveSpellSwitched;

    // GETTERS
    public int ActiveSpell => activeSpell; 

    // DRIVEN
    #region DRIVEN
    private float scrollValue;
    #endregion

    private void Start()
    {
        Debug.LogWarning("Spellbook scrolling is backwards");
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
        spellBook[activeSpell].Cast(spawnPosition);
    }

    // untested
    private void SetSpell()
    {
        // if 
        if (scrollValue > 0f)
        {
            activeSpell++;

            if (activeSpell >= spellBook.Count)
            {
                activeSpell = 0;
            }
        }

        else if (scrollValue < 0f)
        {
            activeSpell--;

            if (activeSpell < 0)
            {
                activeSpell = spellBook.Count - 1;
            }
        }

        // RAISE AN EVENT THAT THE SPELL SELECTION HAS CHANGED
        ActiveSpellSwitched.Invoke();
    }

    public string GetSpellUIData()
    {
        return $"{spellBook[activeSpell].Name}";
    }
    // update active spell
    // shoot using enum to determine method
}
