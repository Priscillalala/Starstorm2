using RoR2;
using System.Collections.Generic;
using UnityEngine;


namespace Moonstorm.Starstorm2.Survivors
{
    [DisabledContent]
    public sealed class NemExe : SurvivorBase
    {
        public override GameObject BodyPrefab { get; } = SS2Assets.LoadAsset<GameObject>("NemExeBody");
        public override GameObject MasterPrefab { get; } = SS2Assets.LoadAsset<GameObject>("ExecutionerMonsterMaster");
        public override SurvivorDef SurvivorDef { get; } = SS2Assets.LoadAsset<SurvivorDef>("survivorNemExe");

        

        /*public override void ModifyPrefab()
        {
            base.ModifyPrefab();

            var cb = BodyPrefab.GetComponent<CharacterBody>();
            cb.preferredPodPrefab = Resources.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod");
            cb._defaultCrosshairPrefab = Resources.Load<GameObject>("Prefabs/Crosshair/StandardCrosshair");
            var footstepHandler = BodyPrefab.GetComponent<ModelLocator>().modelTransform.GetComponent<FootstepHandler>();
            footstepHandler.footstepDustPrefab = Resources.Load<GameObject>("Prefabs/GenericFootstepDust");
        }*/

        

    }
}