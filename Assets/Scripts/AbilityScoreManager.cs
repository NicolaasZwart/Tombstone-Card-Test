using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using CardStatsNamespace;

public class AbilityScoreManager : MonoBehaviour
{
    [Header("Ability Score Value Fields")]
    [SerializeField] TextMeshProUGUI agilityValue;
    [SerializeField] TextMeshProUGUI enduranceValue;
    [SerializeField] TextMeshProUGUI charismaValue;
    [SerializeField] TextMeshProUGUI mindValue;

    void Awake()
    {
        
    }

    void OnEnable()
    {
        AbilityCheckManager.PopulatePlayerScores += PopulatePlayerScores;
    }

    void PopulatePlayerScores(List<CardStatEntry> cardStatList, bool isPlayer)
    {
        
        if (isPlayer)
        {
            foreach (var CardStatEntry in cardStatList)
            {
                switch (CardStatEntry.AbilityName)
                {
                    case "Agility":
                        agilityValue = CardStatEntry.AbilityBonus + 10)ToString());


                           // }" + $" {cardAttribute1.AbilityName}");
                        break;

                    case 0:
                        attribute1Text.text = ("");
                        break;

                    case < 0:
                        attribute1Text.text = ($"{cardAttribute1.AbilityBonus}" + $" {cardAttribute1.AbilityName}");
                        break;
                }
            }
            
        }
    }

    void OnDisable()
    {
        AbilityCheckManager.PopulatePlayerScores -= PopulatePlayerScores;
    }


}
