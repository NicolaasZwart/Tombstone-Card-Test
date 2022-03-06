using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardStatsNamespace
{
    public class CardStatEntry 
    {
        private int abilityBonus;
        private string abilityName;

        public CardStatEntry() 
        {
            abilityBonus = 0;
            abilityName = "";
        }
        
        public CardStatEntry(int bonusValue, string abilityName) 
        {
            this.abilityBonus = bonusValue;
            this.abilityName = abilityName;
        }

        public int AbilityBonus 
        {
            get { return abilityBonus; }
            set { abilityBonus = value; }
        }
                
        public string AbilityName 
        {
            get { return abilityName; }
            set { abilityName = value; }
        }

        public override string ToString()
        {
            return abilityBonus + " " + abilityName;
        }

    }
}
