using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CardStatsNamespace;

public class AbilityCheckManager : MonoBehaviour
{
    [SerializeField] GameObject playerScoreArea;

    int p_agVal = 10;
    int p_endVal = 10;
    int p_chaVal = 10;
    int p_miVal = 10;

    // Ability score populating event declarations.
    public delegate void PScoresUIManager(int p_agVal, int p_endVal, int p_chaVal, int p_miVal, bool isPlayer);
    public static event PScoresUIManager PopulatedPScores;

    void OnEnable()
    {
        HandBuilder.ResetPScores += ResetPlayerScores;
        HandBuilder.p_AbilityCheck += p_ScoresMath;  
    }

    public void ResetPlayerScores()
    {
        this.p_agVal = 10;
        this.p_endVal = 10;
        this.p_chaVal = 10;
        this.p_miVal = 10;
    }

    public void p_ScoresMath(List<CardStatEntry> cardStatList, bool isPlayer)
    {
        if (isPlayer)
        {
            foreach (var CardStatEntry in cardStatList)
            {
                switch (CardStatEntry.AbilityName)
                {
                    case "Agility":
                        p_agVal += CardStatEntry.AbilityBonus;
                        break;

                    case "Endurance":
                        p_endVal += CardStatEntry.AbilityBonus;
                        break;

                    case "Charisma":
                        p_chaVal += CardStatEntry.AbilityBonus;
                        break;

                    case "Mind":
                        p_miVal += CardStatEntry.AbilityBonus;
                        break;

                    default:
                        Debug.Log("The PopulatePlayerScores switch defaulted.");
                        break;
                }
            }
        }
        else
        {
            Debug.Log("The bool isPlayer == true did not reach the PopulatePlayerScores method.");
        }

        // This is an event which shares the player ability checks with AbilityUIManager.
        if (isPlayer)
        {
            if (PopulatedPScores != null)
            {
                PopulatedPScores.Invoke(p_agVal, p_endVal, p_chaVal, p_miVal, isPlayer);
            }
        }

        List<CardStatEntry> p_scoresFinal = new List<CardStatEntry>();
        cardStatList.Add(new CardStatEntry() { AbilityBonus = p_agVal, AbilityName = "Agility" });
        cardStatList.Add(new CardStatEntry() { AbilityBonus = p_endVal, AbilityName = "Endurance" });
        cardStatList.Add(new CardStatEntry() { AbilityBonus = p_chaVal, AbilityName = "Charisma" });
        cardStatList.Add(new CardStatEntry() { AbilityBonus = p_miVal, AbilityName = "Mind" });

        // We need something here to compare the player's scores
        // with the NPC.
    }

    void OnDisable()
    {
        HandBuilder.ResetPScores -= ResetPlayerScores;
        HandBuilder.p_AbilityCheck -= p_ScoresMath;
    }
}
