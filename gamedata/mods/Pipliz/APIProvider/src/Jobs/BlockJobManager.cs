﻿namespace Pipliz.Mods.APIProvider.Jobs
{
	public class BlockJobManager<T> : IBlockJobManager where T : IBlockJobBase, ITrackableBlock, new()
	{
		BlockTracker<T> tracker;
		string blockName;

		public BlockJobManager (string blockName)
		{
			this.blockName = blockName;
		}

		public void RegisterCallback ()
		{
			ItemTypesServer.RegisterOnAdd(blockName, OnAdd);
			ItemTypesServer.RegisterOnRemove(blockName, OnRemove);
		}

		public void Load ()
		{
			tracker = new BlockTracker<T>(blockName);
		}

		public void OnSave ()
		{
			tracker.Save();
		}

		void OnRemove (Vector3Int position, ushort type, Players.Player player)
		{
			tracker.Remove(position);
		}

		void OnAdd (Vector3Int position, ushort type, Players.Player player)
		{
			tracker.Add((T)new T().InitializeOnAdd(position, type, player));
		}
	}
}
