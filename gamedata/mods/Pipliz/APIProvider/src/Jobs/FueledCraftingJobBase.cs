﻿using NPC;
using Pipliz.APIProvider.Recipes;
using Pipliz.JSON;
using System.Collections.Generic;

namespace Pipliz.APIProvider.Jobs
{
	public class FueledCraftingJobBase : BlockJobBase, IRecipeLimitsProvider
	{
		protected ushort blockType;

		bool shouldTakeItems;
		float storedFuel;
		NPCInventory blockInventory;
		RecipeFueled selectedRecipe;
		Vector3Int positionNPC;
		int recipesToCraft;

		public virtual ITrackableBlock InitializeOnAdd (Vector3Int position, ushort type, Players.Player player)
		{
			blockType = type;
			blockInventory = new NPCInventory(10000000f);
			positionNPC = GetPositionNPC(position);
			InitializeJob(player, position, 0);
			return this;
		}

		public override ITrackableBlock InitializeFromJSON (Players.Player player, JSONNode node)
		{
			blockInventory = new NPCInventory(node["inventory"]);
			storedFuel = node.GetAs<float>("storedFuel");
			blockType = ItemTypes.IndexLookup.GetIndex(node.GetAs<string>("type"));
			Vector3Int position = (Vector3Int)node["position"];
			positionNPC = GetPositionNPC(position);
			InitializeJob(player, position, node.GetAs<int>("npcID"));
			return this;
		}

		public override Vector3Int GetJobLocation ()
		{
			return positionNPC;
		}

		public override bool NeedsItems { get { return shouldTakeItems; } }

		public virtual RecipeFueled[] GetPossibleRecipes { get { return RecipeManager.RecipeFueledStorage[NPCTypeKey]; } }

		public virtual int MaxRecipeCraftsPerHaul { get { throw new System.NotImplementedException(); } }

		public virtual Vector3Int GetPositionNPC (Vector3Int position) { throw new System.NotImplementedException(); }

		public override JSONNode GetJSON ()
		{
			return base.GetJSON()
				.SetAs("position", (JSONNode)position)
				.SetAs("type", ItemTypes.IndexLookup.GetName(blockType))
				.SetAs("inventory", blockInventory.GetJSON())
				.SetAs("storedFuel", storedFuel);
		}

		public virtual void OnLit () { }

		public virtual void OnUnlit ()
		{
			ServerManager.TryChangeBlock(position, blockType);
		}

		public override void OnNPCDoJob (ref NPCBase.NPCState state)
		{
			state.JobIsDone = true;
			usedNPC.LookAt(position.Vector);
			if (!state.Inventory.IsEmpty) {
				storedFuel += state.Inventory.TotalFuel;
				state.Inventory.ClearFuel();
				state.Inventory.Dump(blockInventory);
			}
			if (selectedRecipe != null) {
				if (recipesToCraft > 0 && selectedRecipe.IsPossible(usedNPC.Colony.UsedStockpile, blockInventory, storedFuel)) {
					storedFuel -= selectedRecipe.FuelPerCraft;
					blockInventory.Take(selectedRecipe.Requirements);
					blockInventory.Add(selectedRecipe.Results);
					recipesToCraft--;
					OnLit();
					state.SetIndicator(NPCIndicatorType.Crafted, TimeBetweenJobs, selectedRecipe.Results[0].Type);
					state.JobIsDone = false;
					return;
				} else {
					recipesToCraft = 0;
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
			OnUnlit();
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
						recipesToCraft = 0;
						OverrideCooldown(0.5);
						break;
					case Recipe.RecipeMatchType.FoundCraftable:
						selectedRecipe = recipeMatch.FoundRecipe;
						recipesToCraft = Math.Min(recipeMatch.FoundRecipeCount, MaxRecipeCraftsPerHaul);
						float fuelNeeded = (recipesToCraft * selectedRecipe.FuelPerCraft) - storedFuel;
						if (fuelNeeded > 0f) {
							if (!usedNPC.Colony.UsedStockpile.TryTransferFuel(fuelNeeded, state.Inventory)) {
								state.SetIndicator(NPCIndicatorType.NoFuel, 5.0f);
								shouldTakeItems = true;
								OverrideCooldown(5.0);
								return;
							}
						}
						for (int i = 0; i < selectedRecipe.Requirements.Count; i++) {
							state.Inventory.Add(selectedRecipe.Requirements[i] * recipesToCraft);
							usedNPC.Colony.UsedStockpile.Remove(selectedRecipe.Requirements[i] * recipesToCraft);
						}
						OverrideCooldown(0.5);
						break;
				}
			}
		}

		// IRecipeLimitsProvider
		public virtual Recipe[] GetCraftingLimitsRecipes ()
		{
			RecipeFueled[] list;
			if (RecipeManager.RecipeFueledStorage.TryGetValue(NPCTypeKey, out list)) {
				return list;
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
