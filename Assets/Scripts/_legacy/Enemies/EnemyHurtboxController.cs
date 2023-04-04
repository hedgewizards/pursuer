using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHurtboxController : MonoBehaviour
{
    Collider player;

    /// <summary>
    /// Is the player in contact with this hurtbox?
    /// </summary>
    /// <returns></returns>
    public bool PlayerIsInContact()
    {
        return player != null;
    }

    /// <summary>
    /// Gets the Player touching this hurtbox (if it exists, else null)
    /// </summary>
    /// <returns></returns>
    public PlayerController GetPlayer()
    {
        if(PlayerIsInContact())
        {
            return player.GetComponent<PlayerController>();
        }
        else
        {
            return null;
        }
    }
  
    private void OnTriggerEnter(Collider other)
    {
        player = other;
    }

    private void OnTriggerExit(Collider other)
    {
        player = null;
    }
}
