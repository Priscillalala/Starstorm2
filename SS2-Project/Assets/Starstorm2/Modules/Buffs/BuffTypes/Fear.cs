using EntityStates;
using EntityStates.AI.Walker;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Moonstorm.Components;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.CharacterAI;
using System;
using UnityEngine;

namespace Moonstorm.Starstorm2.Buffs
{
    public sealed class Fear : BuffBase
    {
        [TokenModifier("SS2_KEYWORD_FEAR", StatTypes.Percentage, 0)]
        public static float fearDamageBonus = 0.2f;
        [TokenModifier("SS2_KEYWORD_FEAR", StatTypes.Percentage, 1)]
        public static float fearMovespeedBonus = 0.4f;
        public override BuffDef BuffDef { get; } = SS2Assets.LoadAsset<BuffDef>("BuffFear");

        public static DamageColorIndex fearDamageColor;
        public override void Initialize()
        {
            base.Initialize();
            Hook();
            fearDamageColor = SS2ColorUtil.RegisterDamageColor(new Color(0.7122642f, 0.8666036f, 1f));
            //dont wanna load this every time fear is inflicted
            Behavior.tempVFXPrefab = SS2Assets.LoadAsset<GameObject>("ExecutionerFearEffect");

            //for screwing with the floating skull mat
            /*MaterialControllerComponents.HGCloudRemapController remapController = Behavior.tempVFXPrefab.AddComponent<MaterialControllerComponents.HGCloudRemapController>();
            remapController.material = SS2Assets.LoadAsset<Material>("matFearIndicator");*/
        }
        // Make this shit not a hook
        // Note: keep the hook until kevin comes back.
        // Hook wins!!
        // Fuck kevin, lol
        // yeah, true
        internal void Hook()
        {
            //feared enemies run away
            IL.EntityStates.AI.Walker.Combat.UpdateAI += Combat_UpdateAI;
            //feared enemies take more damage - doing this w/ interfaces is likely to cause double dipping through proc chains
            IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private void HealthComponent_TakeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            int damageLocIndex = -1;

            bool ILFound = c.TryGotoNext(MoveType.After,
                x => x.MatchLdarg(1),
                x => x.MatchLdfld<DamageInfo>(nameof(DamageInfo.rejected)),
                x => x.MatchBrfalse(out ILLabel label),
                x => x.MatchRet(),
                x => x.MatchLdarg(1),
                x => x.MatchLdfld<DamageInfo>(nameof(DamageInfo.damage)),
                x => x.MatchStloc(out damageLocIndex)
                ) && c.TryGotoNext(MoveType.After,
                x => x.MatchLdloc(damageLocIndex),
                x => x.MatchLdcR4(0),
                x => x.MatchBleUn(out ILLabel label)
                );

            if (ILFound)
            {
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldarg_1);
                c.Emit(OpCodes.Ldloc, damageLocIndex);
                c.EmitDelegate<Func<HealthComponent, DamageInfo, float, float>>((healthComponent, damageInfo, damage) =>
                {
                    if (healthComponent.body && healthComponent.body.HasBuff(SS2Content.Buffs.BuffFear))
                    {
                        damageInfo.damageColorIndex = fearDamageColor;
                        return damage * (1f + fearDamageBonus);
                    }
                    return damage;
                });
                c.Emit(OpCodes.Stloc, damageLocIndex);
            }
            else { SS2Log.Error(this + ": Take Damage IL hook failed!"); }
        }

        private void Combat_UpdateAI(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            int movementTypeLocIndex = -1;

            bool ILFound = c.TryGotoNext(MoveType.After,
                x => x.MatchLdfld<Combat>(nameof(Combat.dominantSkillDriver)),
                x => x.MatchLdfld<AISkillDriver>(nameof(AISkillDriver.movementType)),
                x => x.MatchStloc(out movementTypeLocIndex)
                );

            if (ILFound)
            {
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldloc, movementTypeLocIndex);
                c.EmitDelegate<Func<Combat, AISkillDriver.MovementType, AISkillDriver.MovementType>>((combat, movementType) =>
                {
                    if(combat.body && combat.body.HasBuff(SS2Content.Buffs.BuffFear))
                    {
                        return AISkillDriver.MovementType.FleeMoveTarget;
                    }
                    return movementType;
                });
                c.Emit(OpCodes.Stloc, movementTypeLocIndex);
            }
            else { SS2Log.Error(this + ": Update AI IL hook failed!"); }
        }

        public sealed class Behavior : BaseBuffBodyBehavior, IBodyStatArgModifier
        {
            public static GameObject tempVFXPrefab;
            private TemporaryVisualEffect tempEffectInstance;
            public const string childLocatorOverride = "Head";
            [BuffDefAssociation]
            private static BuffDef GetBuffDef() => SS2Content.Buffs.BuffFear;
            private void Start()
            {
                /*if (body.healthComponent)
                {
                    HG.ArrayUtils.ArrayAppend(ref body.healthComponent.onIncomingDamageReceivers, this);
                }*/
                GameObject gameObject = Instantiate(tempVFXPrefab, body.corePosition, Quaternion.identity);
                tempEffectInstance = gameObject.GetComponent<TemporaryVisualEffect>();
                tempEffectInstance.parentTransform = body.coreTransform;
                tempEffectInstance.visualState = TemporaryVisualEffect.VisualState.Enter;
                tempEffectInstance.healthComponent = body.healthComponent;
                tempEffectInstance.radius = body.radius;
                Transform mdlTransform = body.modelLocator ? body.modelLocator.modelTransform : null;
                if (mdlTransform)
                {
                    ChildLocator childLocator = mdlTransform.GetComponent<ChildLocator>();
                    Transform headtransform = childLocator ? childLocator.FindChild(childLocatorOverride) : null;
                    if (headtransform)
                    {
                        tempEffectInstance.parentTransform = headtransform;
                    }
                }
                //ordering is wrong and sometimes this behaviour isnt included in the normal recalc stats from a buff being applied
                body.MarkAllStatsDirty();
            }

            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                args.moveSpeedMultAdd += fearMovespeedBonus;
            }

            private void OnDestroy()
            {
                //ordering is wrong and sometimes this behaviour isnt included in the normal recalc stats from a buff being lost
                body.MarkAllStatsDirty();
                if (tempEffectInstance)
                {
                    tempEffectInstance.visualState = TemporaryVisualEffect.VisualState.Exit;
                }
                //This SHOULDNT cause any errors because nothing should be fucking with the order of things in this list... I hope.
                /*if (body.healthComponent)
                {
                    int i = Array.IndexOf(body.healthComponent.onIncomingDamageReceivers, this);
                    if (i > -1)
                        HG.ArrayUtils.ArrayRemoveAtAndResize(ref body.healthComponent.onIncomingDamageReceivers, body.healthComponent.onIncomingDamageReceivers.Length, i);
                }*/
            }
        }
    }
}
/* (il) =>
            {
                
                //go to where movement type is checked (applying movement vector)
                curs.GotoNext(x => x.MatchCall<Vector3>("Cross"));
                curs.Index += 2;
                curs.Emit(OpCodes.Ldarg_0);
                curs.Emit(OpCodes.Ldfld, typeof(EntityState).GetFieldCached("outer"));
                curs.Emit(OpCodes.Ldloc_1);
                curs.EmitDelegate<Func<EntityStateMachine, AISkillDriver.MovementType, AISkillDriver.MovementType>>((ESM, MoveType) =>
                {
                    if (ESM.GetComponent<CharacterMaster>().GetBody().HasBuff(SS2Content.Buffs.BuffFear))
                    {
                        return AISkillDriver.MovementType.FleeMoveTarget;
                    }
                    else
                        return MoveType;
                });
                curs.Emit(OpCodes.Stloc_1);
            };*/

/*IL.EntityStates.AI.Walker.Combat.GenerateBodyInputs += (il) =>
{
    ILCursor curs = new ILCursor(il);
    curs.GotoNext(x => x.MatchLdfld<Combat>("currentSkillMeetsActivationConditions"));
    curs.Index += 1;
    curs.Emit(OpCodes.Ldarg_0);
    curs.Emit(OpCodes.Ldfld, typeof(EntityState).GetFieldCached("outer"));
    curs.EmitDelegate<Func<bool, EntityStateMachine, bool>>((cond, ESM) =>
    {
        CharacterMaster characterMaster = ESM.GetComponent<CharacterMaster>();
        if (characterMaster && characterMaster.hasBody && characterMaster.GetBody().HasBuff(SS2Content.Buffs.BuffFear))
        {
            return false;
        }
        return cond;
    });
};*/
