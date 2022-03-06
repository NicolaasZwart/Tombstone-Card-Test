using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;


public class ItemLog : MonoBehaviour
{

    public void OnClick()
    {
        
        foreach (Item card in DialogueManager.masterDatabase.items) 
        {
            
            if (card.FieldExists("isCard"))
            {              
                
                int bonus1Value = card.LookupInt("bonus1Value");
                int bonus2Value = card.LookupInt("bonus2Value");

                string bonus1Attribute = card.LookupValue("bonus1Attribute");
                bonus1Attribute = bonus1Attribute.Remove(0, 15);
                string bonus2Attribute = card.LookupValue("bonus2Attribute");
                bonus2Attribute = bonus2Attribute.Remove(0, 15);

                string cardType = card.LookupValue("cardType");
                cardType = cardType.Remove(0, 8);

                Debug.Log ($"Card: {card.Name}, Bonuses: {bonus1Value} {bonus1Attribute}, {bonus2Value} {bonus2Attribute}, Type: {cardType}");   
            } 
        }
    }
}
