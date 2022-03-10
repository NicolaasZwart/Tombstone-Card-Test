using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
            agilityValue.text = 
        }
    }

    void OnDisable()
    {
        AbilityCheckManager.PopulatePlayerScores -= PopulatePlayerScores;
    }


}
