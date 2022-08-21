using RoR2;
using System.Collections.Generic;
using UnityEngine;


namespace Moonstorm.Starstorm2.Survivors
{
    public sealed class Executioner : SurvivorBase
    {
        public override GameObject BodyPrefab { get; } = SS2Assets.LoadAsset<GameObject>("ExecutionerBody");
        public override GameObject MasterPrefab { get; } = SS2Assets.LoadAsset<GameObject>("ExecutionerMonsterMaster");
        public override SurvivorDef SurvivorDef { get; } = SS2Assets.LoadAsset<SurvivorDef>("SurvivorExecutioner");

        

        public override void ModifyPrefab()
        {
            base.ModifyPrefab();

            var cb = BodyPrefab.GetComponent<CharacterBody>();
            cb.preferredPodPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod");
            cb._defaultCrosshairPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/StandardCrosshair");
            /*var footstepHandler = BodyPrefab.GetComponent<ModelLocator>().modelTransform.GetComponent<FootstepHandler>();
            footstepHandler.footstepDustPrefab = Resources.Load<GameObject>("Prefabs/GenericFootstepDust");*/
        }



    }
}