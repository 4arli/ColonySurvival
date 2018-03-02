﻿using BlockTypes.Builtin;

namespace Pipliz.Mods.BaseGame.AreaJobs
{
	using APIProvider.AreaJobs;

	[AreaJobDefinitionAutoLoader]
	public class Hollyhock : AreaJobDefinitionDefault<Hollyhock>
	{
		public Hollyhock ()
		{
			identifier = "pipliz.hollyhockfarm";
			fileName = "hollyhockfarms";
			stages = new ushort[] {
				BuiltinBlocks.HollyhockStage1,
				BuiltinBlocks.HollyhockStage2
			};
			npcType = Server.NPCs.NPCType.GetByKeyNameOrDefault("pipliz.hollyhockfarmer");
			areaType = Shared.EAreaType.HollyhockFarm;
		}

		public override IAreaJob CreateAreaJob (Players.Player owner, Vector3Int min, Vector3Int max, int npcID = 0)
		{
			SetLayer(min, max, BuiltinBlocks.Dirt, -1, owner);
			return base.CreateAreaJob(owner, min, max, npcID);
		}
	}
}