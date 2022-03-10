using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using System.Linq;

public class LINQTester : MonoBehaviour
{
    
    public void TestLINQOnClick()
    {
        TestLINQ();
    }

    // LINQ card organizing experiment
    public void TestLINQ()
    {
        IEnumerable<Item> allCards =
            from card in DialogueManager.masterDatabase.items
            where card.FieldExists("isCard") == true
            select card;

        var sortedCards =
            from card in allCards
            group card by card.LookupValue("cardType");

        foreach (var cardGroup in sortedCards)
        {      
            Debug.Log(cardGroup.Key);
            foreach (Item card in cardGroup)
            {
                Debug.Log(card.Name);
            }
        }
    }  
}
