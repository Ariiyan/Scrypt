using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject bullet;
    public AudioClip shootSound;

    public float shootForce, upwardForce;

    public float timeBetweenShooting, Spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;

    int bulletsLeft, bulletsShot;

    bool shooting, readyToShoot, reloading;

    public Camera fpsCam;
    public Transform BulletSpawn;

    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;


    public bool allowInvoke = true;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        MyInput();

        if (ammunitionDisplay != null)
            ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
    }

    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKey(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();


        if(readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;

            Shoot();
        }


    }


    private void Shoot()
    {
        readyToShoot = false;

        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75);

        Vector3 directionanWithoutSpread = targetPoint - BulletSpawn.position;

        float x = Random.Range(-Spread, Spread);
        float y = Random.Range(-Spread, Spread);

        Vector3 directionalWithSpread = directionanWithoutSpread + new Vector3(x, y, 0);

        GameObject currentBullet = Instantiate(bullet, BulletSpawn.position, Quaternion.identity);
        currentBullet.transform.forward = directionalWithSpread.normalized;

        currentBullet.GetComponent<Rigidbody>().AddForce(directionalWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        if (muzzleFlash != null)
        {
            GameObject flash = Instantiate(muzzleFlash, BulletSpawn.position, Quaternion.identity);
            flash.transform.forward = BulletSpawn.forward;
        }

        bulletsLeft--;
        bulletsShot++;

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);


        AudioSource.PlayClipAtPoint(shootSound, transform.position);

    }




    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
        
    }

    private void Reload()
    {
        if (bulletsLeft < magazineSize && !reloading)
        {
            reloading = true;
            Invoke("ReloadFinished", reloadTime);
        }
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
