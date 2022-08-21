using Moonstorm.Starstorm2.Components;
using RoR2;
using RoR2.Orbs;
using UnityEngine;

namespace Moonstorm.Starstorm2.Orbs
{
    public class ExecutionerIonOrb : Orb
    {
        public ExecutionerController execController;

        //private NetworkSoundEventDef sound = SS2Assets.LoadAsset<NetworkSoundEventDef>("SoundEventExecutionerGainCharge");
        public bool fullRestock;
        private const float speed = 50f;

        public override void Begin()
        {
            duration = distanceToTarget / speed;
            EffectData effectData = new EffectData
            {
                origin = origin,
                genericFloat = duration,
                scale = 1f
            };
            effectData.SetHurtBoxReference(target);
            string path = fullRestock ? "ExecutionerIonSuperOrbEffect" : "ExecutionerIonOrbEffect";
            EffectManager.SpawnEffect(SS2Assets.LoadAsset<GameObject>(path), effectData, true);
            HurtBox hurtBox = target.GetComponent<HurtBox>();
            if (hurtBox)
                execController = hurtBox.healthComponent.GetComponent<ExecutionerController>();
        }

        public override void OnArrival()
        {
            if (execController)
            {
                if (fullRestock)
                {
                    execController.RpcRestockIonFull();
                }
                else
                {
                    execController.RpcAddIonCharge();
                }
                //if (sound)
                    //EffectManager.SimpleSoundEffect(sound.index, execController.transform.position, true);
            }
        }
    }
}