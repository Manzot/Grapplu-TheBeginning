using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour {
    PlayerController player;
    public AbilityStats stats;
    public Ability(PlayerController _player) {
        player = _player;
    }
    public virtual void AbilityRelease()
    {

    }
   
    public virtual bool UseAbility()
    {
        if (stats.canUseAbility)
        {
            stats.timeAbilityLastUsed = Time.time;
          
            return true;
        }
        return false;
    }

    public class AbilityStats
    {
        public Ability abilityParent;
        public Abilities abilityType;
        public float timeAbilityLastUsed;
        public float energyCost = 1;
        public float cooldown = 1;
        public bool canUseAbility { get { return Time.time - timeAbilityLastUsed >= cooldown; } }
        public float chargePercentage { get { return (Time.time - timeAbilityLastUsed) / cooldown; } }

        public AbilityStats(Ability _abilityParent, Abilities _abilityType, float _energyCost, float _cooldown)
        {
            this.abilityParent = _abilityParent;
            this.abilityType = _abilityType;
            this.energyCost = _energyCost;
            this.cooldown = _cooldown;
        }
    }
}
