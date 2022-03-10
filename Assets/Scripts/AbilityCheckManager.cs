using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CardStatsNamespace;

public class AbilityCheckManager : MonoBehaviour
{
    [SerializeField] GameObject playerScoreArea;

    // Ability score populating event declarations.
    public delegate void PopulatePlayerAbilityScores(List<CardStatEntry> cardStatList, bool isPlayer);
    public static event PopulatePlayerAbilityScores PopulatePlayerScores;

    // Player stat declarations.
    int playerAgility = 10;

    void Start()
    {

    }

    public static void ScoreUI(List<CardStatEntry> cardStatList, bool isPlayer)
    {
        foreach (object CardStatEntry in cardStatList)
        {
            Debug.Log(CardStatEntry + " from HandBuilderMethod");
        }

        if (isPlayer)
        {
            if (PopulatePlayerScores != null)
            {
                PopulatePlayerScores.Invoke(cardStatList, isPlayer);
            }
        }
    }

}
