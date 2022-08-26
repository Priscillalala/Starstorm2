using Moonstorm.Starstorm2.Orbs;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using Moonstorm.Starstorm2.DamageTypes;

namespace Moonstorm.Starstorm2.Components
{
    [RequireComponent(typeof(SkillLocator))]
    public class ExecutionerController : NetworkBehaviour, IOnKilledOtherServerReceiver
    {
        [TokenModifier("SS2_KEYWORD_ENERGIZING", StatTypes.Default, 0)]
        public static int cdrPerSlamKill = 1;
        private CharacterBody characterBody;
        private GenericSkill secondary;

        private void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
            secondary = characterBody.skillLocator.secondary;
        }
        private void Start()
        {
            if (secondary)
            {
                secondary.RemoveAllStocks();
            }
        }

        /*public void OnDamageDealtServer(DamageReport report)
        {
            //This will break is anyone renames that skilldef's identifier
            if (report.victim.gameObject != report.attacker && !report.victimBody.bodyFlags.HasFlag(CharacterBody.BodyFlags.Masterless) && (secondary.skillDef.skillName == "ExecutionerFireIonGun" || secondary.skillDef.skillName == "ExecutionerFireIonSummon"))
            {
                var killComponents = report.victimBody.GetComponents<ExecutionerKillComponent>();
                foreach (var killCpt in killComponents)
                {
                    //If a kill component for this executioner already exists, reset the timer.
                    if (killCpt.attacker == gameObject)
                    {
                        killCpt.timeAlive = 0f;
                        return;
                    }
                }
                //Else add a kill component
                var killComponent = report.victim.gameObject.AddComponent<ExecutionerKillComponent>();
                killComponent.attacker = gameObject;
            }
        }*/
        public bool CanRestockSecondary()
        {
            //secondary exists, isnt something like hooks of heresy, and needs more stock
            return secondary && secondary.skillDef == secondary.defaultSkillDef && secondary.stock < secondary.maxStock;
        }
        [ClientRpc]
        public void RpcRestockIonFull()
        {
            if (CanRestockSecondary())
            {
                secondary.stock = secondary.maxStock;
                Util.PlaySound("ExecutionerMaxCharge", base.gameObject);
            }
        }
        [ClientRpc]
        public void RpcAddIonCharge()
        {
            if (CanRestockSecondary())
            {
                //AddOneStock resets the current cd timer
                secondary.stock++;
                Util.PlaySound("ExecutionerGainCharge", base.gameObject);
            }
        }

        public void OnKilledOtherServer(DamageReport damageReport)
        {
            if (!damageReport.victimBody)
            {
                return;
            }
            if (CanRestockSecondary() && (damageReport.victimBody.bodyFlags & CharacterBody.BodyFlags.Masterless) == CharacterBody.BodyFlags.None)
            {
                ExecutionerIonOrb ionOrb = new ExecutionerIonOrb();
                ionOrb.origin = damageReport.victimBody.corePosition;
                ionOrb.target = characterBody.mainHurtBox;
                ionOrb.fullRestock = damageReport.victimBody.isChampion;
                OrbManager.instance.AddOrb(ionOrb);
            }
            if (damageReport.damageInfo.HasModdedDamageType(ExecutionerSlamDamageType.damageType))
            {
                characterBody.skillLocator.DeductCooldownFromAllSkillsServer(cdrPerSlamKill);
            }
        }
    }
}