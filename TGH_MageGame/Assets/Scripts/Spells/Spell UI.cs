using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class SpellUI : MonoBehaviour
{
    [SerializeField] private TMP_Text spellBookUIText;
    [SerializeField] private SpellBook SpellBook;

    private void Start()
    {
        GetSpellUIData();
    }
    public void GetSpellUIData()
    {
        int spellIndex = SpellBook.ActiveSpell;
        spellBookUIText.text = $"Spell {SpellBook.ActiveSpell.ToString()}: {SpellBook.GetSpellUIData()}";
    }
}