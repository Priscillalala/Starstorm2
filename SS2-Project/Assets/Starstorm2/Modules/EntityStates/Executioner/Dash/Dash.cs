using Moonstorm;
using Moonstorm.Starstorm2;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EntityStates.Executioner
{
    public class Dash : BaseSkillState
    {
        public static float baseDuration = 0.9f;
        public static float speedMultiplier = 3.0f;
        public static float debuffRadius = 20f;
        [TokenModifier("SS2_EXECUTIONER_DASH_DESCRIPTION", StatTypes.Default, 0)]
        public static float debuffDuration = 4.0f;
        public static GameObject dashEffect;

        //I ain't afraid of no executioner
        //this is kind of a goofy solution, but bosses have different body names in different phases
        private static string[] immuneToFearNameTokens = new string[]{ "VOIDRAIDCRAB_BODY_NAME", "BROTHER_BODY_NAME", "SUPERROBOBALLBOSS_BODY_NAME", "DIRESEEKER_BOSS_BODY_NAME", "TITANGOLD_BODY_NAME" };

        private float duration;
        private SphereSearch fearSearch;
        private List<HurtBox> hits;
        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();

            duration = baseDuration;
            Util.PlayAttackSpeedSound("ExecutionerUtility", gameObject, 1.0f);
            PlayAnimation("FullBody, Override", "Utility", "Utility.playbackRate", duration);

            //create dash aoe
            /*if (isAuthority)
            {
                Vector3 orig = characterBody.corePosition;
                BlastAttack blast = new BlastAttack()
                {
                    baseDamage = 0,
                    damageType = DamageType.Stun1s,
                    radius = debuffRadius,
                    falloffModel = BlastAttack.FalloffModel.None,
                    baseForce = 600f,
                    teamIndex = TeamComponent.GetObjectTeam(gameObject),
                    attacker = gameObject,
                    inflictor = gameObject,
                    position = orig,
                    attackerFiltering = AttackerFiltering.NeverHitSelf,
                    procCoefficient = 0f
                };
                blast.Fire();
            }*/

            if (dashEffect)
                EffectManager.SimpleMuzzleFlash(dashEffect, gameObject, "DashEffect", true);

            hits = new List<HurtBox>();
            fearSearch = new SphereSearch();
            fearSearch.mask = LayerIndex.entityPrecise.mask;
            fearSearch.radius = debuffRadius;

            Transform modelTransform = GetModelTransform();
            if (modelTransform)
            {
                TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 1.5f * baseDuration;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = Resources.Load<Material>("Materials/matHuntressFlashBright");
                temporaryOverlay.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
            }
        }

        private void CreateFearAoe()
        {
            hits.Clear();
            fearSearch.ClearCandidates();
            fearSearch.origin = characterBody.corePosition;
            fearSearch.RefreshCandidates();
            fearSearch.FilterCandidatesByDistinctHurtBoxEntities();
            fearSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetUnprotectedTeams(teamComponent.teamIndex));
            fearSearch.GetHurtBoxes(hits);
            foreach (HurtBox h in hits)
            {
                HealthComponent hp = h.healthComponent;
                CharacterBody body = hp?.body;
                if (body && !body.HasBuff(SS2Content.Buffs.BuffFear) && body != characterBody && (body.bodyFlags & CharacterBody.BodyFlags.Masterless) == CharacterBody.BodyFlags.None && Array.IndexOf(immuneToFearNameTokens, body.baseNameToken) == -1)
                {
                    /*var fear = body.AddItemBehavior<Fear.Behavior>(1);
                    fear.inflictor = characterBody;*/
                    body.AddTimedBuff(SS2Content.Buffs.BuffFear, debuffDuration);
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (characterDirection && characterMotor)
                characterMotor.rootMotion += characterDirection.forward * characterBody.moveSpeed * speedMultiplier * Time.fixedDeltaTime;

            CreateFearAoe();

            if (fixedAge >= duration)
                outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}