using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus
{
    //this list should ALWAYS be sorted where soonest expireTime appears first
    List<StatusEffect> statusList;
    float nextExpireTime;

    public PlayerStatus()
    {
        statusList = new List<StatusEffect>();
        nextExpireTime = 0;
    }

    public void AddStatus(StatusEffect newEff)
    {
        //first check against all current effects and do interactions
        bool doAdd = true;
        foreach(StatusEffect eff in statusList)
        {
            Debug.Log(eff.ToString());
            if(eff.DoInteract(newEff))
            {
                doAdd = false;
                break;
            }
        }
        //if no interactions consumed the status, insert into the list
        if(doAdd)
        {
            //add the status to the list at the appropriate place based
            //on its expire time
            int index;
            float expireTime = newEff.GetExpireTime();
            for(index = 0; index < statusList.Count; index++)
            {
                //if the new object expires sooner
                if(expireTime < statusList[index].GetExpireTime())
                {
                    //insert it there and break
                    statusList.Insert(index, newEff);
                    //if this is the new first item, update expireTime
                    if (index == 0)
                    {
                        nextExpireTime = expireTime;
                    }
                    break;
                }
            }
            //if the list is empty, the above will not happen, so seperately add the item
            if(statusList.Count == 0)
            {
                statusList.Insert(0, newEff);
                nextExpireTime = expireTime;
            }
            newEff.DoApplyEffect();
        }
    }
    public void RemoveStatus(int index)
    {
        if (index < 0 || index >= statusList.Count)
        {
            throw new System.Exception("RemoveStatus out of range");
        }

        //update nextExpireTime if necessary (& possible)
        if(index == 0 && statusList.Count > 1)
        {
            nextExpireTime = statusList[1].GetExpireTime();
        }

        //pull the item from the list
        StatusEffect eff = statusList[index];
        statusList.RemoveAt(index);

        //remove effect from player
        eff.DoRemoveEffect();
    }
    public void Update()
    {
        //do update for each status effect
        foreach(StatusEffect eff in statusList)
        {
            eff.Update();
        }

        //if the first effect has expired...
        if(nextExpireTime < Time.time && statusList.Count > 0)
        {
            RemoveStatus(0); //remove that effect
        }
    }
}
