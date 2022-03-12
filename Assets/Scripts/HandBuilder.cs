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
    [SerializeField] TMP_Dropdown handSizeDropdown;
    [SerializeField] TMP_Dropdown numberOfClassCardsDropdown;
    [SerializeField] TMP_Dropdown numberOfBGCardsDropdown;
    [SerializeField] TMP_Text infoText;

    [Header("Player Hand Alignments")]
    [SerializeField] int playerHandLeftAlign = -750;
    [SerializeField] int playerHandVerticalAlign = 0;

    // This connects with the CardStats script on each CardGeneric.
    CardStats abilityCardStats;

    // This gives us the count of the whole deck when needed for debugging purposes.
    int cardCount;

    // This helps us know whose cards are whose.
    bool isPlayer = true;
    
    // This is the float which ticks up and helps set the distance between each card.
    float cardCountSeparator = 0f; 
    
    // Handbuilder variables
    int handSize = 5;
    int classCardsDesired = 1;
    int backgroundCardsDesired = 2;
    int questCardsDesired = 2;

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

    public delegate void PlayerAbilityCheck(List<CardStatEntry> cardStatList, bool isPlayer);
    public static event PlayerAbilityCheck p_AbilityCheck;

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

        // This counts how many total cards are in the deck.
        cardCount = allCards.Count();
        
        foreach (Item card in allCards)
        {
            CardData(card, isPlayer);
        }
    }

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
        List<Item> randomHand = new();

        // Takes the entries in the serialized dropdown lists and converts them to variables.
        handSize = Convert.ToInt32((handSizeDropdown.options[handSizeDropdown.value].text));
        //Debug.Log("The current hand size is " + handSize + "!");
        classCardsDesired = Convert.ToInt32((numberOfClassCardsDropdown.options[numberOfClassCardsDropdown.value].text));
        backgroundCardsDesired = Convert.ToInt32((numberOfBGCardsDropdown.options[numberOfBGCardsDropdown.value].text));
        //Debug.Log(backgroundCardsDesired + " background cards desired from dropdown");

        if (handSize < classCardsDesired + backgroundCardsDesired)
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
            List<int> randomClassCardPicks = new(); // Declares list
            for (int n = 0; n < classCards.Count() + 1; n++) // Populates list
            {
                randomClassCardPicks.Add(n);
            }

            // Pulls *randomly* from the list of numbers, then finds the class card at that index and adds it to the randomHand list.
            // Then removes the just-chosen number from the list, and the process repeats a number of times equal to the number of
            // class cards desired.
            for (int n = 0; n < classCardsDesired; n++)
            {
                int indexClass = Random.Range(0, randomClassCardPicks.Count - 1);
                int randomClassCardDraw = randomClassCardPicks[indexClass];
                //Debug.Log(randomClassCardDraw + " is the randomly chosen index from randomClassCardPicks.");
                randomHand.Add(classCards.ElementAt(randomClassCardDraw));
                randomClassCardPicks.RemoveAt(indexClass);
            }


            //// Background card randomizer.

            List<int> randomBackgroundCardPicks = new();
            for (int n = 0; n < backgroundCards.Count() + 1; n++)
            {
                randomBackgroundCardPicks.Add(n);
            }

            for (int n = 0; n < backgroundCardsDesired; n++)
            {
                int indexBackground = Random.Range(0, randomBackgroundCardPicks.Count - 1);
                int randomBackgroundCardDraw = randomBackgroundCardPicks[indexBackground];
                //Debug.Log(randomBackgroundCardDraw + " is the randomly chosen index from randomBackgroundCardPicks.");
                randomHand.Add(backgroundCards.ElementAt(randomBackgroundCardDraw));
                randomBackgroundCardPicks.RemoveAt(indexBackground);
            }


            //// Quest card randomizer.

            questCardsDesired = handSize - classCardsDesired - backgroundCardsDesired;

            List<int> randomQuestCardPicks = new();
            for (int n = 0; n < questCards.Count() + 1; n++)
            {
                randomQuestCardPicks.Add(n);
            }

            //Debug.Log(questCardsDesired + " Quest cards desired.");

            for (int n = 0; n < questCardsDesired; n++)
            {
                int indexQuest = Random.Range(0, randomQuestCardPicks.Count - 1);
                int randomQuestCardDraw = randomQuestCardPicks[indexQuest];
                //Debug.Log(randomQuestCardDraw + " is the randomly chosen index from randomQuestCardPicks.");
                randomHand.Add(questCards.ElementAt(randomQuestCardDraw));
                randomQuestCardPicks.RemoveAt(indexQuest);
            }

            //// Psionic card randomizer TBD!!!!

            // Final count of cards in randomHand.
            //Debug.Log(randomHand.Count() + " is the total number of cards currently in randomHand!");

        }

        if (ResetPScores != null)
        {
            ResetPScores.Invoke();
        }

        foreach (Item card in randomHand)
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
        // WILL NEED TO UPDATE THIS FOR NPCCARDS
        // probably the way to do it is to have a playerCardGenerate method fire on button press
        // which sets isPlayer bool to true, (and below we have an if statement that places
        // cards depending on if that bool is true or not)
        // then have an NPCCardGenerate method fire right after, which at the top of the method sets isPlayer
        // bool to false.
        GameObject abilityCard = Instantiate
            (cardGeneric, new Vector3(playerHandLeftAlign + 
            (180 * cardCountSeparator), playerHandVerticalAlign, 0), Quaternion.identity);
        abilityCard.transform.SetParent(playerHandArea.transform, false);
        abilityCard.name = abilityCardName;

        // Tells the new clone card to update its values
        CardStats abilityCardStats = abilityCard.GetComponent<CardStats>();
        abilityCardStats.PopulateCardText(abilityCardName, cardStatList, cardType);

        // Feeds the cardStatList info to the AbilityCheckManager, along with the isPlayer bool.
        p_AbilityCheck.Invoke(cardStatList, isPlayer);
    }
}