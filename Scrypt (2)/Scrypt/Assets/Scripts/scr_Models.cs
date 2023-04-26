using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_Models : MonoBehaviour
{
    #region - Player -

    public enum PlayerStance
    {
        Stand,
        Crouch,
        Prone,
    }

    [Serializable]
    public class PlayerSettingModel
    {
        [Header("View Settings")]

        public float ViewXSensitivity;
        public float ViewYSensitivity;

        public float AimingSensitivityEffector;

        public bool ViewXInverted;
        public bool ViewYInverted;

        [Header("Movement Settings")]
        public bool SprintHold;
        public float MovementSmooting;
        public float SprintSpeedMultiplier;

        [Header("Movement - Running")]
        public float RunningForwardSpeed;
        public float RunningStraightSpeed;

        [Header("Movement - Walking")]
        public float WalkingForwardSpeed;
        public float WalkingStraightSpeed;
        public float WalkingBackwardSpeed;

        [Header("Jumping")]
        public float JumpingHeight;
        public float JumpingFalloff;
        public float FallingSmoothing;

        [Header("Speed Effectors")]
        public float SpeedEffector = 1;
        public float CrouchSpeedEffector;
        public float ProneSpeedEffector;
        public float FallingSpeedEffector;
        public float AimingSpeedEffector;

        [Header("Is Grounded")]
        public float isGroundedRadius;
        public float isFallingSpeed;


    }
    
    [Serializable]
        public class CharaterStance
        {
            public float CameraHeight;
            public CapsuleCollider StanceCollider;
        }



    #endregion


    #region - Weapon -




    [Serializable]
    public class WeaponSettingsModel
    {
        [Header("Weapon Sway")]
        public float SwayAmount;
        public bool SwayYInverted;
        public bool SwayXInverted;
        public float SwaySmoothing;
        public float SwayResetSmoothing;
        public float SwayClampX;
        public float SwayClampY;

        [Header("Weapon Movement Sway")]
        public float MovementSwayX;
        public float MovementSwayY;
        public bool MovementSwayYInverted;
        public bool MovementSwayXInverted;
        public float MovementSwaySmoothing;
        
        

    }


    #endregion
}
