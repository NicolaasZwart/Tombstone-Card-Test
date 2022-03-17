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

    public void PopulateOnClick()
    {
        Debug.Log("Fire");   
        CardInventoryTest();
    }

    void CardInventoryTest()
    {
        LuaTableWrapper bellboyCards = DialogueLua.GetActorField("Bellboy", "NPC Hand").asTable;

        foreach (var card in bellboyCards.values)
        {
            Debug.Log((card as LuaTableWrapper)["Name"]);
        }
    }
}
