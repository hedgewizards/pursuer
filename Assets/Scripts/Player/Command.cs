using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CmdType {none, spell, inputOverride};

public struct Command
{
    public CmdType cmdType;
    public string[] argv;

    //static property for an empty command
    public static Command none
    {
        get
        {
            Command cmd = new Command
            {
                cmdType = CmdType.none,
                argv = new string[0]
            };
            return cmd;
        }
    }
    
}
