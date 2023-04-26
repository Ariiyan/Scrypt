using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static scr_Models;

public class CharacterController : MonoBehaviour
{
    private UnityEngine.CharacterController characterController;
    private DefaultInput defaultinput;
    public Vector2 input_movement;
    public Vector2 input_view;

    private Vector3 newCameraRotation;
    private Vector3 newCharacterRotation;

    [Header("References")]
    public Transform cameraHolder;
    public Transform Camera;
    public Transform feetTransform;
    public Transform feet;




    [Header("Setting")]
    public PlayerSettingModel playerSettings;
    public float viewClampYMin = -70;
    public float viewClampYMax = 80;
    public LayerMask playerMask;
    public LayerMask groundMask;

    [Header("Gravity")]
    public float gravityAmount;
    public float gravityMin;
    private float playerGravity;

    public Vector3 jumpingForce;
    private Vector3 jumpingForceVelocity;

    [Header("Stance")]
    public PlayerStance playerStance;
    public float playerStanceSmoothing;

    public float cameraStandHeight;
    public float cameraCrouchHeight;
    public float cameraProneHeight;

    public CharaterStance playerStandStance;
    public CharaterStance playerCrouchStance;
    public CharaterStance playerProneStance;

    public float stanceCheckErrorMargin = 0.05f;
    private float cameraHeight;
    private float cameraHeightVelocity;


    private Vector3 stanceCapsuleCenter;
    private Vector3 stanceCapsuleCenterVelocity;

    private float stanceCapsuleHeight;
    private float stanceCapsuleHeightVelocity;

    
    public bool issprinting;
    private Vector3 newMovementSpeed;
    private Vector3 newMovementSpeedVelocity;


    [Header("Weapon")]
    public WeaponController currentWeapon;
    public float weaponAnimationSpeed;

    [Header("Is Grounded / Falling")]
    public float isGroundedRadius;
    public float isFallingSpeed;


    [HideInInspector]
    public bool isGrounded;
    [HideInInspector]
    public bool isFalling;

    [Header("Aiming In")]
    public bool isAimingIn;


    [Header("Leaning")]
    public Transform LeanPivot;
    public float currentLean;
    public float targetLean;
    public float leanAngle;
    public float leanSmoothing;
    public float leanVelocity;



    private bool isLeaningLeft;
    public bool isLeaningRight;


    private void Awake()
    {

        defaultinput = new DefaultInput();

        defaultinput.Character.Movement.performed += e => input_movement = e.ReadValue<Vector2>();
        defaultinput.Character.View.performed += e => input_view = e.ReadValue<Vector2>();
        defaultinput.Character.Jump.performed += e => Jump();
        defaultinput.Character.Crouch.performed += e => Crouch();
        defaultinput.Character.Prone.performed += e => Prone();
        defaultinput.Character.Sprint.performed += e => ToggleSprint();
        defaultinput.Character.SprintReleased.performed += e => StopSprint();

        defaultinput.Weapon.Fire2Pressed.performed += e => AimingInPressed();
        defaultinput.Weapon.Fire2Released.performed += e => AimingInReleased();

        defaultinput.Character.LeanLeftPressed.performed += e => isLeaningLeft = true;
        defaultinput.Character.LeanLeftReleased.performed += e => isLeaningLeft = false;


        defaultinput.Character.LeanRightPressed.performed += e => isLeaningRight = true;
        defaultinput.Character.LeanRightReleased.performed += e => isLeaningRight = false;

        defaultinput.Enable();

        newCameraRotation = cameraHolder.localRotation.eulerAngles;
        newCharacterRotation = transform.localRotation.eulerAngles;

        characterController = GetComponent<UnityEngine.CharacterController>();

        cameraHeight = cameraHolder.localPosition.y;

        if (currentWeapon)
        {
            currentWeapon.Initialise(this);
        }

    }


    private void Update()
    {
        CalculateView();
        CalculateMovement();
        CalculateJump();
        CalculateStance();
        SetIsGrounded();
        SetIsFalling();
        CalculateLeaning();
        CalculateAimingIn();


    }

    private void CalculateView()
    {

        newCharacterRotation.y += (isAimingIn ? playerSettings.ViewXSensitivity * playerSettings.AimingSensitivityEffector : playerSettings.ViewXSensitivity) * (playerSettings.ViewXInverted ? -input_view.x : input_view.x) * Time.deltaTime;
        transform.rotation = Quaternion.Euler(newCharacterRotation);



        newCameraRotation.x += (isAimingIn ? playerSettings.ViewYSensitivity * playerSettings.AimingSensitivityEffector : playerSettings.ViewYSensitivity) * (playerSettings.ViewYInverted ? input_view.y : -input_view.y) * Time.deltaTime;
        newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, viewClampYMin, viewClampYMax);

        cameraHolder.localRotation = Quaternion.Euler(newCameraRotation);


    }

    private void CalculateMovement()
{
    float forwardSpeed = input_movement.y * (issprinting ? playerSettings.RunningForwardSpeed : playerSettings.WalkingForwardSpeed);
    float sidewaysSpeed = input_movement.x * playerSettings.WalkingStraightSpeed;
    if (issprinting && playerStance != PlayerStance.Prone) forwardSpeed *= playerSettings.SprintSpeedMultiplier;
    float movementSpeedEffector = playerStance == PlayerStance.Crouch ? playerSettings.CrouchSpeedEffector :
        playerStance == PlayerStance.Prone ? playerSettings.ProneSpeedEffector : 1f;
    forwardSpeed *= movementSpeedEffector;
    sidewaysSpeed *= movementSpeedEffector;

    Vector3 forwardMovement = transform.forward * forwardSpeed * Time.deltaTime;
    Vector3 sidewaysMovement = transform.right * sidewaysSpeed * Time.deltaTime;

    if (isGrounded)
    {
        playerGravity = -gravityMin;
    }
    else
    {
        playerGravity -= gravityAmount * Time.deltaTime;
    }
    
    if(isAimingIn)
        {
            playerSettings.SpeedEffector = playerSettings.AimingSpeedEffector;
        } 
    

    newMovementSpeed.y = playerGravity;
    
        if (isGrounded && jumpingForce.y < 0f)
    {
        jumpingForce.y = 0f;
    }

        weaponAnimationSpeed = characterController.velocity.magnitude / playerSettings.WalkingForwardSpeed * playerSettings.SpeedEffector;

        if (weaponAnimationSpeed > 1)
        {
            weaponAnimationSpeed = 1;
        }


    Vector3 gravityMovement = Vector3.up * playerGravity * Time.deltaTime;
    Vector3 jumpingMovement = jumpingForce * Time.deltaTime;

    characterController.Move(forwardMovement + sidewaysMovement + gravityMovement + jumpingMovement);
}

    private void AimingInPressed()
    {
        isAimingIn = true;
    }

    private void AimingInReleased()
    {
        isAimingIn = false;
    }

    private void CalculateAimingIn()
    {
        if (!currentWeapon)
        {
            return;
        }
        currentWeapon.isAimingIn = isAimingIn;    }

    private void CalculateJump()

    {
        jumpingForce = Vector3.SmoothDamp(jumpingForce, Vector3.zero, ref jumpingForceVelocity, playerSettings.JumpingFalloff);
    }
    private void CalculateStance()
    {

        var currentstance = playerStandStance;

        if (playerStance == PlayerStance.Crouch)
        {
            currentstance = playerCrouchStance;
        }
        else if (playerStance == PlayerStance.Prone)
        {
            currentstance = playerProneStance;
        }


        cameraHeight = Mathf.SmoothDamp(cameraHolder.localPosition.y, currentstance.CameraHeight, ref cameraHeightVelocity, playerStanceSmoothing);
        cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, cameraHeight, cameraHolder.localPosition.z);

        characterController.height = Mathf.SmoothDamp(characterController.height, currentstance.StanceCollider.height, ref stanceCapsuleHeightVelocity, playerStanceSmoothing);
        characterController.center = Vector3.SmoothDamp(characterController.center, currentstance.StanceCollider.center, ref stanceCapsuleCenterVelocity, playerStanceSmoothing);
    
   
    }
    private void Jump()
    {

        if (!isGrounded || playerStance == PlayerStance.Prone)
        {
            return;

        }


        if (playerStance == PlayerStance.Crouch)
        {
            if (StanceCheck(playerStandStance.StanceCollider.height))
            {
                return;
            }

            playerStance = PlayerStance.Stand;
            return;
        }


        jumpingForce += jumpingForceVelocity * Time.deltaTime;
        playerGravity = 0;

        currentWeapon.TriggerJump();

    }


    private void Crouch()
    
    { 
        if (playerStance == PlayerStance.Crouch)
        {

            if (StanceCheck(playerStandStance.StanceCollider.height)){
                return;
            } 

            playerStance = PlayerStance.Stand;
            return;
        }


        if (StanceCheck(playerCrouchStance.StanceCollider.height)){
            return;
        }

        playerStance = PlayerStance.Crouch;

    }
    private void Prone()
    {
        playerStance = PlayerStance.Prone;
    }
    private bool StanceCheck(float stanceCheckHeight)
    {

        var start = new Vector3(feetTransform.position.x, feetTransform.position.y + characterController.radius + stanceCheckErrorMargin, feetTransform.position.z);
        var end = new Vector3(feetTransform.position.x, feetTransform.position.y - characterController.radius - stanceCheckErrorMargin + stanceCheckHeight, feetTransform.position.z);




        return Physics.CheckCapsule(start, end, characterController.radius, playerMask);
    }
    private void ToggleSprint()
    {
        if (input_movement.y <= 0.2f)
        {
            issprinting = false;
            return;
        }
        issprinting = !issprinting;
    }

    private void StopSprint()
    {
        issprinting = false;
    }


    private void SetIsGrounded()
    {
        isGrounded = Physics.CheckSphere(feetTransform.position, playerSettings.isGroundedRadius, groundMask);
    }



    private void SetIsFalling()
    {
       
        isFalling = (!isGrounded && characterController.velocity.magnitude >= playerSettings.isFallingSpeed);

    }





    private void CalculateLeaning()

    {

        if (isLeaningLeft)
        {
            targetLean = leanAngle;
        }
        else if (isLeaningRight)
        {
            targetLean = -leanAngle;
        }
        else
        {
            targetLean = 0;
        }

        currentLean = Mathf.SmoothDamp(currentLean, targetLean, ref leanVelocity, leanSmoothing);

        LeanPivot.localRotation = Quaternion.Euler(new Vector3(0,0,currentLean));
    }


   
}

