using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using System.Linq;
using TMPro;
using CardStatsNamespace;

public class HandManager : MonoBehaviour
{

    [Header("Game Objects")]
    [SerializeField] GameObject cardGeneric;
    [SerializeField] GameObject playerHandArea;
    [SerializeField] GameObject npcHandArea;

    [Header("Player Hand Alignments")]
    [SerializeField] int playerHandLeftAlign = -750;
    [SerializeField] int playerHandVerticalAlign = 0;

    [Header("NPC Hand Alignments")]
    [SerializeField] int npcHandLeftAlign = -750;
    [SerializeField] int npcHandVerticalAlign = 0;

    bool drew = false; // This helps us lock off functions unless the player has drawn cards.
    bool isPlayer = true; // This helps us know whose cards are whose.
    float cardCountSeparator = 0f; // This is the float which ticks up and helps set the distance between each card.

    // Card stat variables
    int agilityBonus = 0;
    int enduranceBonus = 0;
    int charismaBonus = 0;
    int mindBonus = 0;
    string cardType = "";
    string abilityCardName = "";

    public delegate void DestroyCardsHandler();
    public static event DestroyCardsHandler DestroyOldCards;

    public delegate void ResetPScoreHandler();
    public static event ResetPScoreHandler ResetPScores;

    public delegate void ResetNPCScoreHandler();
    public static event ResetNPCScoreHandler ResetNpcScores;

    public delegate void PlayerAbilityCheck(List<CardStatEntry> cardStatList, bool isPlayer);
    public static event PlayerAbilityCheck p_AbilityMath;

    public delegate void npcAbilityCheck(List<CardStatEntry> cardStatList, bool isPlayer);
    public static event npcAbilityCheck npc_AbilityMath;

    public delegate void TriggerAbilityCheck();
    public static event TriggerAbilityCheck TriggeredAbilityCheck;

    void Start()
    {
        CardStats abilityCardStats = cardGeneric.GetComponent<CardStats>();
        PixelCrushers.DialogueSystem.Articy.ArticyTools.InitializeLuaSubtables();
    }

    //////////////////////////////////////////////////////////////////////////
    //                                                                      //
    //                                                                      //
    // Button click methods                                                 //
    //                                                                      //
    //                                                                      //
    //////////////////////////////////////////////////////////////////////////

    public void Draw()
    {
        drew = false;
        ResetCards();
        PBuildHand();
        NPCBuildHand();
        drew = true;
    }

    public void Check()
    {
        if (drew)
        {
            if (TriggeredAbilityCheck != null)
            {
                TriggeredAbilityCheck.Invoke();
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////
    //                                                                      //
    //                                                                      //
    // Card populating                                                      //
    //                                                                      //
    //                                                                      //
    //////////////////////////////////////////////////////////////////////////

    public void ResetCards()
    {
        if (DestroyOldCards != null)
        {
            DestroyOldCards.Invoke();
        }
    }

    // Building the player hand.
    private void PBuildHand()
    {
        isPlayer = true;
        cardCountSeparator = 0f;

        LuaTableWrapper pCards = DialogueLua.GetActorField("Ovidio_Cabeaga", "p_hand").asTable;

        if (ResetPScores != null)
        {
            ResetPScores.Invoke();
        }

        foreach (LuaTableWrapper card in pCards.values)
        {
            CardData(card, isPlayer);
        }
    }

    // Building the NPC hand.
    private void NPCBuildHand()
    {
        isPlayer = false;
        cardCountSeparator = 0f;

        //
        //
        //
        // VERY IMPORTANT:
        // "Bellboy" is just a test.
        // This should be replaced by
        // whichever NPC is the Conversant, like so:
        //
        //        string npcName = DialogueActor.GetActorName(DialogueManager.currentConversant);
        //        LuaTableWrapper npcCards = DialogueLua.GetActorField(npcName, "npc_hand").asTable;
        //
        //
        LuaTableWrapper npcCards = DialogueLua.GetActorField("Bellboy", "npc_hand").asTable;

        if (ResetNpcScores != null)
        {
            ResetNpcScores.Invoke();
        }

        foreach (LuaTableWrapper card in npcCards.values)
        {
            CardData(card, isPlayer);
        }

    }

    //////////////////////////////////////////////////////////////////////////
    //                                                                      //
    //                                                                      //
    // Fetching card data from DB and creating card clones                  //
    //                                                                      //
    //                                                                      //
    //////////////////////////////////////////////////////////////////////////

    public void CardData(LuaTableWrapper card, bool isPlayer)
    {
        cardCountSeparator++;

        // Gets values of a card's bonuses from the Articy DB created by Dialogue System
        float agBonusFloat = (float)card["agilityBonus"]; // specified cast is not valid
        float endBonusFloat = (float)card["enduranceBonus"];
        float chaBonusFloat = (float)card["charismaBonus"];
        float miBonusFloat = (float)card["mindBonus"];

        agilityBonus = (int)agBonusFloat;
        enduranceBonus = (int)endBonusFloat;
        charismaBonus = (int)chaBonusFloat;
        mindBonus = (int)miBonusFloat;

        // Gets card type and removes first 8 characters from technical name
        cardType = (string)card["cardType"];
        cardType = cardType.Remove(0, 8);

        // Gets name of card
        abilityCardName = ((string)card["Name"]); // OLD VERSION   abilityCardName = ($"{card.Name}");

        // Stores the card stats in a list.
        List<CardStatEntry> cardStatList = new List<CardStatEntry>();
        cardStatList.Add(new CardStatEntry() { AbilityBonus = agilityBonus, AbilityName = "Agility" });
        cardStatList.Add(new CardStatEntry() { AbilityBonus = enduranceBonus, AbilityName = "Endurance" });
        cardStatList.Add(new CardStatEntry() { AbilityBonus = charismaBonus, AbilityName = "Charisma" });
        cardStatList.Add(new CardStatEntry() { AbilityBonus = mindBonus, AbilityName = "Mind" });

        // Creates clone of card, and does basic math to determine space between each card.
        if (isPlayer == true)
        {
            GameObject abilityCard = Instantiate
                (cardGeneric, new Vector3(playerHandLeftAlign +
                (180 * cardCountSeparator), playerHandVerticalAlign, 0), Quaternion.identity);
            abilityCard.transform.SetParent(playerHandArea.transform, false);
            abilityCard.name = abilityCardName;

            // Tells the new clone card to update its values
            CardStats abilityCardStats = abilityCard.GetComponent<CardStats>();
            abilityCardStats.PopulateCardText(abilityCardName, cardStatList, cardType);
        }
        else if (isPlayer == false)
        {
            GameObject abilityCard = Instantiate
                (cardGeneric, new Vector3(npcHandLeftAlign +
                (180 * cardCountSeparator), npcHandVerticalAlign, 0), Quaternion.identity);
            abilityCard.transform.SetParent(npcHandArea.transform, false);
            abilityCard.name = abilityCardName;

            // Tells the new clone card to update its values
            CardStats abilityCardStats = abilityCard.GetComponent<CardStats>();
            abilityCardStats.PopulateCardText(abilityCardName, cardStatList, cardType);
        }

        // Feeds the cardStatList info to the AbilityCheckManager, along with the isPlayer bool.
        if (isPlayer == true)
        {
            if (p_AbilityMath != null)
            {
                p_AbilityMath.Invoke(cardStatList, isPlayer);
            }
        }
        else if (isPlayer == false)
        {
            if (npc_AbilityMath != null)
            {
                npc_AbilityMath.Invoke(cardStatList, isPlayer);
            }
        }

    }
}