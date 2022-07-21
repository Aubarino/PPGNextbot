using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public struct IngameRegist
{
    public static void Regist(){ //used to register nextbots ingame via the nextbot class

        NextCategory.RegistCategory();

        //add your nextbot(s) here, can also be a single nextbot if you want, i'm just showing 2 examples of 'em
        RegistEntity("Backrooms Entity", //backrooms style entity
            "The Backrooms entity from the Backmaps mod.",
            NextbotAssets.GetNextbotSprite("entity_icon"), "ReflecCreateEntity"); //the icon
        RegistEntity("Sanic", //SANIC
            "The classic Sanic Nextbot from Gmod",
            NextbotAssets.GetNextbotSprite("sanic_icon"), "ReflecCreateSanic"); //the icon

    }

    public static void RegistEntity(string Name, string Desc, Sprite TheThumbnail, string EntMethod){
        ModAPI.Register(
            new Modification(){
                OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                NameOverride = Name,
                NameToOrderByOverride = "Nextbot_" + Name,
                DescriptionOverride = Desc,
                CategoryOverride = ModAPI.FindCategory("Nextbots"), //DO NOT TOUCH THIS BIT OF CODE
                ThumbnailOverride = TheThumbnail,
                AfterSpawn = (Instance)=> {
                    NextbotAssets.DoMethod(EntMethod,"EntIndexSpawning");
                    UnityEngine.Object.Destroy(Instance);
                }
            }
        );
    }
}
