using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;
using BepInEx;
using UnityEngine;
using SlugBase;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using DressMySlugcat;
using On;
using MonoMod.RuntimeDetour;


namespace Webs
{
    [BepInDependency("slime-cubed.slugbase")]
    [BepInDependency("dressmyslugcat", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("Kezia-Knrc_Webs", "Webs Slugcat", "0.1.4.3")]
    class Plugin : BaseUnityPlugin
    {
        public static readonly PlayerFeature<float> SuperJump = PlayerFloat("Webs/super_jump");
        public static readonly PlayerFeature<bool> SpiderSpit = PlayerBool("Webs/spider_spit");
        public static readonly PlayerFeature<float> CrawlSpeed = PlayerFloat("Webs/crawl_speed");
        //TODO: Red Spider Spit
        //Maximum Ammount of SpitAmmo the Player can have
        private int maxSpiderSpitCapacity = 4;

        //Capacity the player currently have
        private int playerA_SpiderSpitCapacity = 4;
        private int playerB_SpiderSpitCapacity = 4;
        private int playerC_SpiderSpitCapacity = 4;
        private int playerD_SpiderSpitCapacity = 4;



        //Time until Spit Regenerates
        private float resetCooldown = 5f;
        private float playerA_spitCooldown = 5f;
        private float playerB_spitCooldown = 5f;
        private float playerC_spitCooldown = 5f;
        private float playerD_spitCooldown = 5f;

        //Checks if player can Spit or not
        private bool playerA_canSpit; 
        private bool playerB_canSpit; 
        private bool playerC_canSpit; 
        private bool playerD_canSpit; 


        //TODO: Climb on Walls
        //TODO: Climb on Ceilings


        public static bool IsPostInit;
        public string atlasPath = "atlases/spiderlegs";



        // Add hooks
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

            //Hooks
            On.Player.Jump += Player_Jump;
            On.Player.GrabUpdate += GrabUpdate;
            On.PlayerState.ctor += PlayerState_ctor;
            On.Player.UpdateBodyMode += Player_UpdateBodyMode; //For Movement (crawl, move, stand)

            if (ModManager.ActiveMods.Any(mod => mod.id == "dressmyslugcat")) 
            {
                SetupDMSSprites();
            }
            On.RainWorld.PostModsInit += RainWorld_PostModsInit;

            Hook overseercolorHook = new Hook(typeof(OverseerGraphics).GetProperty("MainColor", BindingFlags.Instance | BindingFlags.Public).GetGetMethod(), OverseerGraphics_MainColor_get);
        }

        //Change Overseer Color
        public delegate Color orig_OverseerMainColor(OverseerGraphics self);
        public static Color OverseerGraphics_MainColor_get(orig_OverseerMainColor orig, OverseerGraphics self)
        {
            Color res = orig(self); // REQUIRED or else default overseers won't have colors
            if ((self.overseer.abstractCreature.abstractAI as OverseerAbstractAI).ownerIterator == 1) //Overseer ID 
            {
                res = new Color(0.298f, 0.329f, 0.765f); //Color of the Overseer
            }
            return res;
        }

        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
            Futile.atlasManager.LoadAtlas("atlases/spiderlegs");
        }

        private void PlayerState_ctor(On.PlayerState.orig_ctor orig, PlayerState self, AbstractCreature crit, int playerNumber, SlugcatStats.Name slugcatCharacter, bool isGhost)
        {
            orig(self, crit, playerNumber, slugcatCharacter, isGhost);
        }
        private void RainWorld_PostModsInit(On.RainWorld.orig_PostModsInit orig, RainWorld self)
        {
            orig(self);
            try
            {
                if (IsPostInit) return;
                IsPostInit = true;

                ////-- Creating a blank slugbase slugcat, you don't need this if you are using the json
                //var templateCat = SlugBaseCharacter.Create("Kezia-Knrc_Webs");
                //templateCat.DisplayName = "Webs via this code";
                //templateCat.Description = "A lost soul, weak with spears, but strong with teeth.<LINE>It must go to the farest East.";

                //-- You can have the DMS sprite setup in a separate method and only call it if DMS is loaded
                //-- With this the mod will still work even if DMS isn't installed
                if (ModManager.ActiveMods.Any(mod => mod.id == "dressmyslugcat"))
                {
                    SetupDMSSprites();
                }

                Debug.Log($"Plugin dressmyslugcat.Kezia-Knrc_Webs is loaded!");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        public void SetupDMSSprites()
        {

            //Didn't managed to get that working
            //SpriteDefinitions.AddSprite(new SpriteDefinitions.AvailableSprite
            //TODO: - Add spider legs
            //{
            //    Name = "SPIDERLEGS",
            //    Description = "Spider Legs",
            //    GallerySprite = "spiderlegs",
            //    RequiredSprites = new List<string>
            //    {
            //        "spiderlegs"
            //    },
            //    Slugcats = new List<string>
            //    {
            //        "Webs"
            //    }
            //});

            //ID of the Spritesheet 
            var sheetID = "Kezia-Knrc_Webs";

            // Each player slot (0 - 3) can be customized individually
            for (int i = 0; i < 4; i++)
            {
                SpriteDefinitions.AddSlugcatDefault(new Customization()
                {
                    Slugcat = "Webs",
                    PlayerNumber = i,
                    CustomSprites = new List<CustomSprite>
                    {
                        //Customize Spritesheet
                        new CustomSprite() { Sprite = "HEAD", SpriteSheetID = sheetID },
                        new CustomSprite() { Sprite = "FACE", SpriteSheetID = sheetID },
                        new CustomSprite() { Sprite = "BODY", SpriteSheetID = sheetID },
                        new CustomSprite() { Sprite = "ARMS", SpriteSheetID = sheetID },
                        new CustomSprite() { Sprite = "HIPS", SpriteSheetID = sheetID },
                        new CustomSprite() { Sprite = "LEGS", SpriteSheetID = sheetID },
                        new CustomSprite() { Sprite = "TAIL", SpriteSheetID = sheetID },
                        new CustomSprite() { Sprite = "PIXEL", SpriteSheetID = sheetID }

                    },
                    CustomTail = new CustomTail()
                    {
                        Length = 5f,
                        Wideness = 1.7f,
                        Roundness = 0.4f
                    }
                });
            }
        }

        //--Setup Spiderlegs
        //public void InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        //{
     
        //}
        
        //public void DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos) 
        //{
         
        //}

        //--Implement SuperJump
        private void Player_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);

            if (SuperJump.TryGet(self, out var power))
            {
                self.jumpBoost *= 1f + power;
            }
        }

        private void Player_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self)
        {
            orig(self);
            SpiderSuperCrawl(self);//effecting movement speed
        }

        public void SpiderSuperCrawl(Player self)// Changing movment speeds with crawling or walking
        {
            if (CrawlSpeed.TryGet(self, out var power))
            {
                if (self.bodyMode == Player.BodyModeIndex.Crawl)
                {
                    self.dynamicRunSpeed[0] *= power;
                    self.dynamicRunSpeed[1] *= power;
                }
                //if (self.bodyMode == Player.BodyModeIndex.Default)
                //{
                //    self.dynamicRunSpeed[0] *= 1.5f;
                //    self.dynamicRunSpeed[1] *= 1.5f;
                //}
                ////if (self.standing)
                //if (self.bodyMode == Player.BodyModeIndex.Stand)
                //{
                //    self.dynamicRunSpeed[0] *= 0.7f;
                //    self.dynamicRunSpeed[1] *= 0.7f;
                //}
            }
        }

        private void GrabUpdate(On.Player.orig_GrabUpdate orig, Player self, bool eu) // Player grab update check
        {
            //THIS METHOD IS RESPONSIBLE FOR THROWING HELD OBJECTS, SO WE WANT YOUR THING TO HAPPEN RIGHT BEFORE ALL THAT STUFF HAPPENS
            SlugcatSpit(self); 
            orig(self, eu); 
        }

        public void throwButtonCheckFalse(Player self)
        {
            if (self.playerState.playerNumber == 0) { playerA_canSpit = false; }
            if (self.playerState.playerNumber == 1) { playerB_canSpit = false; }
            if (self.playerState.playerNumber == 2) { playerC_canSpit = false; }
            if (self.playerState.playerNumber >= 3) { playerD_canSpit = false; }

        }

        public void throwButtonPressed(Player self)
        {
            if (playerA_canSpit && self.playerState.playerNumber == 0)
            {
                if (playerA_SpiderSpitCapacity == 0)
                {
                    playerA_canSpit = false;
                }
                else
                {
                    SpitProjectile(self);
                    playerA_SpiderSpitCapacity--;
                    playerA_canSpit = false;
                    Debug.Log("Spit happened");
                }
            }

            if (playerB_canSpit && self.playerState.playerNumber == 0)
            {
                if (playerB_SpiderSpitCapacity == 0)
                {
                    playerB_canSpit = false;
                }
                else
                {
                    SpitProjectile(self);
                    playerB_SpiderSpitCapacity--;
                    playerB_canSpit = false;
                }
            }

            if (playerC_canSpit && self.playerState.playerNumber == 0)
            {
                if (playerC_SpiderSpitCapacity == 0)
                {
                    playerC_canSpit = false;
                }
                else
                {
                    SpitProjectile(self);
                    playerC_SpiderSpitCapacity--;
                    playerC_canSpit = false;
                }
            }

            if (playerD_canSpit && self.playerState.playerNumber == 0)
            {
                if (playerD_SpiderSpitCapacity == 0)
                {
                    playerD_canSpit = false;
                }
                else
                {
                    SpitProjectile(self);
                    playerD_SpiderSpitCapacity--;
                    playerD_canSpit = false;
                    
                }
            }
        }


        public void SpitRecharge(Player self) 
        {
            if(playerA_SpiderSpitCapacity < maxSpiderSpitCapacity && self.playerState.playerNumber == 0)
            {
                playerA_spitCooldown -= Time.deltaTime;
                    if(playerA_spitCooldown <= 0)
                    {   
                        playerA_SpiderSpitCapacity++;
                        playerA_spitCooldown = resetCooldown;
                        Debug.Log("Reload Spit");
                }
            }
            if (playerB_SpiderSpitCapacity < maxSpiderSpitCapacity && self.playerState.playerNumber == 1)
            {
                playerB_spitCooldown -= Time.deltaTime;
                if (playerB_spitCooldown <= 0)
                {
                    playerB_SpiderSpitCapacity++;
                    playerB_spitCooldown = resetCooldown;
                }
            }
            if (playerC_SpiderSpitCapacity < maxSpiderSpitCapacity && self.playerState.playerNumber == 2)
            {
                playerC_spitCooldown -= Time.deltaTime;
                if (playerC_spitCooldown <= 0)
                {
                    playerC_SpiderSpitCapacity++;
                    playerC_spitCooldown = resetCooldown;
                }
            }
            if (playerD_SpiderSpitCapacity < maxSpiderSpitCapacity && self.playerState.playerNumber >= 3)
            {
                playerD_spitCooldown -= Time.deltaTime;
                if (playerD_spitCooldown <= 0)
                {
                    playerD_SpiderSpitCapacity++;
                    playerD_spitCooldown = resetCooldown;
                }
            }
        }

        public void CanSpitCheck(Player self)
        {
            if (playerA_SpiderSpitCapacity > 0 && self.playerState.playerNumber == 0)
            {
                playerA_canSpit = true;
                Debug.Log("Can Spit");
            }
            if (playerB_SpiderSpitCapacity > 0 && self.playerState.playerNumber == 1)
            {
                playerB_canSpit = true;
            }
            if (playerC_SpiderSpitCapacity > 0 && self.playerState.playerNumber == 2)
            {
                playerC_canSpit = true;
            }
            if (playerD_SpiderSpitCapacity > 0 && self.playerState.playerNumber >= 3)
            {
                playerD_canSpit = true;
            }
        }

        public void SlugcatSpit(Player self)
        {
            if (SpiderSpit.TryGet(self, out var spiderSpit))
            {
                if ((self.grasps[0] != null || self.grasps[1] != null) && self.input[0].thrw)
                {
                    throwButtonCheckFalse(self);
                    Debug.Log("Spit");
                }
                if (!self.input[1].thrw && self.input[0].thrw)
                {
                    throwButtonPressed(self);
                }
            }
            SpitRecharge(self);
            if (!self.input[0].thrw)
            {
                CanSpitCheck(self);
            }
        }

        public void SpitProjectile(Player self)
        {
            AbstractPhysicalObject abstractPhysicalObject = new AbstractPhysicalObject(self.room.world, AbstractPhysicalObject.AbstractObjectType.DartMaggot, null, self.abstractCreature.pos, self.room.game.GetNewID());
            abstractPhysicalObject.RealizeInRoom();
            if (self.input[0].x == 0 && self.input[0].y == 0) //checking if using direction axis
            {
                (abstractPhysicalObject.realizedObject as DartMaggot).Shoot(self.mainBodyChunk.pos, new(self.ThrowDirection, 0), self); //shooting dart and if not moving uses throw direction
            }
            else
            {
                (abstractPhysicalObject.realizedObject as DartMaggot).Shoot(self.mainBodyChunk.pos, new(self.input[0].x, self.input[0].y), self); //shoting dart direction to movement inputs
            }
            self.room.PlaySound(SoundID.Big_Spider_Spit, self.mainBodyChunk);
            Debug.Log("Projectile Spit");
        }
    }
}

//Credits to the Spider Spit Code function
//RezilloRyker 