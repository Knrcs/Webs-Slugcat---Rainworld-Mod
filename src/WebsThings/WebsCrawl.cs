using System;
using RWCustom;
using UnityEngine;
using Webs.WebsThings;

namespace Webs.WebsThings
{
    internal class WebsCrawl
    {
        public static void Apply()
        {
            On.Player.UpdateBodyMode += Player_UpdateBodyMode; //For Movement (crawl, move, stand)
        }

        private static void Player_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self)
        {
            orig(self);
            SpiderSuperCrawl(self);//effecting movement speed
        }

        public static void SpiderSuperCrawl(Player self)// Changing movment speeds with crawling or walking
        {
            if (Plugin.CrawlSpeed.TryGet(self, out var power))
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
