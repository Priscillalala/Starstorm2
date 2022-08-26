using R2API;
using RoR2;
using UnityEngine;
using static R2API.DamageAPI;

namespace Moonstorm.Starstorm2.DamageTypes
{
    //tracks if damage is coming from exe's axe slam
    public sealed class ExecutionerSlamDamageType : DamageTypeBase
    {
        public override ModdedDamageType ModdedDamageType { get; protected set; }

        public static ModdedDamageType damageType;

        public override void Initialize()
        {
            damageType = ModdedDamageType;
        }
        public override void Delegates()
        {
            
        }
    }
}
