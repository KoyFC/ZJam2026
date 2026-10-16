using UnityEngine;
using System.Collections.Generic;

public class PlayerAbilities : Singleton<PlayerAbilities>
{
    #region References
    [Header("References")]
    [SerializeField] private Player_Stats m_playerStats;
    #endregion
    
    #region Player Abilities
    private bool m_canDash = true;

    #endregion

    #region Boss Abilities
    //private List<BossAbility> m_bossAbilities = new List<BossAbility>();
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region Abilities Methods
    private void TryDash()
    {
        if(!m_canDash || m_playerStats.CurrentDashes <= 0)
        {
            return;
        }

        //execute dash logic here
        //the dash is a barrel roll that moves the player in the direction they are pressing, and can be used to dodge attacks or move quickly across the map.
    }
    #endregion
}
