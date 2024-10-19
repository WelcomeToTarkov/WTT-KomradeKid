#if !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Comfort.Common;
using EFT;
using UnityEngine;
using AnimationSystem;
using GPUInstancer;
using EFT.UI;


public class CustomUsableItemController : Player.UsableItemController
{

    public override Dictionary<Type, OperationFactoryDelegate> GetOperationFactoryDelegates()
    {
        var factoryDelegates = new Dictionary<Type, OperationFactoryDelegate>
        {
            { typeof(OperationOne), new OperationFactoryDelegate(CreateOperationOne) },
            { typeof(OperationTwo), new OperationFactoryDelegate(CreateOperationTwo) },
            { typeof(OperationThree), new OperationFactoryDelegate(CreateOperationThree) },
            { typeof(OperationFour), new OperationFactoryDelegate(CreateOperationFour) }
        };
        return factoryDelegates;
    }

    public override void vmethod_0(Player player, WeaponPrefab weaponPrefab)
    {
        base.vmethod_0(player, weaponPrefab);

        player.ProceduralWeaponAnimation.ManualSetVariables(2f, 0f, 0f, 0f);

        method_5();

        gclass1679_0.AfterGetFromPoolInit(player.ProceduralWeaponAnimation, null, player.IsYourPlayer);

        BaseSoundPlayer soundPlayer = _controllerObject.GetComponent<BaseSoundPlayer>();
        if (soundPlayer != null)
        {
            soundPlayer.Init(this, player.PlayerBones.WeaponRoot, player);
        }

    }

    public override void vmethod_2(EPlayerState previousState, EPlayerState nextState)
    {
    }

    public override void vmethod_1(Action callback)
    {
        InitiateOperation<OperationOne>().Start(callback);
    }


    public Player.GClass1594 CreateOperationOne()
    {
        return new OperationOne(this);
    }

    public Player.GClass1594 CreateOperationTwo()
    {
        return new OperationTwo(this);
    }

    public Player.GClass1594 CreateOperationThree()
    {
        return new OperationThree(this);
    }

    public Player.GClass1594 CreateOperationFour()
    {
        return new OperationFour(this);
    }

    public class OperationOne : Class1078
    {
        public OperationOne(CustomUsableItemController controller) : base(controller)
        {

        }

        public override void vmethod_0()
        {
            OperationTwo operation = usableItemController_0.InitiateOperation<OperationTwo>();
            operation.Start();

            action_1();

            if (action_0 != null)
            {
                operation.HideWeapon(action_0, bool_0);
            }
        }

        public override void SetLeftStanceAnimOnStartOperation()
        {
            player_0.MovementContext.LeftStanceController.DisableLeftStanceAnimFromHandsAction();
        }
    }

    public class OperationTwo : Class1072
    {

        public OperationTwo(CustomUsableItemController controller) : base(controller)
        {
        }

        public override void vmethod_0(GInterface354 oneItemOperation, Callback callback)
        {
            usableItemController_0.InitiateOperation<OperationFour>().Start(oneItemOperation.Item1, callback);
        }

        public override void SetAiming(bool isAiming)
        {
            base.SetAiming(isAiming);
        }


        public override void HideWeapon(Action onHidden, bool fastDrop)
        {
            State = Player.EOperationState.Finished;
            usableItemController_0.InitiateOperation<OperationThree>().Start(onHidden, fastDrop);
        }
    }

    public class OperationThree : Class1075
    {
        public OperationThree(CustomUsableItemController controller) : base(controller)
        {
        }

        public override void Start(Action onHidden, bool fastDrop)
        {
            base.Start(onHidden, fastDrop);
        }

    }

    public class OperationFour : Class1066
    {
        public OperationFour(CustomUsableItemController controller) : base(controller)
        {
        }

        public override void SetAiming(bool isAiming)
        {
            base.SetAiming(isAiming);
        }

        public override void vmethod_0()
        {
            usableItemController_0.InitiateOperation<OperationTwo>().Start();
        }
    }

    public class OperationPlaceholder : Class1067
    {
        public OperationPlaceholder(CustomUsableItemController controller) : base(controller)
        {

        }
    }
}

#endif