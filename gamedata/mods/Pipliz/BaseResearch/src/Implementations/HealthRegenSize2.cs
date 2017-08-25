﻿using Pipliz.APIProvider.Science;
using Server.Science;

namespace Pipliz.BaseResearch.Implementations
{
	[AutoLoadedResearchable]
	public class HealthRegenSize2 : BaseResearchable
	{
		public HealthRegenSize2 ()
		{
			key = "pipliz.baseresearch.healthregensize2";
			icon = "baseresearch_healthregensize2.png";
			iterationCount = 25;
			AddIterationRequirement("sciencebagbasic");
			AddIterationRequirement("sciencebaglife", 3);
			AddDependency("pipliz.baseresearch.healthregensize1");
		}

		public override void OnResearchComplete (ScienceManagerPlayer manager)
		{
			manager.Player.SetTemporaryValue("pipliz.healthregenmax", 65f);
		}
	}
}
