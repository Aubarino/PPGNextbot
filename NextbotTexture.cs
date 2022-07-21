using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

//type used to define Textures

//DO NOT TOUCH THIS CODE
//DO NOT TOUCH THIS CODE
//DO NOT TOUCH THIS CODE
//DO NOT TOUCH THIS CODE
//DO NOT TOUCH THIS CODE
//DO NOT TOUCH THIS CODE
//DO NOT TOUCH THIS CODE
public class NextbotTexture : IComparable<NextbotTexture>
{
    public string name;
    public Sprite sprite;
    public Sprite[] spriteArray;

    public Sprite RngSprite(){ //returns random sprite
        if (spriteArray == null){
            Debug.LogWarning("spriteArray of NextbotTexture " + name + " returns null!");
        }
        return(spriteArray[(int) UnityEngine.Random.Range(0, spriteArray.Length)]);
    }

    public NextbotTexture(string NewName, Sprite NewSprite, Sprite[] NewSpriteArray = null)
    {
        name = NewName;
        sprite = NewSprite;
        spriteArray = NewSpriteArray;
    }

    //required by IComparable for some reason
    public int CompareTo(NextbotTexture other)
    {
        if(other == null)
        {
            return 1;
        }
        return 0;
    }
}

//an entry used in lists of SFX's, generally used for a global SFX list to pull everywhere.
public class SFXEntry : IComparable<SFXEntry>
{
    public AudioClip clip;
    public AudioClip[] clips;
    public string name;

    public SFXEntry(string NewName,AudioClip NewClip = null, AudioClip[] NewClips = null)
    {
        name = NewName; //required
        if (NewClips == null)clip = NewClip;
        else clips = NewClips;
    }

    public AudioClip RngClip(){ //returns random AudioClip from the AudioClips in the SFXEntry.
        if (clips == null){
            Debug.LogWarning("SFXEntry " + name + " RngClip attempt but does not contain multiple!");
        }
        return(clips[(int) UnityEngine.Random.Range(0, clips.Length - 1)]);
    }

    //required by IComparable for some reason
    public int CompareTo(SFXEntry other)
    {
        if(other == null)
        {
            return 1;
        }
        return 0;
    }
}