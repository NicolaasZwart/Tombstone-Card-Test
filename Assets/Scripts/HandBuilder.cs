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
    [SerializeField] GameObject cardGeneric;
    [SerializeField] GameObject playerCardsArea;
    [SerializeField] TMP_Dropdown handSizeDropdown;
    [SerializeField] TMP_Dropdown numberOfClassCardsDropdown;
    [SerializeField] TMP_Dropdown numberOfBGCardsDropdown;

    [SerializeField] TMP_Text infoText;

    // This connects with the CardStats script on each CardGeneric.
    CardStats playerCardStats; 

    // This gives us the count of the whole deck when needed for debugging purposes.
    int cardCount;
    
    // This is the float which ticks up and helps set the distance between each card.
    float cardCountSeparator = 0f; 
    
    // Handbuilder variables
    int handSize = 5; // Hand size. Modifiable from a dropdown.
    int classCardsDesired = 1; // Number of class cards desired in a hand. Modifiable from a dropdown.
    int backgroundCardsDesired = 2; // Number of background cards desired in a hand. Modifiable from a dropdown.
    int questCardsDesired = 2;

    // Card stat variables
    int agilityBonus = 0;
    int enduranceBonus = 0;    
    int charismaBonus = 0;
    int mindBonus = 0;
    string cardType = "";
    string playerCardName = "";

    private IEnumerable<Item> allCards = Enumerable.Empty<Item>();


  /*  void Awake() 
    {

        IEnumerable<Item> allCards =
                from card in DialogueManager.masterDatabase.items
                where card.FieldExists("isCard") == true
                select card;
    }*/

    void Start()
    {
        playerCardStats = cardGeneric.GetComponent<CardStats>();
        Random.InitState(DateTime.Now.Millisecond);
        //handSizeDropdown = GetComponent<TMP_Dropdown>();
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
        BuildTheWholeDeck();
    }

    public void RandomHandOnClick()
    {
        BuildARandomHand();
    }

    //////////////////////////////////////////////////////////////////////////
    //                                                                      //
    //                                                                      //
    // Card populating                                                      //
    //                                                                      //
    //                                                                      //
    //////////////////////////////////////////////////////////////////////////

    public void BuildTheWholeDeck()
    {
        cardCountSeparator = 0f;

        // This counts how many total cards are in the deck.
        cardCount = allCards.Count();
        
        foreach (Item card in allCards)
        {
            CardData(card);
        }
    }

    private void BuildARandomHand()
    {

        //int randomNumberTest = Random.Range(0, 100);
        //Debug.Log(randomNumberTest + ": Random number test!");

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
        backgroundCardsDesired = Convert.ToInt32((numberOfBGCardsDropdown.options[numberOfClassCardsDropdown.value].text));

        if (handSize < classCardsDesired + backgroundCardsDesired - 1)
        {
            infoText.text = "Warning: The hand size is too small. Decrease number of Background and/or Class Cards, or increase hand size.";
        }
        else
        {
            // Sorts all cards from the entire deck into temporary lists and counts how many cards of each type there are.
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

            //// Class card randomizer.
            
            // Generates a list of numbers from 0 to the count of class cards desired for the hand.
            List<int> randomClassCardPicks = new(); // Declares list
            for (int n = 0; n < classCards.Count() + 1; n++) // Populates list
            {
                randomClassCardPicks.Add(n);
            }

            /*foreach (var x in randomBackgroundCardPicks)
            {
                Debug.Log(x.ToString() + " IS THE LIST OF BACKGROUND CARD INT PICKS");
            }*/

            // Pulls *randomly* from the list of numbers, then finds the class card at that index and adds it to the randomHand list.
            // Then removes the just-chosen number from the list, and the process repeats a number of times equal to the number of
            // class cards desired.
            for (int n = 0; n < classCardsDesired; n++)
            {
                int indexClass = Random.Range(0, randomClassCardPicks.Count - 1);
                Debug.Log(indexClass + " is the randomly chosen index from randomClassCardPicks.");
                int randomClassCardDraw = randomClassCardPicks[indexClass];
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
                Debug.Log(indexBackground + " is the randomly chosen index from randomBackgroundCardPicks.");
                int randomBackgroundCardDraw = randomBackgroundCardPicks[indexBackground];
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

            //Debug.Log(questCardsDesired + " Quest cards desired."); // this works, but somewhere in the code block immediately below, something does not.

            for (int n = 0; n < questCardsDesired; n++)
            {
                int indexQuest = Random.Range(0, randomQuestCardPicks.Count - 1);
                Debug.Log(indexQuest + " is the randomly chosen index from randomQuestCardPicks.");
                int randomQuestCardDraw = randomQuestCardPicks[indexQuest];
                randomHand.Add(questCards.ElementAt(randomQuestCardDraw));
                randomQuestCardPicks.RemoveAt(indexQuest);
            }

            //Debug.Log("Fire");

            //// Psionic card randomizer TBD!!!!

            // Final count of cards in randomHand.
            //Debug.Log(randomHand.Count() + " is the total number of cards currently in randomHand!");

        }


    }
    
    //////////////////////////////////////////////////////////////////////////
    //                                                                      //
    //                                                                      //
    // CardData                                                             //
    //                                                                      //
    //                                                                      //
    //////////////////////////////////////////////////////////////////////////

    public void CardData(Item card)
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
        playerCardName = ($"{card.Name}");
        
        // Stores the card stats in a list.
        List<CardStatEntry> cardStatList = new List<CardStatEntry>();
        cardStatList.Add(new CardStatEntry() { AbilityBonus = agilityBonus, AbilityName = "Agility" });
        cardStatList.Add(new CardStatEntry() { AbilityBonus = enduranceBonus, AbilityName = "Endurance" });
        cardStatList.Add(new CardStatEntry() { AbilityBonus = charismaBonus, AbilityName = "Charisma" });
        cardStatList.Add(new CardStatEntry() { AbilityBonus = mindBonus, AbilityName = "Mind" });

        // Creates clone of card, and does basic math to determine space between each card.
        GameObject playerCard = Instantiate (cardGeneric, new Vector3(-700 + (140 * cardCountSeparator), 0, 0), Quaternion.identity);
        playerCard.transform.SetParent (playerCardsArea.transform, false);
        playerCard.name = playerCardName;

        // Tells the new clone card to update its values
        CardStats playerCardStats = playerCard.GetComponent<CardStats>();
        playerCardStats.populateCardText(playerCardName, cardStatList, cardType);
    }
}