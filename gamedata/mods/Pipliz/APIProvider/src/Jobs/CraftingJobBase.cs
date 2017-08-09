﻿using NPC;
using Pipliz.APIProvider.Recipes;
using Pipliz.JSON;
using System.Collections.Generic;

namespace Pipliz.APIProvider.Jobs
{
	public class CraftingJobBase : BlockJobBase, IRecipeLimitsProvider
	{
		NPCInventory blockInventory;
		bool shouldTakeItems;
		Recipe selectedRecipe;

		public override JSONNode GetJSON ()
		{
			return base.GetJSON()
				.SetAs("position", (JSONNode)position)
				.SetAs("inventory", blockInventory.GetJSON());
		}

		public virtual ITrackableBlock InitializeOnAdd (Vector3Int position, ushort type, Players.Player player)
		{
			blockInventory = new NPCInventory(10000000f);
			InitializeJob(player, position, 0);
			return this;
		}

		public override ITrackableBlock InitializeFromJSON (Players.Player player, JSONNode node)
		{
			blockInventory = new NPCInventory(node["inventory"]);
			InitializeJob(player, (Vector3Int)node["position"], node.GetAs<int>("npcID"));
			return this;
		}

		public override bool NeedsItems { get { return shouldTakeItems; } }

		public virtual Recipe[] GetPossibleRecipes { get { return RecipeManager.RecipeStorage[NPCTypeKey]; } }

		public virtual int MaxRecipeCraftsPerHaul { get { throw new System.NotImplementedException(); } }

		public override void OnNPCDoJob (ref NPCBase.NPCState state)
		{
			state.JobIsDone = true;
			usedNPC.LookAt(position.Vector);
			if (!state.Inventory.IsEmpty) {
				usedNPC.Inventory.Dump(blockInventory);
			}
			if (selectedRecipe != null) {
				if (selectedRecipe.IsPossible(usedNPC.Colony.UsedStockpile, blockInventory)) {
					blockInventory.Take(selectedRecipe.Requirements);
					blockInventory.Add(selectedRecipe.Results);
					state.SetIndicator(NPCIndicatorType.Crafted, TimeBetweenJobs, selectedRecipe.Results[0].Type);
					state.JobIsDone = false;
				} else {
					selectedRecipe = null;
					blockInventory.Dump(usedNPC.Inventory);
					if (!state.Inventory.IsEmpty) {
						shouldTakeItems = true;
					}
					OverrideCooldown(0.1);
				}
			} else {
				var recipeMatch = Recipe.MatchRecipe(GetPossibleRecipes, usedNPC.Colony.UsedStockpile);
				switch (recipeMatch.MatchType) {
					case Recipe.RecipeMatchType.AllDone:
					case Recipe.RecipeMatchType.FoundMissingRequirements:
						if (!state.Inventory.IsEmpty || !blockInventory.IsEmpty) {
							blockInventory.Dump(usedNPC.Inventory);
							shouldTakeItems = true;
						} else {
							state.JobIsDone = false;
							if (recipeMatch.MatchType == Recipe.RecipeMatchType.AllDone) {
								state.SetIndicator(NPCIndicatorType.SuccessIdle, 6f);
							} else {
								state.SetIndicator(NPCIndicatorType.MissingItem, 6f, recipeMatch.FoundRecipe.FindMissingType(owner));
							}
							OverrideCooldown(6.0);
						}
						break;
					case Recipe.RecipeMatchType.FoundCraftable:
						blockInventory.Dump(usedNPC.Inventory);
						selectedRecipe = recipeMatch.FoundRecipe;
						shouldTakeItems = true;
						OverrideCooldown(0.5);
						break;
				}
			}
		}

		public override void OnNPCDoStockpile (ref NPCBase.NPCState state)
		{
			state.Inventory.TryDump(usedNPC.Colony.UsedStockpile);
			state.JobIsDone = true;
			if (shouldTakeItems) {
				shouldTakeItems = false;
				var recipeMatch = Recipe.MatchRecipe(GetPossibleRecipes, usedNPC.Colony.UsedStockpile);
				switch (recipeMatch.MatchType) {
					case Recipe.RecipeMatchType.FoundMissingRequirements:
					case Recipe.RecipeMatchType.AllDone:
						OverrideCooldown(0.5);
						break;
					case Recipe.RecipeMatchType.FoundCraftable:
						selectedRecipe = recipeMatch.FoundRecipe;
						int count = Math.Min(recipeMatch.FoundRecipeCount, MaxRecipeCraftsPerHaul);
						for (int i = 0; i < selectedRecipe.Requirements.Count; i++) {
							state.Inventory.Add(selectedRecipe.Requirements[i] * count);
							usedNPC.Colony.UsedStockpile.Remove(selectedRecipe.Requirements[i] * count);
						}
						OverrideCooldown(0.5);
						break;
				}
			}
		}

		// IRecipeLimitsProvider
		public virtual Recipe[] GetCraftingLimitsRecipes ()
		{
			Recipe[] arr;
			if (RecipeManager.RecipeStorage.TryGetValue(NPCTypeKey, out arr)) {
				return arr;
			} else {
				return null;
			}
		}

		// IRecipeLimitsProvider
		public virtual List<string> GetCraftingLimitsTriggers ()
		{
			return null;
		}

		// IRecipeLimitsProvider
		public virtual string GetCraftingLimitsIdentifier ()
		{
			return NPCTypeKey;
		}
	}
}
