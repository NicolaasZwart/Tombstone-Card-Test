using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using CardStatsNamespace;

public class AbilityUIManager : MonoBehaviour
{
    [Header("Player Ability Score Value Fields")]
    [SerializeField] TextMeshProUGUI p_agValDisp;
    [SerializeField] TextMeshProUGUI p_endValDisp;
    [SerializeField] TextMeshProUGUI p_chaValDisp;
    [SerializeField] TextMeshProUGUI p_miValDisp;

    void OnEnable()
    {
        AbilityCheckManager.PopulatedPScores += PopulatePlayerScores;
    }

    void PopulatePlayerScores(int p_agVal, int p_endVal, int p_chaVal, int p_miVal, bool isPlayer)
    {
        if (isPlayer)
        {
            p_agValDisp.text = p_agVal.ToString();
            p_endValDisp.text = p_endVal.ToString();
            p_chaValDisp.text = p_chaVal.ToString();
            p_miValDisp.text = p_miVal.ToString();
        }
        else
        {
            Debug.Log("The bool isPlayer == true did not reach the PopulatePlayerScores method.");
        }
    }

    void OnDisable()
    {
        AbilityCheckManager.PopulatedPScores -= PopulatePlayerScores;
    }
}
