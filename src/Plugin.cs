using System;
using BepInEx;
using UnityEngine;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;

namespace SlugTemplate
{
    [BepInPlugin(MOD_ID, "Webs Slugcat", "0.1")]
    class Plugin : BaseUnityPlugin
    {
        private const string MOD_ID = "Kezia-Knrc_Webs";

        public static readonly PlayerFeature<float> SuperJump = PlayerFloat("Webs/super_jump");
        public static readonly PlayerFeature<bool> SpiderSpit = PlayerBool("Webs/spider_spit");
        public static readonly PlayerFeature<float> CrawlSpeed = PlayerFloat("Webs/crawl_speed");

        private float crawlSpeed = 3f;

        // Add hooks
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

            //Hooks
            On.Player.Jump += Player_Jump;
            On.Player.UpdateBodyMode += Player_UpdateBodyMode; //For Movement (crawl, move, stand)
        }
        
        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
        }


        // Implement SuperJump
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
            SpiderSuperCrawl(self);// effecting movement speed
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
    }
}