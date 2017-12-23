﻿using Pipliz.Mods.APIProvider.Science;
using Server.Science;

namespace Pipliz.Mods.BaseGame.Researches
{
	[AutoLoadedResearchable]
	public class GunSmithShop : BaseResearchable
	{
		public GunSmithShop ()
		{
			key = "pipliz.baseresearch.gunsmithshop";
			icon = "gamedata/textures/icons/gunsmithshop.png";
			iterationCount = 25;
			AddIterationRequirement("lead");
			AddIterationRequirement("sciencebagmilitary");
			AddDependency("pipliz.baseresearch.fineryforge");
			AddDependency("pipliz.baseresearch.sciencebagmilitary");
		}

		public override void OnResearchComplete (ScienceManagerPlayer manager, EResearchCompletionReason reason)
		{
			RecipeStorage.GetPlayerStorage(manager.Player).SetRecipeAvailability("pipliz.crafter.gunsmithshop", true, "pipliz.crafter");
			RecipePlayer.UnlockOptionalRecipe(manager.Player, "pipliz.player.gunsmithshop");
		}
	}
}
