using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using System.Linq;

//~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~
//Nextbot Base code main mod script
//Full Credit to Aubarino
// ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~
namespace Mod
{
    public class Mod
    {
        public static void Main()//ran at the initialization of the mod when in the map, used for main mod setup
        {
            NextbotAssets.Init(); //must run before IngameRegist.Regist();
            IngameRegist.Regist();
        }
	}
}