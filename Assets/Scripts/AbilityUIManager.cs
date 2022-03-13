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

    [Header("NPC Ability Score Value Fields")]
    [SerializeField] TextMeshProUGUI npc_agValDisp;
    [SerializeField] TextMeshProUGUI npc_endValDisp;
    [SerializeField] TextMeshProUGUI npc_chaValDisp;
    [SerializeField] TextMeshProUGUI npc_miValDisp;

    void OnEnable()
    {
        AbilityCheckManager.PopulatedPScores += PopulatePlayerScores;
        AbilityCheckManager.PopulatedNPCScores += PopulateNPCScores;
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

    void PopulateNPCScores(int npc_agVal, int npc_endVal, int npc_chaVal, int npc_miVal, bool isPlayer)
    {
        if (!isPlayer)
        {
            Debug.Log(npc_agVal + " this is npc agility value");
            npc_agValDisp.text = npc_agVal.ToString(); ///// THERE IS AN ERROR HERE THAT I CAN'T FIGURE OUT.
            npc_endValDisp.text = npc_endVal.ToString();
            npc_chaValDisp.text = npc_chaVal.ToString();
            npc_miValDisp.text = npc_miVal.ToString();
        }
        else
        {
            Debug.Log("The bool isPlayer == false did not reach the PopulateNPCScores method.");
        }
    }

    void OnDisable()
    {
        AbilityCheckManager.PopulatedPScores -= PopulatePlayerScores;
        AbilityCheckManager.PopulatedNPCScores -= PopulateNPCScores;
    }
}
