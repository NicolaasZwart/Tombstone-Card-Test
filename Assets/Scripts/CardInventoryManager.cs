using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class CardInventoryManager : MonoBehaviour
{
    // This script gets the card inventory for a selected actor.
    void Start()
    {
        PixelCrushers.DialogueSystem.Articy.ArticyTools.InitializeLuaSubtables();
    }

    void CardInventoryTest()
    {
        //var cardInventoryTable = DialogueLua.GetItemField("cardName", "agilityBonus", "enduranceBonus", "charismaBonus", "mindBonus", "cardType").AsTable;
    }

    void Update()
    {
        
    }
}
