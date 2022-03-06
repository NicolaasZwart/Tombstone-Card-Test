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

    // Card count variables
    int classCardCount = 0;
    int backgroundCardCount = 0;
    int questCardCount = 0;
    int psionicCardCount = 0;

    // Random selection variables
    int randomClassCardDraw = 0;
    int randomBackgroundCardDraw = 0;
    int randomQuestCardDraw = 0;

    // Card stat variables
    int agilityBonus = 0;
    int enduranceBonus = 0;    
    int charismaBonus = 0;
    int mindBonus = 0;
    string cardType = "";
    string playerCardName = "";

    private IEnumerable<Item> allCards;


    void Awake() 
    {
        IEnumerable<Item> allCards =
            from card in DialogueManager.masterDatabase.items
            where card.FieldExists("isCard") == true
            select card;
    }
    
    void Start()
    {
        playerCardStats = cardGeneric.GetComponent<CardStats>();
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
        cardCountSeparator = 0f;

        var sortedCards =
            from card in allCards
            group card by card.LookupValue("cardType");

        // Takes the entries in the serialized dropdown list and converts them to the handSize variable.
        handSize = Convert.ToInt32((handSizeDropdown.options[handSizeDropdown.value].text));
        Debug.Log("The current hand size is " + handSize + "!");
        classCardsDesired = Convert.ToInt32((numberOfClassCardsDropdown.options[numberOfClassCardsDropdown.value].text));
        backgroundCardsDesired = Convert.ToInt32((numberOfBGCardsDropdown.options[numberOfClassCardsDropdown.value].text));

        List<Item> classCards = new List<Item>();
        List<Item> backgroundCards = new List<Item>();
        List<Item> questCards = new List<Item>();
        List<Item> psionicCards = new List<Item>();

        foreach (var cardGroup in sortedCards) // don't nbeed any of this. When doing the drawing, do it from a temporary list of all the class cards, and remove from the list when you draw it.
        {      
            switch (cardGroup.Key)
            {
                case "cardTypeClass":
                    classCardCount = cardGroup.Count();
                    Debug.Log(classCardCount + " Class Cards");
                    classCards = cardGroup.ToList<Item>(); // you don't even need to do this, whenever I need to access the list by a random number, just access sortedCards by the key, and pull a random card out of the list returned by that key
                    Debug.Log(cardGroup.ElementAt(2).Name + " is the third sorted card of the Class-type group!");
                    break;

                case "cardTypeBackground":
                    backgroundCardCount = cardGroup.Count();
                    Debug.Log(backgroundCardCount + " Background Cards");
                    Debug.Log(cardGroup.ElementAt(2).Name + " is the third sorted card of the Background-type group!");
                    break;

                case "cardTypeQuest":
                    questCardCount = cardGroup.Count();
                    Debug.Log(questCardCount + " Quest Cards");
                    break;

                case "cardTypePsionic":
                    psionicCardCount = cardGroup.Count();
                    Debug.Log(psionicCardCount + " Psionic Cards");
                    break;

                default:
                    Debug.Log($"The BuildARandomHand switch statement reached a default.");
                    break;
            }
        }

        Debug.Log(sortedCards.FirstOrDefault(x => x.Key == "cardTypeClass").ToList<Item>().ElementAt(1).Name + " is the second sorted card of the Class-type group!");
        sortedCards.FirstOrDefault(x => x.Key == "cardTypeClass").ToList<Item>().Count(); // this is the same result as the switch.
        
        // classCards = sortedCards.FirstOrDefault(x => x.Key == "cardTypeClass").ToList<Item>(); // this is equivalent to an entire case in the above switch.
        // This would be in a while, or For loop, going for how many cards you need to draw. classCards.Count(); 
        // then, make a random number between 0 and Count of class cards.
        // then you draw that card from the class cards list and remove it, and then you repeat the step above until you draw as many cards as you needed.
         

        /*
        take the hand size and the number of class cards and do some math 
        to figure out how many of each other kinds of cards to draw. 
        Probably should be two background cards.
        */
        List<Item> randomHand = new List<Item>();
/*
        List<int> randomClassPicks = new List<int>(); // Declare list.
        for (int n = 0; n < classCardsDesired; n++) // Populate list.
        {
            randomClassPicks.Add(n);
        }
        for (int n = 0; n < classCardsDesired; n++)
        {
            int index = Random.Range(0, randomClassPicks.Count - 1); // Pick random element from the list.
            int randomClassCardDraw = randomClassPicks[index]; // classCardPick = the number that was randomly picked.
            randomHand.Add(//CLASS CARD GROUP NAME HERE.ElementAt(randomClassCardDraw)); // .....................TROUBLESHOOT THIS........................................................
            randomClassPicks.RemoveAt(index); // Remove chosen element.
        }

*/
        // add those cards to a new list of cards. (probably of the Dialogue System Item type.)

        /*
        // Iterate through the list with a foreach, and point that at the CardData method like so:
        foreach (Item card in TO-BE-DETERMINED LIST)
        {
            if (card.FieldExists("isCard"))
            {
                cardCountSeparator++;
                CardData(card, cardCountSeparator);
            }
        }
        */
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