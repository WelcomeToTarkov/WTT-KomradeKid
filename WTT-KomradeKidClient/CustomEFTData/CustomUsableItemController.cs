#if !UNITY_EDITOR
using System;
using System.Collections.Generic;
using Comfort.Common;
using EFT;

namespace GameBoyEmulator.CustomEFTData;

public class CustomUsableItemController : Player.UsableItemController
{
    public override Dictionary<Type, OperationFactoryDelegate> GetOperationFactoryDelegates()
    {
        var factoryDelegates = new Dictionary<Type, OperationFactoryDelegate>
        {
            { typeof(OperationOne), CreateOperationOne },
            { typeof(OperationTwo), CreateOperationTwo },
            { typeof(OperationThree), CreateOperationThree },
            { typeof(OperationFour), CreateOperationFour }
        };
        return factoryDelegates;
    }

    public override void vmethod_0(Player player, WeaponPrefab weaponPrefab)
    {
        base.vmethod_0(player, weaponPrefab);

        player.ProceduralWeaponAnimation.ManualSetVariables(2f, 0f, 0f, 0f);

        method_6();

        gclass2086_0.AfterGetFromPoolInit(player.ProceduralWeaponAnimation, null, player.IsYourPlayer);

        BaseSoundPlayer soundPlayer = _controllerObject.GetComponent<BaseSoundPlayer>();
        if (soundPlayer != null)
        {
            soundPlayer.Init(this, player.PlayerBones.WeaponRoot, player);
        }

        // Pass 'this' directly so DefaultEmulatorManager doesn't have to look it up
        InitializeEmulator(this);
    }
    
    private void InitializeEmulator(CustomUsableItemController controller)
    {
        DefaultEmulatorManager manager = _controllerObject.GetComponentInChildren<DefaultEmulatorManager>();
        if (manager != null)
        {
            manager.Init(controller);
        }
    }
    
    public override void vmethod_2(EPlayerState previousState, EPlayerState nextState)
    {
    }

    public override void vmethod_1(Action callback)
    {
        InitiateOperation<OperationOne>().Start(callback);
    }

    private Player.BaseAnimationOperationClass CreateOperationOne()
    {
        return new OperationOne(this);
    }

    private Player.BaseAnimationOperationClass CreateOperationTwo()
    {
        return new OperationTwo(this);
    }

    private Player.BaseAnimationOperationClass CreateOperationThree()
    {
        return new OperationThree(this);
    }

    private Player.BaseAnimationOperationClass CreateOperationFour()
    {
        return new OperationFour(this);
    }

    private class OperationOne(CustomUsableItemController controller) : Class1305(controller)
    {
        public override void vmethod_0()
        {
            OperationTwo operation = UsableItemController_0.InitiateOperation<OperationTwo>();
            operation.Start();

            Action_1();

            if (Action_0 != null)
            {
                operation.HideWeapon(Action_0, Bool_0);
            }
        }

        public override void SetLeftStanceAnimOnStartOperation()
        {
            Player_0.MovementContext.LeftStanceController.DisableLeftStanceAnimFromHandsAction();
        }
    }

    private class OperationTwo(CustomUsableItemController controller) : Class1299(controller)
    {
        public override void vmethod_0(GInterface443 oneItemOperation, Callback callback)
        {
            UsableItemController_0.InitiateOperation<OperationFour>().Start(oneItemOperation.Item1, callback);
        }
        
        public override void HideWeapon(Action onHidden, bool fastDrop)
        {
            State = Player.EOperationState.Finished;
            UsableItemController_0.InitiateOperation<OperationThree>().Start(onHidden, fastDrop);
        }
    }

    private class OperationThree(CustomUsableItemController controller) : Class1302(controller);

    private class OperationFour(CustomUsableItemController controller) : Class1293(controller)
    {
        public override void vmethod_0()
        {
            UsableItemController_0.InitiateOperation<OperationTwo>().Start();
        }
    }
}

#endif