using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using System.Linq;
using CardStatsNamespace;
using TMPro;
using Random = UnityEngine.Random;

public class HandBuilder : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] GameObject cardGeneric;
    [SerializeField] GameObject playerHandArea;
    [SerializeField] GameObject npcHandArea;
    [SerializeField] TMP_Dropdown p_handSzDrpdown;
    [SerializeField] TMP_Dropdown p_numClsCrdsDrpdown;
    [SerializeField] TMP_Dropdown p_numBGCrdsDrpdown;
    [SerializeField] TMP_Text infoText;
    [SerializeField] TMP_Dropdown npc_handSizeDrpdwn;

    [Header("Player Hand Alignments")]
    [SerializeField] int playerHandLeftAlign = -750;
    [SerializeField] int playerHandVerticalAlign = 0;

    [Header("NPC Hand Alignments")]
    [SerializeField] int npcHandLeftAlign = -750;
    [SerializeField] int npcHandVerticalAlign = 0;

    int n = 0; // int for debugging

    // This connects with the CardStats script on each CardGeneric.
    CardStats abilityCardStats;

    // This gives us the count of the whole deck when needed for debugging purposes.
    int cardCount;

    // This helps us know whose cards are whose.
    bool isPlayer = true;
    
    // This is the float which ticks up and helps set the distance between each card.
    float cardCountSeparator = 0f; 
    
    // Handbuilder variables
    int p_handSize = 5;
    int p_claCardsDesired = 1;
    int p_bgCardsDesired = 2;
    int p_queCardsDesired = 2;
    int npc_handSize = 5;

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
    public static event PlayerAbilityCheck p_AbilityCheck;

    public delegate void npcAbilityCheck(List<CardStatEntry> cardStatList, bool isPlayer);
    public static event npcAbilityCheck npc_AbilityCheck;

    void Start()
    {
        abilityCardStats = cardGeneric.GetComponent<CardStats>();
        Random.InitState(DateTime.Now.Millisecond);
    }

    //////////////////////////////////////////////////////////////////////////
    //                                                                      //
    //                                                                      //
    // Button click methods                                                 //
    //                                                                      //
    //                                                                      //
    //////////////////////////////////////////////////////////////////////////

    public void WholeDeckOnClick()
    {
        cardCount = 0;
        ResetCards();
        BuildTheWholeDeck();
    }

    public void RandomHandOnClick()
    {
        ResetCards();
        PlayerBuildARandomHand();
        NPCBuildARandomHand();
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
     
    public void BuildTheWholeDeck()
    {
        cardCountSeparator = 0f;

        IEnumerable<Item> allCards =
            from card in DialogueManager.masterDatabase.items
            where card.FieldExists("isCard") == true
            select card;

        cardCount = allCards.Count();
        
        foreach (Item card in allCards)
        {
            CardData(card, isPlayer);
        }
    }

    // Building a player hand.
    private void PlayerBuildARandomHand()
    {
        isPlayer = true;
        infoText.text = "";
        cardCountSeparator = 0f;

        IEnumerable<Item> allCards =
            from card in DialogueManager.masterDatabase.items
            where card.FieldExists("isCard") == true
            select card;

        var sortedCards =
            from card in allCards
            group card by card.LookupValue("cardType");

        // This is the list that will comprise the finalized random hand.
        List<Item> p_ranHand = new();

        // Takes the entries from the serialized dropdown lists and converts them to variables.
        p_handSize = Convert.ToInt32((p_handSzDrpdown.options[p_handSzDrpdown.value].text));
        //Debug.Log("The current hand size is " + handSize + "!");
        p_claCardsDesired = Convert.ToInt32((p_numClsCrdsDrpdown.options[p_numClsCrdsDrpdown.value].text));
        p_bgCardsDesired = Convert.ToInt32((p_numBGCrdsDrpdown.options[p_numBGCrdsDrpdown.value].text));
        //Debug.Log(backgroundCardsDesired + " background cards desired from dropdown");

        if (p_handSize < p_claCardsDesired + p_bgCardsDesired)
        {
            infoText.text = "Warning: " +
                "The hand size is too small. " +
                "Decrease number of Background " +
                "and/or Class Cards, or increase hand size.";
        }
        else
        {
            // Sorts all cards from the entire deck into temporary lists
            // and counts how many cards of each type there are.
            List<Item> classCards = new();
            List<Item> backgroundCards = new();
            List<Item> questCards = new();
            List<Item> psionicCards = new();

            classCards = sortedCards.FirstOrDefault(x => x.Key == "cardTypeClass").ToList<Item>();
            //Debug.Log(classCards.Count() + " Class Cards");

            backgroundCards = sortedCards.FirstOrDefault(x => x.Key == "cardTypeBackground").ToList<Item>();
            //Debug.Log(backgroundCards.Count() + " Background Cards");

            questCards = sortedCards.FirstOrDefault(x => x.Key == "cardTypeQuest").ToList<Item>();
            //Debug.Log(questCards.Count() + " Quest Cards");

            psionicCards = sortedCards.FirstOrDefault(x => x.Key == "cardTypePsionic").ToList<Item>();
            //Debug.Log(psionicCards.Count() + " Psionic Cards");

            
            ////// Randomization section.
            
            //// Class card randomizer.
            
            // Generates a list of numbers from 0 to the count of class cards desired for the hand.
            List<int> p_ranClaCardPicks = new(); // Declares list
            for (int n = 0; n < classCards.Count() + 1; n++) // Populates list
            {
                p_ranClaCardPicks.Add(n);
            }

            // Pulls *randomly* from the list of numbers, then finds the class card at that index and adds it to the p_ranHand list.
            // Then removes the just-chosen number from the list, and the process repeats a number of times equal to the number of
            // class cards desired.
            for (int n = 0; n < p_claCardsDesired; n++)
            {
                int p_indexClass = Random.Range(0, p_ranClaCardPicks.Count - 1);
                int randomClassCardDraw = p_ranClaCardPicks[p_indexClass];
                //Debug.Log(randomClassCardDraw + " is the randomly chosen index from p_ranClaCardPicks.");
                p_ranHand.Add(classCards.ElementAt(randomClassCardDraw));
                p_ranClaCardPicks.RemoveAt(p_indexClass);
            }


            //// Background card randomizer.

            List<int> p_ranBGCardPicks = new();
            for (int n = 0; n < backgroundCards.Count() + 1; n++)
            {
                p_ranBGCardPicks.Add(n);
            }

            for (int n = 0; n < p_bgCardsDesired; n++)
            {
                int p_indexBackground = Random.Range(0, p_ranBGCardPicks.Count - 1);
                int randomBackgroundCardDraw = p_ranBGCardPicks[p_indexBackground];
                //Debug.Log(randomBackgroundCardDraw + " is the randomly chosen index from p_ranBGCardPicks.");
                p_ranHand.Add(backgroundCards.ElementAt(randomBackgroundCardDraw));
                p_ranBGCardPicks.RemoveAt(p_indexBackground);
            }


            //// Quest card randomizer.

            p_queCardsDesired = p_handSize - p_claCardsDesired - p_bgCardsDesired;

            List<int> p_ranQueCardPicks = new();
            for (int n = 0; n < questCards.Count() + 1; n++)
            {
                p_ranQueCardPicks.Add(n);
            }

            //Debug.Log(questCardsDesired + " Quest cards desired.");

            for (int n = 0; n < p_queCardsDesired; n++)
            {
                int p_indexQuest = Random.Range(0, p_ranQueCardPicks.Count - 1);
                int randomQuestCardDraw = p_ranQueCardPicks[p_indexQuest];
                //Debug.Log(randomQuestCardDraw + " is the randomly chosen index from p_ranQueCardPicks.");
                p_ranHand.Add(questCards.ElementAt(randomQuestCardDraw));
                p_ranQueCardPicks.RemoveAt(p_indexQuest);
            }

            //// Psionic card randomizer TBD!!!!

            // Final count of cards in p_ranHand.
            //Debug.Log(p_ranHand.Count() + " is the total number of cards currently in p_ranHand!");

        }

        if (ResetPScores != null)
        {
            ResetPScores.Invoke();
        }

        foreach (Item card in p_ranHand)
        {
            CardData(card, isPlayer);
        }
    }

    private void NPCBuildARandomHand()
    {
        isPlayer = false;
        infoText.text = "";
        cardCountSeparator = 0f;

        IEnumerable<Item> allCards =
            from card in DialogueManager.masterDatabase.items
            where card.FieldExists("isCard") == true
            select card;

        var sortedCards =
            from card in allCards
            group card by card.LookupValue("cardType");

        // This is the list that will comprise the finalized random hand.
        List<Item> npc_ranHand = new();

        // Takes the entries from the serialized dropdown lists and converts them to variables.
        npc_handSize = Convert.ToInt32((npc_handSizeDrpdwn.options[npc_handSizeDrpdwn.value].text));
        //Debug.Log("The current NPC hand size is " + npc_handSize + "!");

        List<Item> npc_backgroundCards = new();

        npc_backgroundCards = sortedCards.FirstOrDefault(x => x.Key == "cardTypeBackground").ToList<Item>();
        //Debug.Log(npc_backgroundCards.Count() + " Background Cards");


        ////// NPC Randomization section.

        //// NPC Background card randomizer.

        List<int> npc_ranBGCardPicks = new();
        for (int n = 0; n < npc_backgroundCards.Count() + 1; n++)
        {
            npc_ranBGCardPicks.Add(n);
        }

        for (int n = 0; n < npc_handSize; n++)
        {
            int npc_indexBackground = Random.Range(0, npc_ranBGCardPicks.Count - 1);
            int npc_randomBackgroundCardDraw = npc_ranBGCardPicks[npc_indexBackground];
            //Debug.Log(randomBackgroundCardDraw + " is the randomly chosen index from npc_ranBGCardPicks.");
            npc_ranHand.Add(npc_backgroundCards.ElementAt(npc_randomBackgroundCardDraw));
            npc_ranBGCardPicks.RemoveAt(npc_indexBackground);
        }

        //// Psionic card randomizer TBD!!!!

        // Final count of cards in npc_ranHand.
        //Debug.Log(npc_ranHand.Count() + " is the total number of cards currently in npc_ranHand!");

        if (ResetNpcScores != null)
        {
            ResetNpcScores.Invoke();
        }

        foreach (Item card in npc_ranHand)
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

    public void CardData(Item card, bool isPlayer)
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
            p_AbilityCheck.Invoke(cardStatList, isPlayer);
        }
        else if (isPlayer == false)
        {
            npc_AbilityCheck.Invoke(cardStatList, isPlayer);
        }
        
    }
}