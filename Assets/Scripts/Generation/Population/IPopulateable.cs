using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    public interface IPopulateable
    {
        IEnumerable<IPopulateable> Populate(LevelGenerator generator);
    }

}