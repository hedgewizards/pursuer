using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectReferencer : MonoBehaviour
{
    public GameObjectReference[] References;

    public GameObject FindByReferenceName(string ReferenceName)
    {
        foreach(GameObjectReference r in References)
        {
            if(ReferenceName == r.ReferenceName)
            {
                return r.gameObject;
            }
        }

        return null;
    }
}

[System.Serializable]
public class GameObjectReference
{
    public string ReferenceName;
    public GameObject gameObject;
}
