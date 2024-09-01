using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellBook : MonoBehaviour
{
    [SerializeField] private List<Spell> spellBook;
    [SerializeField] private int activeSpell;

    private float scrollValue; // driven

    void Update()
    {
        scrollValue = Input.mouseScrollDelta.y;

        if (Input.GetKeyDown(KeyCode.L))
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
        spellBook[activeSpell].Cast();
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
    }


    // update active spell
    // shoot using enum to determine method
}
