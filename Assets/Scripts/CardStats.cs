using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
using CardStatsNamespace;

public class CardStats : MonoBehaviour   
{
    
    [Header("CardGeneric TMPro text fields")]
    [SerializeField] TextMeshProUGUI cardTitleText;
    [SerializeField] TextMeshProUGUI attribute1Text;
    [SerializeField] TextMeshProUGUI attribute2Text;
    [SerializeField] TextMeshProUGUI cardTypeText;
    
    bool attributeSpecial = false;
    bool attributeAll = false;

    public void PopulateCardText(string playerCardName, List<CardStatEntry> cardStatList, string cardType)
    {
        // sets up the "special" and "all" attribute bools
        if (cardType == "Psionic")
        {
            Debug.Log("Psionic Card");
            attributeSpecial = true;
        }
        else
        {
            attributeSpecial = false;
        }
        
        if (cardStatList.ElementAt(0).AbilityBonus != 0 
            && cardStatList.ElementAt(1).AbilityBonus != 0 
            && cardStatList.ElementAt(2).AbilityBonus != 0 
            && cardStatList.ElementAt(3).AbilityBonus != 0)
        {
            attributeAll = true;
        }
        else
        {
            attributeAll = false;
        }
        
        // Gives new game object clone card a name, and puts that name in the title text box.
        cardTitleText.text = playerCardName;

        // Applies bonus values to card.
        IEnumerable<CardStatEntry> cardStatSortedEnumerable = from statEntry in cardStatList
                   //where statEntry.AbilityBonus != 0 // Removes any entries in cardStatList with Ability Bonuses equal to zero. (might not be using this.)
                   orderby statEntry.AbilityBonus 
                   select statEntry;
        List<CardStatEntry> cardStatSorted = cardStatSortedEnumerable.ToList();
     
        CardStatEntry cardAttribute1 = cardStatSorted.Last();
        CardStatEntry cardAttribute2 = cardStatSorted.First();

        switch (cardAttribute1.AbilityBonus)
            {
                case > 0:
                    attribute1Text.text = ($"+{cardAttribute1.AbilityBonus}" + $" {cardAttribute1.AbilityName}");
                    break;

                case 0:
                    attribute1Text.text = ("");
                    break;

                case < 0:
                    attribute1Text.text = ($"{cardAttribute1.AbilityBonus}" + $" {cardAttribute1.AbilityName}");
                    break;
            }

        switch (cardAttribute2.AbilityBonus)
            {
                case > 0:
                    attribute2Text.text = ($"+{cardAttribute2.AbilityBonus}" + $" {cardAttribute2.AbilityName}");
                    break;

                case 0:
                    attribute2Text.text = ("");
                    break;

                case < 0:
                    attribute2Text.text = ($"{cardAttribute2.AbilityBonus}" + $" {cardAttribute2.AbilityName}");
                    break;
            }
        
        if (cardAttribute2.AbilityBonus < 0 && cardAttribute1.AbilityBonus == 0)
        {
            attribute1Text.text = ($"{cardAttribute2.AbilityBonus}" + $" {cardAttribute2.AbilityName}");
            attribute2Text.text = ("");
        }

        // "All" attribute.
        if (attributeAll)
        {
            if (cardAttribute1.AbilityBonus > 0)
            {
                attribute1Text.text = ($"+{cardAttribute1.AbilityBonus} All Attributes");
                attribute2Text.text = "";
            }
            else if (cardAttribute1.AbilityBonus < 0)
            {
                attribute1Text.text = ($"{cardAttribute2.AbilityBonus} All Attributes");
                attribute2Text.text = "";
            }
        }

        // "Special" psionic attribute.
        if (attributeSpecial)
        {
            attribute1Text.text = "Special";
            attribute2Text.text = "";
        }

        // Card type.
        cardTypeText.text = ($"{cardType}");

        // Card color.
        switch (cardType)
        {
            case "Class":
                GetComponent<Image>().color = new Color32(35, 81, 232, 255);
                break;

            case "Background":
                GetComponent<Image>().color = new Color32(51, 51, 51, 255);
                break;

            case "Quest":
                GetComponent<Image>().color = new Color32(156, 0, 0, 255);
                break;

            default:
                GetComponent<Image>().color = new Color32(51, 51, 51, 255);
                break;
        }



    }
}
