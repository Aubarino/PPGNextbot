using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public struct NextbotAssets //the main assets and functions script of the nextbot mod
{
    public static Sprite ControlSprite; //used as a control sprite for pixelsPerUnit 'n etc, ignore this, needed though.
    public static float PixPerUnit; //uses ControlSprite, needed
    public static List<NextbotTexture> NextbotTextures; //the list of all textures
    public static List<SFXEntry> SFX; //the list of all sfx
    public static string ModDirect = ModAPI.Metadata.MetaLocation; //the mod file path location, ignore this, needed i think.

    public static void Init(){ //the init setup of the mod's assets
        ControlSprite = ModAPI.LoadSprite("control.png");
        PixPerUnit = ControlSprite.pixelsPerUnit;

        SetupNextbotTextures();
        SetupNextbotSFX();
    }

    public static void SetupNextbotTextures(){ //sets up Nextbot textures
        NextbotTextures = new List<NextbotTexture>();

        NextbotTexRegist("EntityStand",ModAPI.LoadSprite("assets/EntityStand_1.png"),LoadSpriteArray("assets/EntityStand_", 6));
        NextbotTexRegist("EntityRun",ModAPI.LoadSprite("assets/EntityRun_1.png"),LoadSpriteArray("assets/EntityRun_", 8));
        NextbotTexRegist("sanic", ModAPI.LoadSprite("assets/sanic.png"));

        NextbotTexRegist("entity_icon", ModAPI.LoadSprite("assets/entity_icon.png"));
        NextbotTexRegist("sanic_icon", ModAPI.LoadSprite("assets/sanic_icon.png"));
    }

    public static void SetupNextbotSFX(){ //sets up Nextbot sfxs
        SFX = new List<SFXEntry>();

        RegistSFX("EntityNoise", ModAPI.LoadSound("assets/EntityNoise.mp3"));
        RegistSFX("EntityScream", ModAPI.LoadSound("assets/EntityScream.mp3"));
        RegistSFX("Sanic", ModAPI.LoadSound("assets/Sanic.mp3"));
    }

    public static void DoMethod(string TheMethod, string TheClass){ //a reflection script ported over from the backmaps mod i made
        object[] methValues = new object[]{};

        var ClassType = Type.GetType(TheClass);
        ConstructorInfo MethConstructor = ClassType.GetConstructor(Type.EmptyTypes);
        var levelClassObject = MethConstructor.Invoke(new object[]{});

        MethodInfo LevMethodInf = ClassType.GetMethod(TheMethod);
        object MethValue = LevMethodInf.Invoke(levelClassObject, methValues);
    }

    //sfx related stuff
    public static AudioClip GetSFX(string Name){
        SFXEntry ThisEntry = SFX.Find(x => x.name == Name);
        if (ThisEntry.clip == null) Debug.LogWarning("Nextbot SFX " + Name + " does not contain a single clip!?");
        return(ThisEntry.clip);
    }
    public static AudioClip[] GetSFXArray(string Name){
        SFXEntry ThisEntry = SFX.Find(x => x.name == Name);
        if (ThisEntry.clips == null) Debug.LogWarning("Nextbot SFX Array " + Name + " does not contain an Array!");
        return(ThisEntry.clips);
    }
    public static SFXEntry GetSFXEntry(string Name){
        SFXEntry ThisEntry = SFX.Find(x => x.name == Name);
        return(ThisEntry);
    }
    public static AudioClip GetRandomSFX(string Name){
        SFXEntry ThisEntry = SFX.Find(x => x.name == Name);
        if (ThisEntry.clips == null) Debug.LogWarning("Nextbot SFX Array " + Name + " does not contain an Array to pick randomly from!");
        return(ThisEntry.RngClip());
    }

    public static void RegistSFX(string Name, AudioClip TheClip){ //used to add SINGLE sfx entries
        SFXEntry ThisSFXEntry = new SFXEntry(Name, TheClip);
        SFX.Add(ThisSFXEntry);
        Debug.Log("Nextbot SFX " + Name + " Registered (single)");
    }
    public static void RegistSFXArray(string Name, AudioClip[] TheClips){ //used to add a single sfx entry containing multiple AudioClips
        SFXEntry ThisSFXEntry = new SFXEntry(Name, null, TheClips);
        SFX.Add(ThisSFXEntry);
        Debug.Log("Nextbot SFX " + Name + " Registered (multiple)");
    }

    //texture conversation stuff
    public static Texture2D SpriteToTexture(Sprite TheSprite){;
        return(TheSprite.texture);
    }

    //texture stuff
    public static void NextbotTexRegist(string Name, Sprite TheSprite, Sprite[] SpriteArray = null){ //used to add new map textures
        NextbotTexture ThisNextbotTexture = new NextbotTexture(Name, TheSprite, SpriteArray);
        NextbotTextures.Add(ThisNextbotTexture);
    }
    public static NextbotTexture GetNextbotTexEntry(string Name){
        NextbotTexture ThisEntry = NextbotTextures.Find(x => x.name == Name);
        return(ThisEntry);
    }
    public static Sprite GetNextbotSprite(string Name){
        NextbotTexture ThisEntry = NextbotTextures.Find(x => x.name == Name);
        return(ThisEntry.sprite);
    }

    public static Sprite[] LoadSpriteArray(string Name, int Amount, int Scale = 1){
        Sprite[] tempSpriteArray = new Sprite[] {};
        for(int i = 1; i <= (Amount); i++)
        { tempSpriteArray = tempSpriteArray.Concat(new[] {ModAPI.LoadSprite(Name + (i) + ".png", Scale)}).ToArray();
        }
        return(tempSpriteArray);
    }

    //sfx stuff
    public static AudioClip[] LoadClipArray(string Name, int Amount){
        AudioClip[] tempAudioArray = new AudioClip[] {};
        for(int i = 0; i < (Amount); i++)
        {
            tempAudioArray = tempAudioArray.Concat(new[] {ModAPI.LoadSound(Name + (i) + ".mp3")}).ToArray();
        }
        return(tempAudioArray);
    }

    //since recently decided to make it default to the camera position cause eh why not
    public static void PlaySFXarray(AudioClip[] Audio, Vector3 Transform){
        PlaySFX(Audio[(int) UnityEngine.Random.Range(0, (Audio.Length - 1))], Transform);
    }

    public static void PlaySFX(AudioClip Audio, Vector3 Transform){
        AudioSource.PlayClipAtPoint(Audio, Transform);
    }
}
