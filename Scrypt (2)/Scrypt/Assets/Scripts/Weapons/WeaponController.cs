using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static scr_Models;

public class WeaponController : MonoBehaviour
{

    private CharacterController characterController;


    [Header("References")]
    public Animator weaponAnimator;


    [Header("Settings")]
    public WeaponSettingsModel settings;

    bool isInitialised;

    Vector3 newWeaponRotation;
    Vector3 newWeaponRotationVelocity;

    Vector3 targetWeaponRotation;
    Vector3 targetWeaponRotationVelocity;



    Vector3 newWeaponMovementRotation;
    Vector3 newWeaponMovementRotationVelocity;

    Vector3 targetWeaponMovementRotation;
    Vector3 targetWeaponMovementRotationVelocity;


    private bool isGroundedTrigger;
    private bool isFallingTrigger;

    public float fallingDelay;


    [Header("Weapon Breathing")]
    public Transform weaponSwayObject;


    public float swayAmountA = 1;
    public float swayAmountB = 2;
    public float swayScale = 600;
    public float swayLerpSpeed = 14;


    public float swayTime;
    public Vector3 SwayPosition;


    public bool isAimingIn;



    [Header("Sights")]
    public Transform sightTarget;
    public float sightoffset;
    public float aiminginTime;


    public Vector3 weaponSwayPosition;
    public Vector3 weaponSwayPositionVelocity;



    private void Start()
    {
        newWeaponRotation = transform.localRotation.eulerAngles;

    }

    public void Initialise(CharacterController CharacterController)
    {
        characterController = CharacterController;
        isInitialised = true;
    }


    private void Update()
    {
        if (!isInitialised)
        {
            return;
        }

        CalculateWeaponRotation();
        SetWeaponAnimations();
        CalculateWeaponSway();
        CalculateAimingIn();

    }

   

    public void TriggerJump()
    {
        isGroundedTrigger = false;
    }
  


    private void CalculateWeaponRotation()
    {
        weaponAnimator.speed = characterController.weaponAnimationSpeed;




        newWeaponRotation.y += (isAimingIn ? settings.SwayAmount / 2 : settings.SwayAmount) * (settings.SwayXInverted ? -characterController.input_view.x : characterController.input_view.x) * Time.deltaTime;
        newWeaponRotation.x += (isAimingIn ? settings.SwayAmount / 2 : settings.SwayAmount) * (settings.SwayYInverted ? characterController.input_view.y : -characterController.input_view.y) * Time.deltaTime;




        targetWeaponRotation.x = Mathf.Clamp(targetWeaponRotation.x, -settings.SwayClampX, settings.SwayClampX);
        targetWeaponRotation.x = Mathf.Clamp(targetWeaponRotation.y, -settings.SwayClampY, settings.SwayClampY);
        targetWeaponRotation.z = isAimingIn ? 0 : targetWeaponRotation.y;




        targetWeaponRotation = Vector3.SmoothDamp(targetWeaponRotation, Vector3.zero, ref targetWeaponRotationVelocity, settings.SwayResetSmoothing);
        newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, targetWeaponRotation, ref newWeaponRotationVelocity, settings.SwaySmoothing);

        targetWeaponMovementRotation.z = (isAimingIn ? settings.MovementSwayX / 3 : settings.MovementSwayX) * (settings.MovementSwayXInverted ? -characterController.input_movement.x : characterController.input_movement.x); ;
        targetWeaponMovementRotation.x = (isAimingIn ? settings.MovementSwayY / 3 : settings.MovementSwayY) * (settings.MovementSwayYInverted ? -characterController.input_movement.y : characterController.input_movement.y); ;


        targetWeaponMovementRotation = Vector3.SmoothDamp(targetWeaponMovementRotation, Vector3.zero, ref targetWeaponMovementRotationVelocity, settings.SwayResetSmoothing);
        newWeaponMovementRotation = Vector3.SmoothDamp(newWeaponMovementRotation, targetWeaponMovementRotation, ref newWeaponMovementRotationVelocity, settings.SwaySmoothing);


        transform.localRotation = Quaternion.Euler(newWeaponRotation + newWeaponMovementRotation);
    }

    private void SetWeaponAnimations()
    {
        if (characterController.isGrounded)
        {
            fallingDelay = 0;
        }
        else
        {
            fallingDelay += Time.deltaTime;
        }

        if (characterController.isGrounded && !isGroundedTrigger && fallingDelay > 0.1f)
        {
            weaponAnimator.SetTrigger("Land");

            isGroundedTrigger = true;
        }
        if (!characterController.isGrounded && isGroundedTrigger)
        {
            weaponAnimator.SetTrigger("Falling");
            isGroundedTrigger = false;
        }

        weaponAnimator.SetBool("IsSprinting", characterController.issprinting);

        weaponAnimator.SetFloat("WeaponAnimationSpeed", characterController.weaponAnimationSpeed);
    }


    private void CalculateWeaponSway()
    {
        var targetPosition = LissajousCurve(swayTime, swayAmountA, swayAmountB / (isAimingIn ? swayScale * 3 : swayScale));
        SwayPosition = Vector3.Lerp(SwayPosition, targetPosition, Time.smoothDeltaTime * swayLerpSpeed);
        swayTime += Time.deltaTime;


        if (swayTime > 6.3f)
        {
            swayTime = 0;
        }


    }

    private Vector3 LissajousCurve(float time, float A, float B)
    {
        return new Vector3(Mathf.Sin(time), A * Mathf.Sin(B * time + Mathf.PI), 0.0f);
    }


    private void CalculateAimingIn()
    {
        var targetPosition = transform.position;

        if (isAimingIn)
        {
            targetPosition = characterController.Camera.transform.position + (weaponSwayObject.transform.position - sightTarget.position) + (characterController.Camera.transform.forward * sightoffset);
        }


        weaponSwayPosition = weaponSwayObject.transform.position;
        weaponSwayPosition = Vector3.SmoothDamp(weaponSwayPosition, targetPosition, ref weaponSwayPositionVelocity, aiminginTime);
        weaponSwayObject.transform.position = weaponSwayPosition + SwayPosition;

    }

   
}