using RoR2;
using System.Collections.Generic;
using UnityEngine;


namespace Moonstorm.Starstorm2.Survivors
{
    public static  class ExeIonChargeValuesLegacy
    {
        //note from groove - whoever thought it was a good idea to make this dictionary..i'm gonna find you

        internal static Dictionary<string, int> ionChargeValues = new Dictionary<string, int>
        {
            {
                "MoffeinAncientWispBody",
                10
            },
            {
                "BeetleGuardBody",
                2
            },
            {
                "BeetleQueen2Body",
                10
            },
            {
                "BellBody",
                2
            },
            {
                "BisonBody",
                2
            },
            {
                "BrotherBody",
                10
            },
            {
                "BrotherGlassBody",
                50
            },
            {
                "BrotherHurtBody",
                100
            },
            {
                "ClayBossBody",
                10
            },
            {
                "ClayBruiserBody",
                2
            },
            {
                "ElectricWormBody",
                20
            },
            {
                "GrandParentBody",
                15
            },
            {
                "GravekeeperBody",
                10
            },
            {
                "GreaterWispBody",
                2
            },
            {
                "ImpBossBody",
                10
            },
            {
                "LemurianBruiserBody",
                3
            },
            {
                "LunarGolemBody",
                3
            },
            {
                "LunarWispBody",
                3
            },
            {
                "MagmaWormBody",
                10
            },
            {
                "MiniMushroomBody",
                2
            },
            {
                "NullifierBody",
                2
            },
            {
                "ParentBody",
                3
            },
            {
                "RoboBallBossBody",
                10
            },
            {
                "ScavBody",
                15
            },
            {
                "ScavLunar1Body",
                100
            },
            {
                "ScavLunar2Body",
                100
            },
            {
                "ScavLunar3Body",
                100
            },
            {
                "ScavLunar4Body",
                100
            },
            {
                "ShopkeeperBody",
                300
            },
            {
                "SuperRoboBallBossBody",
                50
            },
            {
                "TitanBody",
                10
            },
            {
                "TitanGoldBody",
                30
            },
            {
                "VagrantBody",
                10
            },
            {
                "DireseekerBody",
                10
            },
            {
                "DireseekerBossBody",
                50
            }
        };

        public static int GetIonCountFromBody(CharacterBody body)
        {
            if (body == null) return 1;
            if (body.bodyIndex == BodyIndex.None) return 1;
            string name = BodyCatalog.GetBodyName(body.bodyIndex);
            if (name == "" || name == null) return 1;
            if (ionChargeValues.ContainsKey(name))
            {
                return ionChargeValues[name];
            }
            return 1;
        }

    }
}