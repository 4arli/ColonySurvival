﻿using Pipliz.Mods.APIProvider.Science;
using Server.Science;

namespace Pipliz.Mods.BaseGame.Researches
{
	[AutoLoadedResearchable]
	public class HealthSize3 : BaseResearchable
	{
		public HealthSize3 ()
		{
			key = "pipliz.baseresearch.healthsize3";
			icon = "gamedata/textures/icons/baseresearch_healthsize3.png";
			iterationCount = 50;
			AddIterationRequirement("sciencebagadvanced", 2);
			AddIterationRequirement("sciencebaglife", 2);
			AddDependency("pipliz.baseresearch.healthsize2");
		}

		public override void OnResearchComplete (ScienceManagerPlayer manager, EResearchCompletionReason reason)
		{
			manager.Player.GetTempValues(true).Set("pipliz.healthmax", 175f);
			if (reason == EResearchCompletionReason.ProgressCompleted) {
				manager.Player.SendHealthPacket();
			}
		}
	}
}
