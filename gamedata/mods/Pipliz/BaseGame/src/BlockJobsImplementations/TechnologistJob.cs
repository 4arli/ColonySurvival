﻿using BlockTypes.Builtin;
using Pipliz.Mods.APIProvider.Jobs;
using Server.NPCs;

namespace Pipliz.Mods.BaseGame.BlockNPCs
{
	public class TechnologistJob : CraftingJobBase, IBlockJobBase, INPCTypeDefiner
	{
		public static float StaticCraftingCooldown = 10f;

		public override string NPCTypeKey { get { return "pipliz.technologist"; } }

		public override float CraftingCooldown
		{
			get { return StaticCraftingCooldown; }
			set { StaticCraftingCooldown = value; }
		}

		public override int MaxRecipeCraftsPerHaul { get { return 2; } }

		NPCTypeStandardSettings INPCTypeDefiner.GetNPCTypeDefinition ()
		{
			return new NPCTypeStandardSettings()
			{
				keyName = NPCTypeKey,
				printName = "Technologist",
				maskColor1 = new UnityEngine.Color32(28, 26, 26, 255),
				type = NPCTypeID.GetNextID()
			};
		}

		protected override bool IsValidWorldType (ushort type)
		{
			return type == BuiltinBlocks.TechnologistTable;
		}
	}
}
