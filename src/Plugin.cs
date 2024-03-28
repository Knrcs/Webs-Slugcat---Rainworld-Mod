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
using Webs.WebsThings;


namespace Webs
{
    [BepInDependency("slime-cubed.slugbase")]
    [BepInDependency("dressmyslugcat", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("Kezia-Knrc_Webs", "Webs Slugcat", "0.1.6")]
    class Plugin : BaseUnityPlugin
    {
        public static readonly PlayerFeature<float> SuperJump = PlayerFloat("Webs/super_jump");
        public static readonly PlayerFeature<bool> SpiderSpit = PlayerBool("Webs/spider_spit");
        public static readonly PlayerFeature<float> CrawlSpeed = PlayerFloat("Webs/crawl_speed");
        public static bool IsPostInit;
        // public string atlasPath = "atlases/spiderlegs";

        #region hooks
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);
            //Hooks
            WebsJump.Apply();
            WebsSpit.Apply();
            WebsCrawl.Apply();
            On.PlayerState.ctor += PlayerState_ctor;
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

                Debug.Log($"Plugin Kezia-Knrc_Webs is loaded!");
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


        
    }
    #endregion
}






//Credits to the Spider Spit Code function
//RezilloRyker 