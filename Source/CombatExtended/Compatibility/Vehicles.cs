using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using Verse.Sound;
using System.Reflection.Emit;
using System;
using UnityEngine;
using RimWorld;
using Vehicles;


namespace CombatExtended.Compatibility
{
    class Vehicles: IPatch
    {
	private static bool active = false;
	public bool CanInstall()
        {
            if (!ModLister.HasActiveModWithName("Vehicle Framework"))
            {
                return false;
            }
            return true;
        }
	public IEnumerable<string> GetCompatList() {
	    yield break;
	}

	public void Install()
        {
	    VehicleTurret.ProjectileAngleCE = ProjectileCE.GetShotAngle;
	    VehicleTurret.LaunchProjectileCE = LaunchProjectileCE;
	    active = true;
	}
	public static object LaunchProjectileCE(ThingDef projectileDef,
						Vector2 origin,
						LocalTargetInfo target,
						VehiclePawn vehicle,
						float shotAngle,
						float shotRotation,
						float shotHeight,
						float shotSpeed)
	{
	    ProjectileCE projectile = (ProjectileCE)ThingMaker.MakeThing(projectileDef, null);
	    GenSpawn.Spawn(projectile, vehicle.Position, vehicle.Map);

	    projectile.ExactPosition = origin;
	    projectile.canTargetSelf = false;
	    projectile.minCollisionDistance = 1;
	    projectile.intendedTarget = target;
	    projectile.mount = null;
	    projectile.AccuracyFactor = 1;

	    
	    projectile.Launch(
			      vehicle,
			      origin,
			      shotAngle,
			      shotRotation,
			      shotHeight,
			      shotSpeed,
			      vehicle);
	    return projectile;
	}

	public static bool GetCollisionBodyFactors(Pawn pawn, out Vector2 ret) {
	    ret = new Vector2();
	    if (active) {
		return _GetCollisionBodyFactors(pawn, out ret);
	    }
	    else {
		return false;
	    }
	}

	private static bool _GetCollisionBodyFactors(Pawn pawn, out Vector2 ret) {
	    ret = new Vector2();
	    if (pawn is VehiclePawn vehicle) {
		ret = new Vector2(1, vehicle.def.fillPercent);
		return true;
	    }
	    return false;
	}

    }
}
