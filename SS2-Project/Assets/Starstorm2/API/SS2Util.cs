﻿using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HG;
using UnityEngine.Networking;

namespace Moonstorm.Starstorm2
{
    public static class SS2Util
    {
        #region Misc
        
        public static void DropShipCall(Transform origin, int tierWeight, uint teamLevel = 1, int amount = 1, ItemTier forcetier = 0, string theWorstCodeOfTheYear = null)
        {
            List<PickupIndex> dropList;
            float rarityscale = tierWeight * (float)(Math.Sqrt(teamLevel * 13) - 4); //I have absolutely no fucking idea what this is
            if (forcetier == ItemTier.Boss)
                dropList = Run.instance.availableBossDropList;
            else if (forcetier == ItemTier.Lunar)
                dropList = Run.instance.availableLunarCombinedDropList;
            else if (Util.CheckRoll(0.5f * rarityscale - 1) || teamLevel >= Items.NkotasHeritage.greenRemovalLevel || forcetier == ItemTier.Tier3)
                dropList = Run.instance.availableTier3DropList;
            else if (Util.CheckRoll(4 * rarityscale) || teamLevel >= Items.NkotasHeritage.whiteRemovalLevel || forcetier == ItemTier.Tier2)
                dropList = Run.instance.availableTier2DropList;
            else
                dropList = Run.instance.availableTier1DropList;
            int item = Run.instance.treasureRng.RangeInt(0, dropList.Count);

            if (amount > 1)
            {
                float angle = 360f / (float)amount;
                Vector3 vector = Quaternion.AngleAxis((float)UnityEngine.Random.Range(0, 360), Vector3.up) * (Vector3.up * 15f + Vector3.forward * (4.75f + (.25f * amount)));

                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
                for (int i = 0; i < amount; i++)
                {
                    //if (theWorstCodeOfTheYear != null)
                    //    CreateVFXDroplet(dropList[item], origin.position, vector, theWorstCodeOfTheYear);
                    //    
                    //else
                    //    PickupDropletController.CreatePickupDroplet(dropList[item], origin.position, vector);
                    PickupDropletController.CreatePickupDroplet(dropList[item], origin.position, vector);
                    vector = rotation * vector;
                }
                return;
            }

            if (theWorstCodeOfTheYear != null)
            {
                PickupDropletController.CreatePickupDroplet(dropList[item], origin.position, new Vector3(0, 15, 0));
                //CreateVFXDroplet(dropList[item], origin.position, new Vector3(0, 15, 0), theWorstCodeOfTheYear);
                return;
            }
            PickupDropletController.CreatePickupDroplet(dropList[item], origin.position, new Vector3(0, 15, 0));
        }

        public static ItemDef NkotasRiggedItemDrop(int tierWeight, uint teamLevel = 1, int forcetier = 0)
        {
            List<PickupIndex> dropList;
            float rarityscale = tierWeight * (float)(Math.Sqrt(teamLevel * 13) - 4); //I have absolutely no fucking idea what this is // me neither
            if (Util.CheckRoll(0.5f * rarityscale - 1) || teamLevel >= Items.NkotasHeritage.greenRemovalLevel || (forcetier == 3 && forcetier != 0))
                dropList = Run.instance.availableTier3DropList;
            else if (Util.CheckRoll(4 * rarityscale) || teamLevel >= Items.NkotasHeritage.whiteRemovalLevel || (forcetier == 2 && forcetier != 0))
                dropList = Run.instance.availableTier2DropList;
            else
                dropList = Run.instance.availableTier1DropList;
            int item = Run.instance.treasureRng.RangeInt(0, dropList.Count);
            return ItemCatalog.GetItemDef(PickupCatalog.GetPickupDef(dropList[item]).itemIndex); 
        }

        public static void CreateVFXDroplet(PickupIndex pickupIndex, Vector3 position, Vector3 velocity, string vfxPrefab)
        {
            //GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(PickupDropletController.pickupDropletPrefab, position, Quaternion.identity);
            //gameObject.GetComponent<PickupDropletController>().NetworkpickupIndex = pickupIndex;
            //Rigidbody component = gameObject.GetComponent<Rigidbody>();
            //component.velocity = velocity;
            //component.AddTorque(UnityEngine.Random.Range(150f, 120f) * UnityEngine.Random.onUnitSphere);
            //NetworkServer.Spawn(gameObject);

            var pickup = new GenericPickupController.CreatePickupInfo();
            
            pickup.prefabOverride = PickupCatalog.GetPickupDef(pickupIndex).dropletDisplayPrefab;
            //pickup.prefabOverride.AddComponent<NetworkIdentity>();
            
            //pickup.prefabOverride.AddComponent<ParticleSystem>();
            //var particleSys = pickup.prefabOverride.GetComponent<ParticleSystem>();
            //particleSys.

            pickup.pickupIndex = pickupIndex;
            PickupDropletController.CreatePickupDroplet(pickup, position, velocity);
            
            //PickupDropletController.CreatePickupDroplet(pickupIndex, position, velocity);
            EffectManager.SpawnEffect(SS2Assets.LoadAsset<GameObject>(vfxPrefab), new EffectData
            {
                rootObject = pickup.prefabOverride,
                origin = position,
                scale = 1f,
            }, true);
        }



        public static IEnumerator BroadcastChat(string token)
        {
            yield return new WaitForSeconds(1);
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = token });
            yield break;
        }

        internal static string ScepterDescription(string desc)
        {
            return "\n<color=#d299ff>SCEPTER: " + desc + "</color>";
        }

        #endregion Misc
    }

    internal static class ArrayHelper
    {
        public static T[] Append<T>(ref T[] array, List<T> list)
        {
            var orig = array.Length;
            var added = list.Count;
            Array.Resize(ref array, orig + added);
            list.CopyTo(array, orig);
            return array;
        }

        public static Func<T[], T[]> AppendDel<T>(List<T> list) => (r) => Append(ref r, list);
    }
    //Basically ColorAPI from r2api but it actually exists
    public static class SS2ColorUtil
    {
        private static List<ColorCatalog.ColorIndex> customColorIndices = new List<ColorCatalog.ColorIndex>();
        private static List<DamageColorIndex> customDamageColorIndices = new List<DamageColorIndex>();
        [SystemInitializer]
        public static void ModInit()
        {
            On.RoR2.ColorCatalog.GetColor += ColorCatalog_GetColor;
            On.RoR2.ColorCatalog.GetColorHexString += ColorCatalog_GetColorHexString;
            On.RoR2.DamageColor.FindColor += DamageColor_FindColor;
        }

        private static Color32 ColorCatalog_GetColor(On.RoR2.ColorCatalog.orig_GetColor orig, ColorCatalog.ColorIndex colorIndex)
        {
            if (customColorIndices.Contains(colorIndex))
            {
                return ColorCatalog.indexToColor32[(int)colorIndex];
            }
            return orig(colorIndex);
        }
        private static string ColorCatalog_GetColorHexString(On.RoR2.ColorCatalog.orig_GetColorHexString orig, ColorCatalog.ColorIndex colorIndex)
        {
            if (customColorIndices.Contains(colorIndex))
            {
                return ColorCatalog.indexToHexString[(int)colorIndex];
            }
            return orig(colorIndex);
        }
        private static Color DamageColor_FindColor(On.RoR2.DamageColor.orig_FindColor orig, DamageColorIndex colorIndex)
        {
            if (customDamageColorIndices.Contains(colorIndex))
            {
                return DamageColor.colors[(int)colorIndex];
            }
            return orig(colorIndex);
        }

        public static ColorCatalog.ColorIndex RegisterColor(Color32 color)
        {
            ColorCatalog.ColorIndex index = (ColorCatalog.ColorIndex)ColorCatalog.indexToColor32.Length;
            ArrayUtils.ArrayAppend(ref ColorCatalog.indexToColor32, color);
            ArrayUtils.ArrayAppend(ref ColorCatalog.indexToHexString, Util.RGBToHex(color));
            customColorIndices.Add(index);
            return index;
        }
        public static DamageColorIndex RegisterDamageColor(Color color)
        {
            ArrayUtils.ArrayAppend(ref DamageColor.colors, color);
            DamageColorIndex damageColorIndex = (DamageColorIndex)DamageColor.colors.Length - 1;
            customDamageColorIndices.Add(damageColorIndex);
            return damageColorIndex;
        }
    }
}