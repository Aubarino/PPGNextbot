using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EntIndexSpawning //define the methods used to spawn each entity ingame
{
    public void ReflecCreateEntity(){ //creates a backrooms entity
        Nextbot tempEnt = NextbotManager.CreateEntity("Entity",
        Global.main.camera.ScreenToWorldPoint(Input.mousePosition)
        );
    }

    public void ReflecCreateSanic(){ //creates SANIC
        Nextbot tempEnt = NextbotManager.CreateSanic(Global.main.camera.ScreenToWorldPoint(Input.mousePosition));
    }
}

public static class NextbotManager //define the internal methods used to spawn each entity, EntIndexSpawning is simply a shell ontop of this that is needed.
{
    public static Nextbot CreateEntity(string EntName, Vector2 EntPos, bool Complex = false){ //defines a backrooms entity
        GameObject NewEnt = ModAPI.CreatePhysicalObject(EntName, NextbotAssets.GetNextbotTexEntry("EntityStand").spriteArray[0]);
        NewEnt.transform.localPosition = EntPos;
        PhysicalBehaviour Phys = NewEnt.GetComponent<PhysicalBehaviour>();
        Phys.Properties = ModAPI.FindPhysicalProperties("Soft");
        Phys.InitialMass = 35f;
        Phys.TrueInitialMass = 35;
        Phys.rigidbody.mass = 35;
        Phys.rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        Nextbot EntityNextbotClass = NewEnt.AddComponent<Nextbot>();
        EntityNextbotClass.TrackRange = 12.5f;
        EntityNextbotClass.StandSprites = NextbotAssets.GetNextbotTexEntry("EntityStand").spriteArray;
        EntityNextbotClass.RunSprites = NextbotAssets.GetNextbotTexEntry("EntityRun").spriteArray;
        EntityNextbotClass.StandFPS = 0.1428f;
        EntityNextbotClass.RunFPS = 0.0625f;
        EntityNextbotClass.Complex = true;

        EntityNextbotClass.IdleSFX = NextbotAssets.GetSFX("EntityNoise");
        EntityNextbotClass.TargetSFX = NextbotAssets.GetSFX("EntityScream");

        return(EntityNextbotClass);
    }

    public static Nextbot CreateSanic(Vector2 EntPos){ //defines SANIC
        Nextbot Sanic = CreateEntity("Sanic", EntPos, false);
        Sanic.Complex = true;
        Sanic.EntAlternate = true;
        Sanic.EntityID = "Sanic";
        Sanic.DefaultSprite = NextbotAssets.GetNextbotSprite("sanic");
        Sanic.AttackDamge = 99999;
        Sanic.TrackRange = 60;
        Sanic.AttackingRange = 4f;
        Sanic.Speed = 20;
        Sanic.AttackPower = 60;
        Sanic.StopInRange = false;
        Sanic.TargetSFX = NextbotAssets.GetSFX("Sanic");
        Sanic.NextBotJoke = true;
        return(Sanic);
    }
}

public class Nextbot : MonoBehaviour //the main actual behaviour of the nextbot, recommend not touching this code without c# skill.
{
    public bool Walking = false; //if the entity is walking or not
    public Vector3 HeadHeight = new Vector3(0,0); //the height used for target detection
    public bool Target = false; //if it has a target, very simular to Walking.
    public int TargetDir = 0; //the direction the Entity is facing, changes from 0, -1 and 1 according to the direction of the last target, normally is facing right i think..
    public float TrackRange = 12.5f; //the range in unity units that the entity tracks targets for and also checks within.
    public Sprite[] StandSprites; //the sprites for Standing
    public float StandFPS = 0.1428f; //the fps for standing sprites (FPS is actually just a fraction of a second, aka 0.25f would be 4 times a second.)
    public Sprite[] RunSprites; //the sprites for Running
    public float RunFPS = 0.0625f; //the fps for the running sprites
    public Sprite DefaultSprite;
    public float AniTimer = 0; //used with the time, to do a simple loop to count along an FPS according to a division of a second.
    public int AniFrame = -1; //the frame currently being used in the animation, i recommend not touching this?
    public bool Complex = false; //set this to true if you want the entity to use any sort of complex setup, aka animated.
    private SpriteRenderer Renderer;
    private PhysicalBehaviour Phys;
    public float CurrentFPS; //the current FPS used in the animation, i recommend not touching this?
    public Sprite[] CurrentSprites; //the current Sprites used in the animation, i recommend not touching this?
    public float CurrentViewDistanceHit; //set by whenever it finds a target, to the Distance.
    public float Speed = 9.5f; //the speed of the entity when running / walking.
    public float AttackingRange = 2f; //the range the entity can Attack from;
    public bool CanAttack = true; //if the entity can Attack or not
    public bool RunAtTarget = true; //runs towards a target
    public float AttackPower = 25; //the power of the punch / attack done to a target if attacking.
    public float AttackDamge = 100; //the damage done when attacking
    public bool StopInRange = true; //makes it stop walking towards the target when within the attack range doesn't need to attack to do this! can be used to make passive friendly entities i guess???
    public AudioClip IdleSFX; //the audio of when it is idle
    public AudioClip TargetSFX; //the audio of when it is attacking a target
    public AudioSource AudioSource;
    public string EntityID = "Entity"; //used for hard-coded interactions
    public float SmilerDetectRange = 2f; //carry-over from the backmaps mod
    public bool HasGravity = true; //if it has gravity or floats
    public bool EntAlternate = false; //alternate entity management
    public bool NextBotJoke = false; //if it acts like a PNG nextbot
    private int StepsTaken = 0;
    private int StepsBehindWall = 0;
    public bool ActLikeAngel = false; //act like scp-173 or a wheeping angel
    public bool DeleteTargetOnAttack = false; //delete the target if attacking it

    public void Start(){
        Renderer = gameObject.GetComponent<SpriteRenderer>();
        Phys = gameObject.GetComponent<PhysicalBehaviour>();
        if (Complex && !EntAlternate){
            Renderer.sprite = StandSprites[0];
        }else{
            Renderer.sprite = DefaultSprite;
            Phys.RefreshOutline();
        }
        gameObject.FixColliders();

        gameObject.AddComponent<DeregisterBehaviour>();
        AudioSource = gameObject.AddComponent<AudioSource>();
        gameObject.AddComponent<AudioSourceTimeScaleBehaviour>();
        AudioSource.loop = true;
        AudioSource.playOnAwake = false;

        if(EntAlternate){
            AudioSource.clip = TargetSFX;
            AudioSource.maxDistance = 2;
            AudioSource.volume = 2;
            AudioSource.Play();
        }

        if (!HasGravity){
            Phys.rigidbody.gravityScale = 0;
        }
    }

    public void FixedUpdate(){
        
        if (Complex || EntAlternate){
            TargetCheck();
            if(TargetDir == 0) gameObject.transform.localScale = new Vector2(1,1);
            else gameObject.transform.localScale = new Vector2(TargetDir,1);

            if (Walking){ //basic shit script I made in a minute, There's only 2 States so does it matter? no? ok then. SHUTUP! :)
                if (StopInRange && (CurrentViewDistanceHit <= (AttackingRange))){
                    if (NextBotJoke) StepsTaken = 0;
                    Phys.rigidbody.velocity = new Vector2(0, Phys.rigidbody.velocity.y);
                }else{
                    Move();
                    if (!EntAlternate){
                        if (CurrentSprites != RunSprites){
                            CurrentSprites = RunSprites;
                            CurrentFPS = RunFPS;
                            AniFrame = 0;

                            AudioSource.clip = TargetSFX;
                            AudioSource.maxDistance = 10;
                            AudioSource.Play();
                        }
                    }
                }
            }else{
                if (!EntAlternate){
                    if (CurrentSprites != StandSprites){
                        CurrentSprites = StandSprites;
                        CurrentFPS = StandFPS;
                        AniFrame = 0;

                        AudioSource.clip = IdleSFX;
                        AudioSource.maxDistance = 5;
                        AudioSource.Play();
                    }
                }
            }

            if (!EntAlternate){
                if (UnityEngine.Time.fixedTime > AniTimer){
                    AniTimer = (UnityEngine.Time.fixedTime + CurrentFPS);
                    AniFrame ++;
                    if (AniFrame >= (CurrentSprites.Length - 1))AniFrame = 0;
                    Renderer.sprite = CurrentSprites[AniFrame];
                    Phys.RefreshOutline();
                }
            }else{
                Renderer.sprite = DefaultSprite;
            }
        }else{
            switch(EntityID) {
                case "Smiler":
                    Collider2D[] SmilerCirclehits = Physics2D.OverlapCircleAll(gameObject.transform.position, TrackRange);
                    bool SmilerAttack = false;
                    Vector2 SmilerTargetPos = Vector2.zero;
                    if (SmilerCirclehits != null){
                        foreach(Collider2D Col in SmilerCirclehits){
                            if (Col.gameObject.GetComponent<Rigidbody2D>() != null &&
                                Col.gameObject.GetComponent<Nextbot>() == null){

                                Rigidbody2D Rigi = Col.gameObject.GetComponent<Rigidbody2D>();
                                if (Rigi.constraints ==  RigidbodyConstraints2D.None){
                                    if (Rigi.velocity.magnitude > SmilerDetectRange){
                                        SmilerAttack = true;
                                        SmilerTargetPos = new Vector2((Col.gameObject.transform.position.x - gameObject.transform.position.x), (Col.gameObject.transform.position.y - gameObject.transform.position.y));
                                    }
                                }
                            }
                        }
                    }
                    if (SmilerAttack){
                        TargetDir = 1;
                        if (SmilerTargetPos.x <= 0) TargetDir = -1;
                        Phys.rigidbody.velocity = SmilerTargetPos;
                        RaycastHit2D[] SmilerAttackCheck = Physics2D.RaycastAll(gameObject.transform.position, SmilerTargetPos, AttackingRange, 1 << LayerMask.NameToLayer("Objects"), -UnityEngine.Mathf.Infinity, UnityEngine.Mathf.Infinity);
                        foreach(RaycastHit2D SmilerAttackCheckIn in SmilerAttackCheck){
                            if (SmilerAttackCheckIn.collider.gameObject.GetComponent<Rigidbody2D>() != null){
                                if (SmilerAttackCheckIn.collider.gameObject.GetComponent<Rigidbody2D>().constraints ==  RigidbodyConstraints2D.None
                                && SmilerAttackCheckIn.collider.gameObject.GetComponent<Nextbot>() == null){
                                CurrentViewDistanceHit = SmilerAttackCheckIn.distance;
                                AttackCheck(SmilerAttackCheckIn.collider.gameObject, AttackDamge);
                                }
                            }
                        }
                    }else{
                        Phys.rigidbody.velocity = new Vector2(Phys.rigidbody.velocity.x * 0.99f, Phys.rigidbody.velocity.y * 0.99f);
                    }
                break;
                default:Debug.LogWarning("No entity found with ID : " + EntityID); break;
            }
        }
    }

    public void Move(){
        if (NextBotJoke){
            if ((Mathf.Abs(Phys.rigidbody.velocity.x) < (Speed * 0.6f)) || ActLikeAngel){
                StepsBehindWall ++;
                if (StepsBehindWall > 50){
                    gameObject.transform.position = new Vector2(gameObject.transform.position.x + (TargetDir * 2.5f), gameObject.transform.position.y);
                    StepsBehindWall = 0;
                }
            }else{
                StepsTaken++;
            }
        }
        if (!ActLikeAngel) Phys.rigidbody.velocity = new Vector2(Speed * TargetDir,Phys.rigidbody.velocity.y);
    }

    public void AttackCheck(GameObject CheckObj, float Damage){
        if ((CurrentViewDistanceHit <= AttackingRange) && CanAttack){
            if (DeleteTargetOnAttack){
                UnityEngine.Object.Destroy(CheckObj);
            }else{
            var Rigi = CheckObj.GetComponent<PhysicalBehaviour>().rigidbody;
            Rigi.velocity = new Vector2(TargetDir * AttackPower,Rigi.velocity.y);
            CheckObj.SendMessage("Shot", new Shot(new Vector2(TargetDir * -1, 0),
                new Vector2(CheckObj.transform.position.x + UnityEngine.Random.Range(-0.5f, 0.5f), CheckObj.transform.position.y + UnityEngine.Random.Range(-0.5f, 0.5f)),
                Damage), SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void TargetCheck(){
        bool HitAny = false;
        RaycastHit2D[] ViewObjsRIGHT = Physics2D.RaycastAll(gameObject.transform.position + HeadHeight, gameObject.transform.right, TrackRange, 1 << LayerMask.NameToLayer("Objects"), -UnityEngine.Mathf.Infinity, UnityEngine.Mathf.Infinity);
        if (ViewObjsRIGHT != null)
            foreach(RaycastHit2D Hit in ViewObjsRIGHT){
                if (Hit.collider.gameObject.GetComponent<LimbBehaviour>() != null){
                    if (Hit.collider.gameObject.GetComponent<LimbBehaviour>().Health > 0.1){ //check if dead, if alive chase
                    Target = true;
                    if (RunAtTarget) Walking = true;
                    TargetDir = 1;
                    CurrentViewDistanceHit = Hit.distance;
                    HitAny = true;
                    AttackCheck(Hit.collider.gameObject, AttackDamge);
                    }
                }
            }
        RaycastHit2D[] ViewObjsLEFT = Physics2D.RaycastAll(gameObject.transform.position + HeadHeight, -gameObject.transform.right, TrackRange, 1 << LayerMask.NameToLayer("Objects"), -UnityEngine.Mathf.Infinity, UnityEngine.Mathf.Infinity);
        if (ViewObjsLEFT != null)
            foreach(RaycastHit2D Hit in ViewObjsLEFT){
                if (Hit.collider.gameObject.GetComponent<LimbBehaviour>() != null){
                    if (Hit.collider.gameObject.GetComponent<LimbBehaviour>().Health > 0.1){ //check if dead, if alive chase
                    Target = true;
                    if (RunAtTarget) Walking = true;
                    TargetDir = -1;
                    CurrentViewDistanceHit = Hit.distance;
                    HitAny = true;
                    AttackCheck(Hit.collider.gameObject, AttackDamge);
                    }
                }
            }
        if ((HitAny == false) && (Target = true)){
            Target = false;
            if (RunAtTarget) Walking = false;
        }
    }
}