using RWCustom;
using System;
using UnityEngine;
using Webs;

namespace Webs.WebsThings
{
    public class WebsSpit
    {

        public static void Apply()
        {
            On.Player.GrabUpdate += GrabUpdate;
        }

        private static void GrabUpdate(On.Player.orig_GrabUpdate orig, Player self, bool eu) // Player grab update check
        {
            //THIS METHOD IS RESPONSIBLE FOR THROWING HELD OBJECTS, SO WE WANT YOUR THING TO HAPPEN RIGHT BEFORE ALL THAT STUFF HAPPENS
            SlugcatSpit(self);
            orig(self, eu);
        }

        //Maximum Ammount of SpitAmmo the Player can have
        private static int maxSpiderSpitCapacity = 4;

        //Capacity the player currently have
        private static int playerA_SpiderSpitCapacity = 4;
        private static int playerB_SpiderSpitCapacity = 4;
        private static int playerC_SpiderSpitCapacity = 4;
        private static int playerD_SpiderSpitCapacity = 4;



        //Time until Spit Regenerates
        private static float resetCooldown = 5f;
        private static float playerA_spitCooldown = 5f;
        private static float playerB_spitCooldown = 5f;
        private static float playerC_spitCooldown = 5f;
        private static  float playerD_spitCooldown = 5f;

        //Checks if player can Spit or not
        private static bool playerA_canSpit;
        private static bool playerB_canSpit;
        private static bool playerC_canSpit;
        private static bool playerD_canSpit;


        public static void throwButtonCheckFalse(Player self)
        {
            if (self.playerState.playerNumber == 0) { playerA_canSpit = false; }
            if (self.playerState.playerNumber == 1) { playerB_canSpit = false; }
            if (self.playerState.playerNumber == 2) { playerC_canSpit = false; }
            if (self.playerState.playerNumber >= 3) { playerD_canSpit = false; }

        }

        public static void throwButtonPressed(Player self)
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


        public static void SpitRecharge(Player self)
        {
            if (playerA_SpiderSpitCapacity < maxSpiderSpitCapacity && self.playerState.playerNumber == 0)
            {
                playerA_spitCooldown -= Time.deltaTime;
                if (playerA_spitCooldown <= 0)
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

        public static void CanSpitCheck(Player self)
        {
            if (playerA_SpiderSpitCapacity > 0 && self.playerState.playerNumber == 0)
            {
                playerA_canSpit = true;
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

        public static void SpitProjectile(Player self)
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



        public static void SlugcatSpit(Player self)
        {
            if (Plugin.SpiderSpit.TryGet(self, out var spiderSpit))
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
    }
}
