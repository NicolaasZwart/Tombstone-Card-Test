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

    // This helps us know whose cards are whose.
    bool isPlayer = true;

    // This is the float which ticks up and helps set the distance between each card.
    float cardCountSeparator = 0f;

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
        ResetCards();
        PBuildHand();
        NPCBuildHand();
        TriggeredAbilityCheck.Invoke();
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

        LuaTableWrapper p_cards = DialogueLua.GetActorField("Ovidio_Cabeaga", "p_hand").asTable;

        foreach (LuaTableWrapper card in p_cards.values)
        {
            CardData(card, isPlayer);
        }

        if (ResetPScores != null)
        {
            ResetPScores.Invoke();
        }

        /*List<Item> p_Hand = new(); // This is the list that will comprise the player's hand.

        foreach (Item card in p_Hand)
        {
            CardData(card, isPlayer);
        }*/
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
        // whichever NPC is the Conversant
        //
        //
        //
        LuaTableWrapper bellboyCards = DialogueLua.GetActorField("Bellboy", "npc_hand").asTable;


        if (ResetNpcScores != null)
        {
            ResetNpcScores.Invoke();
        }

        /*List<Item> npc_Hand = new(); // This is the list that will comprise the NPC's hand.

        foreach (Item card in npc_Hand)
        {
            CardData(card, isPlayer);
        }*/
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
        agilityBonus = card.LookupInt("agilityBonus");
        enduranceBonus = card.LookupInt("enduranceBonus");
        charismaBonus = card.LookupInt("charismaBonus");
        mindBonus = card.LookupInt("mindBonus");

        // Gets card type and removes first 8 characters from technical name
        cardType = card.LookupValue("cardType");
        cardType = cardType.Remove(0, 8);

        // Gets name of card
        abilityCardName = ($"{card.Name}");

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
            p_AbilityMath.Invoke(cardStatList, isPlayer);
        }
        else if (isPlayer == false)
        {
            npc_AbilityMath.Invoke(cardStatList, isPlayer);
        }

    }
}