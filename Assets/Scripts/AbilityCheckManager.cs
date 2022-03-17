using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CardStatsNamespace;

public class AbilityCheckManager : MonoBehaviour
{
    [SerializeField] GameObject playerScoreArea;
    [SerializeField] TMP_Dropdown checkType;

    int p_agVal = 10;
    int p_endVal = 10;
    int p_chaVal = 10;
    int p_miVal = 10;

    int npc_agVal = 10;
    int npc_endVal = 10;
    int npc_chaVal = 10;
    int npc_miVal = 10;

    bool playerWon = false;

    public delegate void TriggerAbilityCheck();

    // Ability score populating event declarations.
    public delegate void PScoresUIManager(int p_agVal, int p_endVal, int p_chaVal, int p_miVal, bool isPlayer);
    public static event PScoresUIManager PopulatedPScores;

    public delegate void NPCScoresUIManager(int npc_agVal, int npc_endVal, int npc_chaVal, int npc_miVal, bool isPlayer);
    public static event NPCScoresUIManager PopulatedNPCScores;

    public delegate void CheckOutcome(bool playerWon);
    public static event CheckOutcome DeterminedCheckOutcome;


HandBuilder handBuilder;


    void OnEnable()
    {
        HandBuilder.ResetPScores += ResetPlayerScores;
        HandBuilder.ResetNpcScores += ResetNPCScores;
        HandBuilder.p_AbilityMath += p_CheckedMath;
        HandBuilder.npc_AbilityMath += npc_CheckedMath;
        HandBuilder.TriggeredAbilityCheck += abilityCheck;
    }

    public void ResetPlayerScores()
    {
        this.p_agVal = 10;
        this.p_endVal = 10;
        this.p_chaVal = 10;
        this.p_miVal = 10;
    }

    public void ResetNPCScores()
    {
        this.npc_agVal = 10;
        this.npc_endVal = 10;
        this.npc_chaVal = 10;
        this.npc_miVal = 10;
    }

    public void p_CheckedMath(List<CardStatEntry> cardStatList, bool isPlayer)
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
                        Debug.Log("The p_ScoresMath switch defaulted.");
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
    }

    public void npc_CheckedMath(List<CardStatEntry> cardStatList, bool isPlayer)
    {
        if (!isPlayer)
        {
            foreach (var CardStatEntry in cardStatList)
            {
                switch (CardStatEntry.AbilityName)
                {
                    case "Agility":
                        npc_agVal += CardStatEntry.AbilityBonus;
                        break;

                    case "Endurance":
                        npc_endVal += CardStatEntry.AbilityBonus;
                        break;

                    case "Charisma":
                        npc_chaVal += CardStatEntry.AbilityBonus;
                        break;

                    case "Mind":
                        npc_miVal += CardStatEntry.AbilityBonus;
                        break;

                    default:
                        Debug.Log("The npc_ScoresMath switch defaulted.");
                        break;
                }
            }
        }
        else
        {
            Debug.Log("The bool isPlayer == false did not reach the npc_ScoresMath method.");
        }

        // This is an event which shares the NPC ability checks with AbilityUIManager.
        if (!isPlayer)
        {
            if (PopulatedNPCScores != null)
            {
                PopulatedNPCScores.Invoke(npc_agVal, npc_endVal, npc_chaVal, npc_miVal, isPlayer);
            }
        }

        List<CardStatEntry> npc_scoresFinal = new List<CardStatEntry>(); // WE MIGHT BE ABLE TO LOSE THESE LISTS
        cardStatList.Add(new CardStatEntry() { AbilityBonus = npc_agVal, AbilityName = "Agility" });
        cardStatList.Add(new CardStatEntry() { AbilityBonus = npc_endVal, AbilityName = "Endurance" });
        cardStatList.Add(new CardStatEntry() { AbilityBonus = npc_chaVal, AbilityName = "Charisma" });
        cardStatList.Add(new CardStatEntry() { AbilityBonus = npc_miVal, AbilityName = "Mind" });
    }

    void abilityCheck()
    {
        switch (checkType.options[checkType.value].text)
        {
            case "Agility Check":
                if (npc_agVal < p_agVal)
                {
                    playerWon = true;
                }
                else
                {
                    playerWon = false;
                }
                break;

            case "Endurance Check":
                if (npc_endVal < p_endVal)
                {
                    playerWon = true;
                }
                else
                {
                    playerWon = false;
                }
                break;

            case "Charisma Check":
                if (npc_chaVal < p_chaVal)
                {
                    playerWon = true;
                }
                else
                {
                    playerWon = false;
                }
                break;

            case "Mind Check":
                if (npc_miVal < p_miVal)
                {
                    playerWon = true;
                }
                else
                {
                    playerWon = false;
                }
                break;
            
            default:
                Debug.Log("The abilityCheck switch defaulted.");
                break;
        }
        
        DeterminedCheckOutcome.Invoke(playerWon);
    }

    void OnDisable()
    {
        HandBuilder.ResetPScores -= ResetPlayerScores;
        HandBuilder.ResetNpcScores -= ResetNPCScores;
        HandBuilder.p_AbilityCheck -= p_ScoresMath;
        HandBuilder.npc_AbilityCheck -= npc_ScoresMath;
        HandBuilder.TriggeredAbilityCheck -= abilityCheck;
    }
}
