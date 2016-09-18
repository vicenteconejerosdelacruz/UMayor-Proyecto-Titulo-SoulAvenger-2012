using UnityEngine;
using System.Collections.Generic;

namespace tk2dRuntime.TileMap
{
	[System.Serializable]
	public class LayerInfo
	{
		public string name;
		public int hash;
		public bool useColor;
		public bool generateCollider;
		public float z = 0.1f;
		public int unityLayer = 0;
		
		public LayerInfo()
		{
			unityLayer = 0;
			useColor = true;
			generateCollider = true;
		}
	}
}

public class tk2dTileMapData : ScriptableObject 
{
	// Start at this point
	public enum SortMethod
	{
		BottomLeft,
		TopLeft,
		BottomRight,
		TopRight,
	}
	
	public Vector3 tileSize;
	public Vector3 tileOrigin;

	public SortMethod sortMethod = SortMethod.BottomLeft;
	
	public Object[] tilePrefabs = new Object[0];

	[SerializeField]
	public List<tk2dRuntime.TileMap.LayerInfo> tileMapLayers = new List<tk2dRuntime.TileMap.LayerInfo>();
	
	public int NumLayers
	{
		get 
		{
			if (tileMapLayers == null || tileMapLayers.Count == 0)
				InitLayers();
			return tileMapLayers.Count;
		}
	}
	
	public tk2dRuntime.TileMap.LayerInfo[] Layers
	{
		get 
		{
			if (tileMapLayers == null || tileMapLayers.Count == 0)
				InitLayers();
			return tileMapLayers.ToArray();
		}
	}
	
	void InitLayers()
	{
		tileMapLayers = new List<tk2dRuntime.TileMap.LayerInfo>();
		var newLayer = new tk2dRuntime.TileMap.LayerInfo();
		newLayer = new tk2dRuntime.TileMap.LayerInfo();
		newLayer.name = "Layer 0";
		newLayer.hash = 0x70d32b98;
		newLayer.z = 0.0f;
		tileMapLayers.Add(newLayer);
	}
}
