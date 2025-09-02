using BepInEx.Logging;
using On;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TemplateMod
{
    internal class Hooks
    {
        public static void Apply()
        {
            int state = 1;
            bool active = false;
            bool holding = false;

            Vector2 pos1 = new Vector2(175f, 120f);
            Vector2 pos2 = new Vector2(500f, 120f);
            Vector2 pos3 = new Vector2(825f, 120f);

            List<AbstractConsumable> Ltrees = new List<AbstractConsumable>();
            List<AbstractConsumable> Rtrees = new List<AbstractConsumable>();

            static void moveTrees(List<AbstractConsumable> trees, float y, float x)
            {
                foreach (var tree in trees)
                {
                    if (tree.realizedObject != null)
                    {
                        tree.realizedObject.bodyChunks[0].vel = new Vector2(0f, 0f);
                        tree.realizedObject.bodyChunks[0].pos.y -= y;
                        tree.realizedObject.bodyChunks[0].pos.x = x;
                        //tree.realizedObject.bodyChunks[1].pos.y -= y;
                        //BepInEx.Logging.Logger.CreateLogSource("TemplateMod").LogInfo(tree.realizedObject.bodyChunks[0].pos);
                    }
                    if (tree.realizedObject.bodyChunks[0].pos.y < 200f)
                    {
                        tree.realizedObject.bodyChunks[0].pos.y = 830f;
                    }
                }
            }

            On.PlayerGraphics.InitiateSprites += (orig, self, sLeaser, rCam) =>
            {
                orig(self, sLeaser, rCam);
                Futile.atlasManager.LoadImage("atlases/mietek");
                Futile.atlasManager.LoadImage("atlases/aa");
                if (sLeaser.sprites[3].element.name != "atlases/mietek")
                {
                    sLeaser.sprites[3].SetElementByName("atlases/mietek");
                }
                //sLeaser.sprites[1]. = 16f;
                //BepInEx.Logging.Logger.CreateLogSource("TemplateMod").LogInfo(Futile.atlasManager.LogAllElementNames());
            };


            On.PlayerGraphics.DrawSprites += (orig, self, sLeaser, rCam, test2, test) =>
            {
                orig(self, sLeaser, rCam, test2, test);
                if (sLeaser.sprites[3].element.name != "atlases/mietek")
                {
                    sLeaser.sprites[3].SetElementByName("atlases/mietek");
                }
                if (sLeaser.sprites[2].element.name != "atlases/aa")
                {
                    sLeaser.sprites[2].SetElementByName("atlases/aa");
                }
                if (sLeaser.sprites[4].element.name != "atlases/aa")
                {
                    sLeaser.sprites[4].SetElementByName("atlases/aa");
                }
                if (sLeaser.sprites[5].element.name != "atlases/aa")
                {
                    sLeaser.sprites[5].SetElementByName("atlases/aa");
                }
                if (sLeaser.sprites[6].element.name != "atlases/aa")
                {
                    sLeaser.sprites[6].SetElementByName("atlases/aa");
                }
            };

            On.Rock.DrawSprites += (orig, self, sLeaser, rCam, test2, test) =>
            {
                orig(self, sLeaser, rCam, test2, test);
                if (sLeaser.sprites[0].element.name != "atlases/mietek")
                {
                    sLeaser.sprites[0].SetElementByName("atlases/mietek");
                }
                sLeaser.sprites[0].rotation = 0f;
                self.color = Color.white;
            };

            On.Player.Update += (orig, self, test) =>
            {
                //BepInEx.Logging.Logger.CreateLogSource("TemplateMod").LogInfo(self.input[0].x);
                //BepInEx.Logging.Logger.CreateLogSource("TemplateMod").LogInfo(active);
                //BepInEx.Logging.Logger.CreateLogSource("TemplateMod").LogInfo(state);
                if (self.input[0].thrw && !active)
                {
                    active = true;

                    float height = 1400;
                    float Lwidth = -136;
                    float Rwidth = 1120;

                    for (int i = 0; i < 10; i++)
                    {
                        var abs = new AbstractConsumable(
                            self.room.world,
                            AbstractPhysicalObject.AbstractObjectType.Rock,
                            null,
                            self.room.GetWorldCoordinate(self.bodyChunks[0].pos),
                            self.room.game.GetNewID(),
                            -1, // consumableTypeIndex (use -1 if not relevant)
                            0,  // abstractConsumableID (default 0)
                            null // AbstractPhysicalObject.AbstractObjectStick
                        );

                        abs.RealizeInRoom();
                        abs.realizedObject.bodyChunks[0].pos.x = Lwidth;
                        abs.realizedObject.bodyChunks[0].pos.y = height;
                        height += 50f;

                        Ltrees.Add(abs);
                    }

                    height = 830;

                    for (int i = 0; i < 10; i++)
                    {
                        var abs = new AbstractConsumable(
                            self.room.world,
                            AbstractPhysicalObject.AbstractObjectType.Rock,
                            null,
                            self.room.GetWorldCoordinate(self.bodyChunks[0].pos),
                            self.room.game.GetNewID(),
                            -1, // consumableTypeIndex (use -1 if not relevant)
                            0,  // abstractConsumableID (default 0)
                            null // AbstractPhysicalObject.AbstractObjectStick
                        );

                        abs.RealizeInRoom();
                        abs.realizedObject.bodyChunks[0].pos.x = Rwidth;
                        abs.realizedObject.bodyChunks[0].pos.y = height;
                        height += 50f;

                        Rtrees.Add(abs);
                    }

                    var scav = new AbstractCreature(self.room.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Scavenger), null, self.room.GetWorldCoordinate(self.bodyChunks[0].pos), self.room.game.GetNewID());
                    scav.RealizeInRoom();
                    self.room.AddObject(scav.realizedCreature);

                    //abs.RealizeInRoom();

                    //if (abs.realizedObject != null)
                    //{
                    //    self.room.AddObject(abs.realizedObject);
                    //}
                }

                //BepInEx.Logging.Logger.CreateLogSource("TemplateMod").LogInfo(self.bodyChunks[0].pos);

                if (active == true)
                {
                    self.bodyChunks[1].vel = new Vector2(0f, 0f);
                    self.bodyChunks[0].vel = new Vector2(0f, 0f);

                    if (self.input[0].x == 0)
                    {
                        holding = false;
                    }

                    if (self.input[0].x == -1 && state != 0 && !holding)
                    {
                        state -= 1;
                        holding = true;
                    }
                    if (self.input[0].x == 1 && state != 2 && !holding)
                    {
                        state += 1;
                        holding = true;
                    }

                    if (state == 0)
                    {
                        self.bodyChunks[0].pos = pos1;
                        self.bodyChunks[1].pos = pos1;
                    }
                    else if (state == 1)
                    {
                        self.bodyChunks[0].pos = pos2;
                        self.bodyChunks[1].pos = pos2;
                    }
                    else if (state == 2)
                    {
                        self.bodyChunks[0].pos = pos3;
                        self.bodyChunks[1].pos = pos3;
                    }

                    moveTrees(Ltrees, 5f, -136f);
                    moveTrees(Rtrees, 5f, 1120f);
                }
                orig(self, test);
            };
        }

        private static void Rock_DrawSprites(On.Rock.orig_DrawSprites orig, Rock self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            throw new NotImplementedException();
        }
    }
}

/*
Vector2 vector = new Vector2(self.bodyChunks[0].pos.x, self.bodyChunks[0].pos.y);
self.room.AddObject(new Explosion.ExplosionLight(vector, 280f, 1f, 7, self.mudColor));
self.room.AddObject(new Explosion.ExplosionLight(vector, 230f, 1f, 3, self.mudColor));
self.room.AddObject(new ExplosionSpikes(self.room, vector, 14, 30f, 9f, 7f, 170f, self.mudColor));
self.room.AddObject(new ShockWave(vector, 330f, 0.045f, 5, false));

BepInEx.Logging.Logger.CreateLogSource("TemplateMod").LogInfo("crazy?");
self.bodyChunks[0].pos = new Vector2(10f, 10f);
*/
//BepInEx.Logging.Logger.CreateLogSource("TemplateMod").LogInfo(self.bodyChunks[0].pos);