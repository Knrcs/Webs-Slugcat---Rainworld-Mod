using System;
using System.Collections.Generic;
using System.Linq;
using RWCustom;
using Webs.WebsThings;
using UnityEngine;

namespace Webs.WebsThings
{
    public class WebsJump
    {

        public static void Apply()
        {
            On.Player.Jump += Player_Jump;
        }



        private static void Player_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);

            if (Plugin.SuperJump.TryGet(self, out var power))
            {
                self.jumpBoost *= 1f + power;
            }
        }
    }
}
