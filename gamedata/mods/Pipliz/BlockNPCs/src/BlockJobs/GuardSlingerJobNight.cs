﻿using BlockTypes.Builtin;
using Pipliz.APIProvider.Jobs;
using Server.NPCs;
using System.Collections.Generic;
using UnityEngine;

namespace Pipliz.BlockNPCs.Implementations
{
	public class GuardSlingerJobNight : GuardBaseJob, INPCTypeDefiner
	{
		public override string NPCTypeKey { get { return "pipliz.guardslingernight"; } }

		protected override GuardSettings SetupSettings ()
		{
			GuardSettings set = new GuardSettings();
			set.cooldownMissingItem = 1.5f;
			set.cooldownSearchingTarget = 0.5f;
			set.cooldownShot = 1.5f;
			set.range = 15;
			set.recruitmentItem = new InventoryItem(BuiltinBlocks.Sling);
			set.shootItem = new List<InventoryItem>() { new InventoryItem(BuiltinBlocks.SlingBullet) };
			set.shootDamage = 10f;
			set.sleepSafetyPeriod = 1f;
			set.sleepType = EGuardSleepType.Day;
			set.typeXN = BuiltinBlocks.GuardSlingerJobNightXN;
			set.typeXP = BuiltinBlocks.GuardSlingerJobNightXP;
			set.typeZN = BuiltinBlocks.GuardSlingerJobNightZN;
			set.typeZP = BuiltinBlocks.GuardSlingerJobNightZP;
			return set;
		}

		NPCTypeStandardSettings INPCTypeDefiner.GetNPCTypeDefinition ()
		{
			return new NPCTypeStandardSettings()
			{
				keyName = NPCTypeKey,
				printName = "Night Slinger Guard",
				maskColor1 = new Color32(136, 136, 136, 255),
				type = NPCTypeID.GetNextID()
			};
		}
	}
}
