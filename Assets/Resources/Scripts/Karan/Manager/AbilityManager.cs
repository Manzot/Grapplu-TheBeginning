using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager
{
    PlayerController player;
    Ability[] abilities;
    
    #region Singleton
    private static AbilityManager instance;
    public AbilityManager() { }
    public static AbilityManager Instance { get { return instance ?? (instance = new AbilityManager()); } }
    #endregion

    public void Initialize(PlayerController _player)
    {
        player = _player;
        abilities = new Ability[4];
    }

    public void PhysicsRefresh()
    {
        for(int i = 0; i < abilities.Length; i++) {
        
        }
    }

    public void PostInitialize()
    {
        throw new System.NotImplementedException();
    }

    public void Refresh()
    {
        throw new System.NotImplementedException();
    }
    public void UseAbility()
    {

    }
    public void ReleaseAbility()
    {

    }


}
