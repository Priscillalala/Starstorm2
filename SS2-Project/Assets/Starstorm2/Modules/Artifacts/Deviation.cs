﻿using EntityStates;
using R2API.ScriptableObjects;
using RoR2;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UObject = UnityEngine.Object;

namespace Moonstorm.Starstorm2.Artifacts
{
    [DisabledContent]

    public sealed class Deviation : ArtifactBase
    {
        public override ArtifactDef ArtifactDef { get; } = SS2Assets.LoadAsset<ArtifactDef>("Deviation", SS2Bundle.Artifacts);
        public override ArtifactCode ArtifactCode { get; } = SS2Assets.LoadAsset<ArtifactCode>("DeviationCode", SS2Bundle.Artifacts);

        public override void Initialize()
        {

        }

        private void DeviationOverride(On.RoR2.TeleporterInteraction.IdleState.orig_OnInteractionBegin orig, BaseState self, Interactor activator)
        {
            SS2Log.Info(self.outer.gameObject.name);
            var tc = self.outer.gameObject.GetComponent<TestComponent>();
            if (!tc)
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = "Yummy eating input"
                });
                self.outer.gameObject.AddComponent<TestComponent>();
            }
            else
            {
                orig(self, activator);
            }
        }


        private void AddCode(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == "lobby")
            {
                var code = UObject.Instantiate(SS2Assets.LoadAsset<GameObject>("CognationCode", SS2Bundle.Artifacts), Vector3.zero, Quaternion.identity);
                code.transform.position = new Vector3(4, 0, 8);
                code.transform.rotation = Quaternion.Euler(10, 90, 0);
                code.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            }
        }

        public override void OnArtifactEnabled()
        {
            On.RoR2.TeleporterInteraction.IdleState.OnInteractionBegin += DeviationOverride;
        }

        public override void OnArtifactDisabled()
        {
            On.RoR2.TeleporterInteraction.IdleState.OnInteractionBegin -= DeviationOverride;

        }

        //private string AddCognateName2(On.RoR2.Util.orig_GetBestBodyName orig, GameObject bodyObject)
        //{
        //    var result = orig(bodyObject);
        //    //SS2Log.Info("ahhhh");
        //    CharacterBody characterBody = bodyObject?.GetComponent<CharacterBody>();
        //    if (characterBody && characterBody.inventory && characterBody.inventory.GetItemCount(SS2Content.Items.Cognation) > 0)
        //    {
        //        result = Language.GetStringFormatted("SS2_ARTIFACT_COGNATION_PREFIX", result);
        //    }
        //    return result;
        //}

        //private void SpawnCognationGhost(DamageReport report)
        //{
        //    if (!hasMadeList)
        //    {
        //        InitializeIllegalGhostList(); //this is so fucking jank
        //        hasMadeList = true;
        //    }
        //    CharacterMaster victimMaster = report.victimMaster;
        //
        //    if (victimMaster)
        //    {
        //        if (CanBecomeGhost(victimMaster))
        //        {
        //            var summon = PrepareMasterSummon(victimMaster);
        //            if (summon != null)
        //            {
        //                SummonGhost(summon.Perform(), victimMaster);
        //            }
        //        }
        //    }
        //}

        //private string AddCognateName(On.RoR2.Util.orig_GetBestBodyName orig, GameObject bodyObject)
        //{
        //    var body = bodyObject.GetComponent<CharacterBody>();
        //    if (body)
        //    { 
        //        if (body.inventory)
        //        {
        //            if (body.inventory.GetItemCount(SS2Content.Items.Cognation) > 0)
        //            {
        //                return "Cognate " + orig(bodyObject);
        //            }
        //        }
        //    }
        //
        //    return orig(bodyObject);
        //}
        //private bool CanBecomeGhost(CharacterMaster victimMaster)
        //{
        //    bool isMonster = victimMaster.teamIndex != TeamIndex.Player;
        //    bool hasBody = victimMaster.hasBody;
        //    bool notGhost = victimMaster.inventory.GetItemCount(SS2Content.Items.Cognation) == 0;
        //    //SS2Log.Info("victim master: " + victimMaster.masterIndex);
        //    //bool notBlacklisted = !BlacklistedMasterIndices.Contains(victimMaster.masterIndex);
        //    bool notBlacklisted = !illegalGhosts.Contains(victimMaster.backupBodyIndex);
        //    //if(victimMaster.masterIndex == ) { 
        //    //}
        //    //bool notBlacklisted
        //    bool notVoidTouched = victimMaster.inventory.currentEquipmentIndex != DLC1Content.Equipment.EliteVoidEquipment.equipmentIndex;
        //    
        //    return isMonster && hasBody && notGhost && notBlacklisted && notVoidTouched;
        //}
        //
        //private MasterSummon PrepareMasterSummon(CharacterMaster master)
        //{
        //    var body = master.GetBody();
        //
        //    var ghostSummon = new MasterSummon();
        //    ghostSummon.ignoreTeamMemberLimit = true;
        //    ghostSummon.masterPrefab = MasterCatalog.GetMasterPrefab(master.masterIndex);
        //    ghostSummon.position = body.corePosition;
        //    ghostSummon.rotation = Quaternion.LookRotation(body.inputBank.GetAimRay().direction);
        //    ghostSummon.teamIndexOverride = master.teamIndex;
        //    ghostSummon.summonerBodyObject = null;
        //    ghostSummon.inventoryToCopy = InheritInventory ? master.inventory : null;
        //
        //    return ghostSummon;
        //}
        //
        //private void SummonGhost(CharacterMaster ghostMaster, CharacterMaster originalMaster)
        //{
        //    var originalBody = originalMaster.GetBody();
        //    var ghostBody = ghostMaster.GetBody();
        //
        //    if (NetworkServer.active)
        //    {
        //        ghostBody.AddTimedBuff(RoR2Content.Buffs.SmallArmorBoost, 1);
        //        DeathRewards ghostRewards = ghostBody.GetComponent<DeathRewards>();
        //        DeathRewards origRewards = originalBody.GetComponent<DeathRewards>();
        //        if (ghostRewards && origRewards)
        //        {
        //            ghostRewards.goldReward = (uint)(origRewards.goldReward / 1.25f);
        //            ghostRewards.expReward = (uint)(origRewards.expReward / 1.25);
        //        }
        //
        //        if (ghostBody)
        //        {
        //            foreach (EntityStateMachine esm in ghostBody.GetComponents<EntityStateMachine>())
        //            {
        //                esm.initialStateType = esm.mainStateType;
        //            }
        //        }
        //
        //        ghostMaster.inventory.SetEquipmentIndex(originalMaster.inventory.currentEquipmentIndex);
        //        ghostMaster.inventory.GiveItem(SS2Content.Items.Cognation);
        //    }
        //
        //    var timer = ghostMaster.gameObject.AddComponent<MasterSuicideOnTimer>();
        //    timer.lifeTimer = (originalBody.isChampion || originalBody.isBoss) ? 30 * 2 : 30;
        //}
    }

    class DeviationState : TeleporterInteraction.BaseTeleporterState
    {
        public override TeleporterInteraction.ActivationState backwardsCompatibleActivationState
        {
            get
            {
                return TeleporterInteraction.ActivationState.Charged;
            }
        }

        private CombatDirector bonusDirector
        {
            get
            {
                return base.teleporterInteraction.bonusDirector;
            }
        }

        // Token: 0x170004A0 RID: 1184
        // (get) Token: 0x0600322C RID: 12844 RVA: 0x000D4356 File Offset: 0x000D2556
        private CombatDirector bossDirector
        {
            get
            {
                return base.teleporterInteraction.bossDirector;
            }
        }

        // Token: 0x170004A1 RID: 1185
        // (get) Token: 0x0600322D RID: 12845 RVA: 0x0000B4BB File Offset: 0x000096BB
        //protected override bool shouldEnableChargingSphere
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}

    }


    public class TestComponent : MonoBehaviour
    {
        //i swear this makes sense
    }


}