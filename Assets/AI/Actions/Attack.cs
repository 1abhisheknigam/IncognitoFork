﻿using UnityEngine;
using RAIN.Action;
using RAIN.Core;

/// <summary>
/// Alerts all guards about the player's presence.
/// </summary>
[RAINAction]
public class Attack : RAINAction
{

    private float attackTimer = 0;
    private float attackTime = .3f;
    private float attackDamage = 0;
    private float projectileSpeed = 30;
    private float inaccuracy = .2f;
    private int leadCalcIters = 10;

    private GameObject projectile = (GameObject)Resources.Load("bullet", typeof(GameObject));


    bool started = false;

    float CALL_IN_DURATION = 3.5f;
    /// <summary>
    /// Updates the AI every frame.
    /// </summary>
    /// <param name="ai">The AI to update.</param>
    public override ActionResult Execute(AI ai)
    {
        Debug.Log("Attack");
        GameObject p = (GameObject)ai.WorkingMemory.GetItem(SquadManager.PLAYER);


        if (p == null)
        {
            Debug.Log("attack fail");
            return ActionResult.FAILURE;
        }

        attackPlayer(ai, p);
        

        attackTimer += Time.deltaTime;

        return ActionResult.RUNNING;
    }

    void attackPlayer(AI ai, GameObject player)
    {

        if (attackTimer > attackTime)
        {
            doRangedAttack(ai, player);
            attackTimer = 0;
        }

    }

    void doRangedAttack(AI ai, GameObject player)
    {
        GunBarrel barrel = ai.Body.GetComponent<GunBarrel>();
        barrel.Fire();
        Vector3 firePoint = barrel.barrelParticles.GetComponent<Transform>().position;

        Vector3 pos = player.GetComponent<Transform>().position + new Vector3(0, 1, 0); //throw up off the ground
        Vector3 vel = player.GetComponent<Rigidbody>().velocity;
        Vector3 target = getLeadingPosition(pos, vel, firePoint);

        GameObject obj = GameObject.Instantiate(projectile);
        obj.GetComponent<Transform>().position = firePoint;


        Bullet proj = obj.GetComponent<Bullet>();
        proj.setDamage(attackDamage);

        Vector3 bulletVel = (target - firePoint).normalized * projectileSpeed + inaccuracy * Random.onUnitSphere;
        proj.setVelocity(bulletVel);

    }

    Vector3 getLeadingPosition(Vector3 targetPos, Vector3 targetVel, Vector3 firePos)
    {

        Vector3 fireTarget = targetPos;

        for (int i = 0; i < leadCalcIters; i++)
        {
            float dist = (fireTarget - firePos).magnitude;
            float travelTime = dist / projectileSpeed;
            fireTarget = targetPos + targetVel * travelTime;
        }

        return fireTarget;
    }

}