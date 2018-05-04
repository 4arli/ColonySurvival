﻿using BlockTypes.Builtin;
using Pipliz.Mods.APIProvider.Jobs;
using Server.NPCs;

namespace Pipliz.Mods.BaseGame.BlockNPCs
{
	public class WorkBenchJob : CraftingJobBase, IBlockJobBase, INPCTypeDefiner
	{
		public static float StaticCraftingCooldown = 8f;

		public override string NPCTypeKey { get { return "pipliz.crafter"; } }

		public override float CraftingCooldown
		{
			get { return StaticCraftingCooldown; }
			set { StaticCraftingCooldown = value; }
		}

		public override int MaxRecipeCraftsPerHaul { get { return 5; } }

		NPCTypeStandardSettings INPCTypeDefiner.GetNPCTypeDefinition ()
		{
			return new NPCTypeStandardSettings()
			{
				keyName = NPCTypeKey,
				printName = "Crafter",
				maskColor1 = new UnityEngine.Color32(81, 121, 123, 255),
				type = NPCTypeID.GetNextID()
			};
		}

		protected override void OnRecipeCrafted ()
		{
			base.OnRecipeCrafted();
			ServerManager.SendAudio(position.Vector, "crafting");
		}

		protected override bool IsValidWorldType (ushort type)
		{
			return type == BuiltinBlocks.WorkBench;
		}
	}
}
