﻿using Pipliz.APIProvider.Jobs;
using Server.NPCs;

namespace Pipliz.BlockNPCs.Implementations
{
	public class ShopJob : CraftingJobBase, IBlockJobBase, INPCTypeDefiner
	{
		public static float StaticCraftingCooldown = 10f;

		public override string NPCTypeKey { get { return "pipliz.merchant"; } }

		public override float CraftingCooldown
		{
			get { return StaticCraftingCooldown; }
			set { StaticCraftingCooldown = value; }
		}

		public override int MaxRecipeCraftsPerHaul { get { return 1; } }

		NPCTypeStandardSettings INPCTypeDefiner.GetNPCTypeDefinition ()
		{
			return new NPCTypeStandardSettings()
			{
				keyName = NPCTypeKey,
				printName = "Merchant",
				maskColor1 = new UnityEngine.Color32(146, 31, 31, 255),
				type = NPCTypeID.GetNextID()
			};
		}

		protected override string GetRecipeLocation ()
		{
			return System.IO.Path.Combine(ModEntries.ModGamedataDirectory, "shopping.json");
		}
	}
}
