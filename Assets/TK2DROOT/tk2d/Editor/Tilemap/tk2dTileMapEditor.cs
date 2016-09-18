using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public interface ITileMapEditorHost
{
	void BuildIncremental();
	void Build(bool force);
}

[CustomEditor(typeof(tk2dTileMap))]
public class tk2dTileMapEditor : Editor, ITileMapEditorHost
{
	tk2dTileMap tileMap { get { return (tk2dTileMap)target; } }
	tk2dTileMapEditorData editorData;

	tk2dTileMapSceneGUI sceneGUI;
	tk2dEditor.BrushRenderer brushRenderer;	
	tk2dEditor.BrushBuilder guiBrushBuilder;
	
	int width, height;
	int partitionSizeX, partitionSizeY;
	int buildKey;

	// Sprite collection accessor, cleanup when changed
	tk2dSpriteCollectionData _spriteCollection = null;
	tk2dSpriteCollectionData SpriteCollection
	{
		get
		{
			if (_spriteCollection == null)
			{
				_spriteCollection = tileMap.spriteCollection;
			}
			
			if (_spriteCollection != tileMap.spriteCollection)
			{
				_spriteCollection = tileMap.spriteCollection;
			}
			
			return _spriteCollection;
		}
	}
	
	
	void OnEnable()
	{
		if (Application.isPlaying || !tileMap.AllowEdit)
			return;
		
		LoadTileMapData();
		guiBrushBuilder = new tk2dEditor.BrushBuilder();
	}
	
	void InitEditor()
	{
		// Initialize editor
		LoadTileMapData();
		guiBrushBuilder = new tk2dEditor.BrushBuilder();		
	}
	
	void OnDisable()
	{
		if (brushRenderer != null)
		{
			brushRenderer.Destroy();
			brushRenderer = null;
		}
		
		if (sceneGUI != null)
		{
			sceneGUI.Destroy();
			sceneGUI = null;
		}
		
		if (editorData)
		{
			EditorUtility.SetDirty(editorData);
		}
		
		if (tileMap && tileMap.data)
		{
			EditorUtility.SetDirty(tileMap.data);
		}
	}
	
	void LoadTileMapData()
	{
		buildKey = tileMap.buildKey;
		
		string editorDataPath = AssetDatabase.GUIDToAssetPath(tileMap.editorDataGUID);
		editorData = Resources.LoadAssetAtPath(editorDataPath, typeof(tk2dTileMapEditorData)) as tk2dTileMapEditorData;
		
		width = tileMap.width;
		height = tileMap.height;
		partitionSizeX = tileMap.partitionSizeX;
		partitionSizeY = tileMap.partitionSizeY;
		
		if (tileMap.data && editorData)
		{
			// Rebuild the palette
			editorData.CreateDefaultPalette(tileMap.spriteCollection, editorData.paletteBrush, editorData.paletteTilesPerRow);
		}
		
		// Rebuild the render utility
		if (sceneGUI != null)
		{
			sceneGUI.Destroy();
		}
		sceneGUI = new tk2dTileMapSceneGUI(this, tileMap, editorData);
		
		// Rebuild the brush renderer
		if (brushRenderer != null)
		{
			brushRenderer.Destroy();
		}
		brushRenderer = new tk2dEditor.BrushRenderer(tileMap.spriteCollection);
	}
	
	public void Build(bool force, bool incremental)
	{
		if (force || buildKey != tileMap.buildKey)
		{
			if (buildKey != tileMap.buildKey)
				tk2dRuntime.TileMap.BuilderUtil.CleanRenderData(tileMap);
			
			tk2dTileMap.BuildFlags buildFlags = tk2dTileMap.BuildFlags.EditMode;
			if (!incremental) buildFlags |= tk2dTileMap.BuildFlags.ForceBuild;
			tileMap.Build(buildFlags);
			
			buildKey = tileMap.buildKey;
		}
	}
	
	public void Build(bool force) { Build(force, false); }
	public void BuildIncremental() { Build(true, true); }
	
	bool Ready
	{
		get
		{
			return (tileMap != null && tileMap.data != null && editorData != null & tileMap.spriteCollection != null);
		}
	}
	
	void HighlightTile(Rect rect, Rect tileSize, int tilesPerRow, int x, int y, Color fillColor, Color outlineColor)
	{
		Rect highlightRect = new Rect(rect.x + x * tileSize.width, 
									  rect.y + y * tileSize.height, 
									  tileSize.width, 
									  tileSize.height);
		Vector3[] rectVerts = { new Vector3(highlightRect.x, highlightRect.y, 0), 
								new Vector3(highlightRect.x + highlightRect.width, highlightRect.y, 0), 
								new Vector3(highlightRect.x + highlightRect.width, highlightRect.y + highlightRect.height, 0), 
								new Vector3(highlightRect.x, highlightRect.y + highlightRect.height, 0) };
		Handles.DrawSolidRectangleWithOutline(rectVerts, fillColor, outlineColor);
	}
	
	int selectedDataTile = -1;
	void DrawTileDataSetupPanel()
	{
		// Sanitize prefabs
		if (tileMap.data.tilePrefabs == null)
			tileMap.data.tilePrefabs = new Object[0];
		
		if (tileMap.data.tilePrefabs.Length != SpriteCollection.Count)
		{
			System.Array.Resize(ref tileMap.data.tilePrefabs, SpriteCollection.Count);
		}
		
		Rect rect = brushRenderer.DrawBrush(tileMap, editorData.paletteBrush, editorData.brushDisplayScale, true, editorData.paletteTilesPerRow);
		float displayScale = brushRenderer.LastScale;
		Rect tileSize = new Rect(0, 0, brushRenderer.TileSizePixels.width * displayScale, brushRenderer.TileSizePixels.height * displayScale);
		int tilesPerRow = editorData.paletteTilesPerRow;
		int newSelectedPrefab = selectedDataTile;
		
		if (Event.current.type == EventType.MouseUp && rect.Contains(Event.current.mousePosition))
		{
			Vector2 localClickPosition = Event.current.mousePosition - new Vector2(rect.x, rect.y);
			Vector2 tileLocalPosition = new Vector2(localClickPosition.x / tileSize.width, localClickPosition.y / tileSize.height);
			int tx = (int)tileLocalPosition.x;
			int ty = (int)tileLocalPosition.y;
			newSelectedPrefab = ty * tilesPerRow + tx;
		}
		
		if (Event.current.type == EventType.Repaint)
		{
			for (int tileId = 0; tileId < SpriteCollection.Count; ++tileId)
			{
				Color noDataFillColor = new Color(0, 0, 0, 0.2f);
				Color noDataOutlineColor = Color.clear;
				Color selectedFillColor = new Color(1,0,0,0.05f);
				Color selectedOutlineColor = Color.red;
				
				if (tileMap.data.tilePrefabs[tileId] == null || tileId == selectedDataTile)
				{
					Color fillColor = (selectedDataTile == tileId)?selectedFillColor:noDataFillColor;
					Color outlineColor = (selectedDataTile == tileId)?selectedOutlineColor:noDataOutlineColor;
					HighlightTile(rect, tileSize, editorData.paletteTilesPerRow, tileId % tilesPerRow, tileId / tilesPerRow, fillColor, outlineColor);
				}
			}
		}
		
		if (selectedDataTile >= 0 && selectedDataTile < tileMap.data.tilePrefabs.Length)
		{
			tileMap.data.tilePrefabs[selectedDataTile] = EditorGUILayout.ObjectField("Prefab", tileMap.data.tilePrefabs[selectedDataTile], typeof(Object), false);
		}
		
		if (newSelectedPrefab != selectedDataTile)		
		{
			selectedDataTile = newSelectedPrefab;
			Repaint();
		}
	}
	
	void DrawLayersPanel(bool allowEditing)
	{
		GUILayout.BeginVertical();
		
		// constrain selected layer
		editorData.layer = Mathf.Clamp(editorData.layer, 0, tileMap.data.NumLayers - 1);
		
		if (allowEditing)
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Add Layer", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
			{
				editorData.layer = tk2dEditor.TileMap.TileMapUtility.AddNewLayer(tileMap);
			}
			GUILayout.EndHorizontal();
		}
		
		GUILayout.BeginVertical();
		
		int numLayers = tileMap.data.NumLayers;
		int deleteLayer = -1;
		int moveUp = -1;
		int moveDown = -1;
		for (int layer = numLayers - 1; layer >= 0; --layer)
		{
			GUILayout.BeginHorizontal(tk2dEditorSkin.SC_ListBoxItem);
			if (allowEditing && editorData.layer == layer)
			{
				string newName = GUILayout.TextField(tileMap.data.Layers[layer].name, EditorStyles.label, GUILayout.ExpandWidth(true));
				tileMap.data.Layers[layer].name = newName;
			}
			else
			{
				if (allowEditing)
				{
					if (GUILayout.Button(tileMap.data.Layers[layer].name, EditorStyles.label, GUILayout.ExpandWidth(true)))
					{
						editorData.layer = layer;
						Repaint();
					}
				}
				else
				{
					bool layerSelVal = editorData.layer == layer;
					bool newLayerSelVal = GUILayout.Toggle(layerSelVal, tileMap.data.Layers[layer].name,  EditorStyles.toggle, GUILayout.ExpandWidth(true));
					if (newLayerSelVal != layerSelVal)
					{
						editorData.layer = layer;
						Repaint();
					}
				}
			}
			
			if (allowEditing)
			{
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical();
				
				GUILayout.BeginHorizontal();
				
				GUILayout.FlexibleSpace();
				
				tk2dGuiUtility.BeginChangeCheck();
				tileMap.data.Layers[layer].useColor = GUILayout.Toggle(tileMap.data.Layers[layer].useColor, "Color", EditorStyles.miniButton, GUILayout.ExpandWidth(false));
				if (tk2dGuiUtility.EndChangeCheck())
					Build(true);
				
				tileMap.data.Layers[layer].generateCollider = GUILayout.Toggle(tileMap.data.Layers[layer].generateCollider, "Collider", EditorStyles.miniButton, GUILayout.ExpandWidth(false));
				
				if (allowEditing && tileMap.data.NumLayers != 1)
				{
					if (layer != 0 && GUILayout.Button("Down", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
					{
						moveUp = layer;
						Repaint();
					}
					
					if (layer != numLayers-1 && GUILayout.Button("Up", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
					{
						moveDown = layer;
						Repaint();
					}
					
					if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
					{
						deleteLayer = layer;
						Repaint();
					}
				}
				
				GUILayout.EndHorizontal();
				

				
				GUILayout.BeginHorizontal();
				
				GUILayout.FlexibleSpace();
				tk2dGuiUtility.BeginChangeCheck();
				if (layer == 0)
				{
					EditorGUILayout.FloatField(0.0f, GUILayout.Width(50));
				}
				else
				{
					tileMap.data.Layers[layer].z = EditorGUILayout.FloatField(tileMap.data.Layers[layer].z, GUILayout.Width(50));
					tileMap.data.Layers[layer].z = Mathf.Max(0, tileMap.data.Layers[layer].z);
				}
				if (tk2dGuiUtility.EndChangeCheck())
					Build(true);
				
				tk2dGuiUtility.BeginChangeCheck();
				tileMap.data.Layers[layer].unityLayer = EditorGUILayout.LayerField(tileMap.data.Layers[layer].unityLayer, GUILayout.Width(90.0f));
				if (tk2dGuiUtility.EndChangeCheck())
					Build(true);
				
				GUILayout.EndHorizontal();
				
				
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
		}
		
		if (deleteLayer != -1)
		{
			Undo.RegisterUndo(new Object[] { tileMap, tileMap.data }, "Deleted layer");
			tk2dEditor.TileMap.TileMapUtility.DeleteLayer(tileMap, deleteLayer);
		}
		
		if (moveUp != -1)
		{
			Undo.RegisterUndo(new Object[] { tileMap, tileMap.data }, "Moved layer");
			tk2dEditor.TileMap.TileMapUtility.MoveLayer(tileMap, moveUp, -1);
		}
		
		if (moveDown != -1)
		{
			Undo.RegisterUndo(new Object[] { tileMap, tileMap.data }, "Moved layer");
			tk2dEditor.TileMap.TileMapUtility.MoveLayer(tileMap, moveDown, 1);
		}
		
		GUILayout.EndVertical();
		
		GUILayout.EndVertical();
	}
	
	void DrawSettingsPanel()
	{
		// Tilemap data
		tk2dTileMapData newData = (tk2dTileMapData)EditorGUILayout.ObjectField("Tile Map Data", tileMap.data, typeof(tk2dTileMapData), false);
		if (newData != tileMap.data)
		{
			tileMap.data = newData;
			LoadTileMapData();
		}
		if (tileMap.data == null)
		{
			if (tk2dGuiUtility.InfoBoxWithButtons(
				"TileMap needs an data object to proceed. " +
				"Please create one or drag an existing data object into the inspector slot.\n",
				tk2dGuiUtility.WarningLevel.Info, 
				"Create") != -1)
			{
				string assetPath = EditorUtility.SaveFilePanelInProject("Save Tile Map Data", "tileMapData", "asset", "");
				if (assetPath.Length > 0)
				{
					tk2dTileMapData tileMapData = ScriptableObject.CreateInstance<tk2dTileMapData>();
					AssetDatabase.CreateAsset(tileMapData, assetPath);
					tileMap.data = tileMapData;
					EditorUtility.SetDirty(tileMap);
					
					Init(tileMapData);
					LoadTileMapData();
				}
			}
		}
		
		// Editor data
		tk2dTileMapEditorData newEditorData = (tk2dTileMapEditorData)EditorGUILayout.ObjectField("Editor Data", editorData, typeof(tk2dTileMapEditorData), false);
		if (newEditorData != editorData)
		{
			string assetPath = AssetDatabase.GetAssetPath(newEditorData);
			if (assetPath.Length > 0)
			{
				tileMap.editorDataGUID = AssetDatabase.AssetPathToGUID(assetPath);
				EditorUtility.SetDirty(tileMap);
				LoadTileMapData();
			}
		}
		if (editorData == null)
		{
			if (tk2dGuiUtility.InfoBoxWithButtons(
				"TileMap needs an editor data object to proceed. " +
				"Please create one or drag an existing data object into the inspector slot.\n",
				tk2dGuiUtility.WarningLevel.Info, 
				"Create") != -1)
			{
				string assetPath = EditorUtility.SaveFilePanelInProject("Save Tile Map Editor Data", "tileMapEditorData", "asset", "");
				if (assetPath.Length > 0)
				{
					tk2dTileMapEditorData tileMapEditorData = ScriptableObject.CreateInstance<tk2dTileMapEditorData>();
					AssetDatabase.CreateAsset(tileMapEditorData, assetPath);
					tileMap.editorDataGUID = AssetDatabase.AssetPathToGUID(assetPath);
					EditorUtility.SetDirty(tileMap);
					LoadTileMapData();
				}
			}
		}
		
		// Sprite collection
		Object selectedSpriteCollectionObject = EditorGUILayout.ObjectField("Sprite Collection", tileMap.spriteCollection, typeof(Object), false) as Object;
		if (selectedSpriteCollectionObject != tileMap.spriteCollection)
		{
			string assetPath = AssetDatabase.GetAssetPath(selectedSpriteCollectionObject);
			string guid = AssetDatabase.AssetPathToGUID(assetPath);
			tk2dIndex index = tk2dEditorUtility.GetOrCreateIndex();
			
			foreach (var scIndex in index.GetSpriteCollectionIndex())
			{
				if (scIndex.spriteCollectionGUID == guid ||
					scIndex.spriteCollectionDataGUID == guid)
				{
					string realDataPath = AssetDatabase.GUIDToAssetPath(scIndex.spriteCollectionDataGUID);
					tk2dSpriteCollectionData data = AssetDatabase.LoadAssetAtPath(realDataPath, typeof(tk2dSpriteCollectionData)) as tk2dSpriteCollectionData;
					
					if (data.allowMultipleAtlases)
					{
						Debug.Log("Atlas spanning not allowed");
					}
					else
					{
						tileMap.spriteCollection = data;
						data.InitMaterialIds();
						LoadTileMapData();
						
						EditorUtility.SetDirty(tileMap);
						
						if (Ready)
						{
							Init(tileMap.data);
							tileMap.BeginEditMode();
						}
						break;						
					}
				}
			}
		}
		
		// If not set up, don't bother drawing anything else
		if (!Ready)
			return;
		
		tileMap.renderData = EditorGUILayout.ObjectField("Render Data", tileMap.renderData, typeof(GameObject), false) as GameObject;
		GUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(" ");
		if (GUILayout.Button("Make Unique"))
		{
			tk2dRuntime.TileMap.BuilderUtil.CleanRenderData(tileMap);
			tileMap.renderData = null;
		}
		GUILayout.EndHorizontal();
		
		// tile map size
		editorData.showDimensions = EditorGUILayout.Foldout(editorData.showDimensions, "Dimensions");
		if (editorData.showDimensions)
		{
			EditorGUI.indentLevel++;
			
			width = Mathf.Clamp(EditorGUILayout.IntField("Width", width), 1, 512);
			height = Mathf.Clamp(EditorGUILayout.IntField("Height", height), 1, 512);
			partitionSizeX = Mathf.Clamp(EditorGUILayout.IntField("PartitionSizeX", partitionSizeX), 4, 32);
			partitionSizeY = Mathf.Clamp(EditorGUILayout.IntField("PartitionSizeY", partitionSizeY), 4, 32);
			
			// Create a default tilemap with given dimensions
			if (!tileMap.AreSpritesInitialized())
			{
				tk2dRuntime.TileMap.BuilderUtil.InitDataStore(tileMap);
				tk2dEditor.TileMap.TileMapUtility.ResizeTileMap(tileMap, width, height, tileMap.partitionSizeX, tileMap.partitionSizeY);	
			}
			
			if (width != tileMap.width || height != tileMap.height || partitionSizeX != tileMap.partitionSizeX || partitionSizeY != tileMap.partitionSizeY)
			{
				if ((width < tileMap.width || height < tileMap.height))
				{
					tk2dGuiUtility.InfoBox("The new size of the tile map is smaller than the current size." +
						"Some clipping will occur.", tk2dGuiUtility.WarningLevel.Warning);
				}
				
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Apply", EditorStyles.miniButton))
				{
					tk2dEditor.TileMap.TileMapUtility.ResizeTileMap(tileMap, width, height, partitionSizeX, partitionSizeY);
				}
				GUILayout.EndHorizontal();
			}

			EditorGUI.indentLevel--;
		}
		
		editorData.showLayers = EditorGUILayout.Foldout(editorData.showLayers, "Layers");
		if (editorData.showLayers)
		{
			EditorGUI.indentLevel++;
			
			DrawLayersPanel(true);
			
			EditorGUI.indentLevel--;
		}
		
		// tilemap info
		editorData.showInfo = EditorGUILayout.Foldout(editorData.showInfo, "Info");
		if (editorData.showInfo)
		{
			EditorGUI.indentLevel++;
			
			int numActiveChunks = 0;
			if (tileMap.Layers != null)
			{
				foreach (var layer in tileMap.Layers)
					numActiveChunks += layer.NumActiveChunks;
			}
			EditorGUILayout.LabelField("Active chunks", numActiveChunks.ToString());
			int partitionMemSize = partitionSizeX * partitionSizeY * 4;
			EditorGUILayout.LabelField("Memory", ((numActiveChunks * partitionMemSize) / 1024).ToString() + "kB" );
			
			int numActiveColorChunks = 0;
			if (tileMap.ColorChannel != null)
				numActiveColorChunks += tileMap.ColorChannel.NumActiveChunks;
			EditorGUILayout.LabelField("Active color chunks", numActiveColorChunks.ToString());
			int colorMemSize = (partitionSizeX + 1) * (partitionSizeY + 1) * 4;
			EditorGUILayout.LabelField("Memory", ((numActiveColorChunks * colorMemSize) / 1024).ToString() + "kB" );
			
			EditorGUI.indentLevel--;
		}
		
		// tile properties
		editorData.showTileProperties = EditorGUILayout.Foldout(editorData.showTileProperties, "Tile Properties");
		if (editorData.showTileProperties)
		{
			EditorGUI.indentLevel++;

			// sort method
			tk2dGuiUtility.BeginChangeCheck();
			tileMap.data.sortMethod = (tk2dTileMapData.SortMethod)EditorGUILayout.EnumPopup("Sort Method", tileMap.data.sortMethod);
			if (tk2dGuiUtility.EndChangeCheck())
			{
				tileMap.BeginEditMode();
			}
			

			// reset sizes			
			GUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Reset sizes");
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Reset", EditorStyles.miniButtonRight))
			{
				Init(tileMap.data);
				Build(true);
			}
			GUILayout.EndHorizontal();

			// convert these to pixel units
			Vector3 texelSize = SpriteCollection.spriteDefinitions[0].texelSize;
			Vector3 tileOriginPixels = new Vector3(tileMap.data.tileOrigin.x / texelSize.x, tileMap.data.tileOrigin.y / texelSize.y, tileMap.data.tileOrigin.z);
			Vector3 tileSizePixels = new Vector3(tileMap.data.tileSize.x / texelSize.x, tileMap.data.tileSize.y / texelSize.y, tileMap.data.tileSize.z);
			
			Vector3 newTileOriginPixels = EditorGUILayout.Vector3Field("Cursor Origin", tileOriginPixels);
			Vector3 newTileSizePixels = EditorGUILayout.Vector3Field("Tile Size", tileSizePixels);
			
			if (newTileOriginPixels != tileOriginPixels ||
				newTileSizePixels != tileSizePixels)
			{
				tileMap.data.tileOrigin = new Vector3(newTileOriginPixels.x * texelSize.x, newTileOriginPixels.y * texelSize.y, newTileOriginPixels.z);
				tileMap.data.tileSize = new Vector3(newTileSizePixels.x * texelSize.x, newTileSizePixels.y * texelSize.y, newTileSizePixels.z);
				Build(true);
			}
			EditorGUI.indentLevel--;
		}
		
		editorData.showPaletteProperties = EditorGUILayout.Foldout(editorData.showPaletteProperties, "Palette Properties");
		if (editorData.showPaletteProperties)
		{
			EditorGUI.indentLevel++;
			int newTilesPerRow = Mathf.Clamp(EditorGUILayout.IntField("Tiles Per Row", editorData.paletteTilesPerRow),
											1, SpriteCollection.Count);
			if (newTilesPerRow != editorData.paletteTilesPerRow)
			{
				guiBrushBuilder.Reset();
				
				editorData.paletteTilesPerRow = newTilesPerRow;
				editorData.CreateDefaultPalette(tileMap.spriteCollection, editorData.paletteBrush, editorData.paletteTilesPerRow);
			}
			
			GUILayout.BeginHorizontal();
			editorData.brushDisplayScale = EditorGUILayout.FloatField("Display Scale", editorData.brushDisplayScale);
			editorData.brushDisplayScale = Mathf.Clamp(editorData.brushDisplayScale, 1.0f / 16.0f, 4.0f);
			if (GUILayout.Button("Reset", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false)))
			{
				editorData.brushDisplayScale = 1.0f;
				Repaint();
			}
			GUILayout.EndHorizontal();
			
			EditorGUILayout.PrefixLabel("Preview");
			brushRenderer.DrawBrush(tileMap, editorData.paletteBrush, editorData.brushDisplayScale, true, editorData.paletteTilesPerRow);
			EditorGUI.indentLevel--;
		}
		
//		editorData.showAdvancedProperties = EditorGUILayout.Foldout(editorData.showAdvancedProperties, "Advanced");
//		if (editorData.showAdvancedProperties)
//		{
//		}
	}
	
	void DrawColorPaintPanel()
	{
		if (!tileMap.HasColorChannel())
		{
			if (GUILayout.Button("Create Color Channel"))
			{
				Undo.RegisterUndo(tileMap, "Created Color Channel");
				tileMap.CreateColorChannel();
				tileMap.BeginEditMode();
			}
			
			Repaint();
			return;
		}
		
		editorData.brushColor = EditorGUILayout.ColorField("Color", editorData.brushColor);
		editorData.blendMode = (tk2dTileMapEditorData.BlendMode)EditorGUILayout.EnumPopup("Blend Mode", editorData.blendMode);
		editorData.brushRadius = EditorGUILayout.Slider("Radius", editorData.brushRadius, 1.0f, 64.0f);
		editorData.blendStrength = EditorGUILayout.Slider("Strength", editorData.blendStrength, 0.0f, 1.0f);

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Clear to Color");
		if (GUILayout.Button("Clear", GUILayout.ExpandWidth(false)))
		{
			tileMap.ColorChannel.Clear(editorData.brushColor);
			Build(true);
		}
		EditorGUILayout.EndHorizontal();
		
		if (tileMap.HasColorChannel())
		{
			EditorGUILayout.Separator();
			if (GUILayout.Button("Delete Color Channel"))
			{
				Undo.RegisterUndo(tileMap, "Deleted Color Channel");
				
				tileMap.DeleteColorChannel();
				tileMap.BeginEditMode();

				Repaint();
				return;
			}
		}
	}

	int InlineToolbar(string name, int val, string[] names)
	{
		int selectedIndex = val;
		GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
		GUILayout.Label(name, EditorStyles.toolbarButton);
		GUILayout.FlexibleSpace();
		for (int i = 0; i < names.Length; ++i)
		{
			bool selected = (i == selectedIndex);
			bool toggled = GUILayout.Toggle(selected, names[i], EditorStyles.toolbarButton);
			if (toggled == true)
			{
				selectedIndex = i;
			}
		}
		
		GUILayout.EndHorizontal();
		return selectedIndex;
	}
	
	void DrawPaintPanel()
	{
		var activeBrush = editorData.activeBrush;
		
		if (Ready && (activeBrush == null || activeBrush.Empty))
		{
			editorData.InitBrushes(tileMap.spriteCollection);
		}
		
		// Draw layer selector
		if (tileMap.data.NumLayers > 1)
		{
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
			GUILayout.Label("Layers", EditorStyles.toolbarButton);	GUILayout.FlexibleSpace();	
			GUILayout.EndHorizontal();
			DrawLayersPanel(false);
			EditorGUILayout.Space();
			GUILayout.EndVertical();
		}
		
		// Brush properties
		string[] toolBarButtonNames = System.Enum.GetNames(typeof(tk2dTileMapEditorBrush.PaintMode));
		editorData.activeBrush.paintMode = (tk2dTileMapEditorBrush.PaintMode)InlineToolbar("Paint mode", (int)editorData.activeBrush.paintMode, toolBarButtonNames);
		
		// Draw palette
		editorData.showPalette = EditorGUILayout.Foldout(editorData.showPalette, "Palette");
		if (editorData.showPalette)
		{
			Rect rect = brushRenderer.DrawBrush(tileMap, editorData.paletteBrush, editorData.brushDisplayScale, true, editorData.paletteTilesPerRow);
			float displayScale = brushRenderer.LastScale;
			
			Rect tileSize = new Rect(0, 0, brushRenderer.TileSizePixels.width * displayScale, brushRenderer.TileSizePixels.height * displayScale);
			guiBrushBuilder.HandleGUI(rect, tileSize, editorData.paletteTilesPerRow, tileMap.spriteCollection, activeBrush);
			EditorGUILayout.Separator();
		}
		EditorGUILayout.Separator();

		// Draw brush
		editorData.showBrush = EditorGUILayout.Foldout(editorData.showBrush, "Brush");
		if (editorData.showBrush)
		{
			brushRenderer.DrawBrush(tileMap, editorData.activeBrush, editorData.brushDisplayScale, false, editorData.paletteTilesPerRow);
			EditorGUILayout.Separator();
		}
		
	}
	
	/// <summary>
	/// Initialize tilemap data to sensible values.
	/// Mainly, tileSize and tileOffset
	/// </summary>
	void Init(tk2dTileMapData tileMapData)
	{
		tileMapData.tileSize = tileMap.spriteCollection.spriteDefinitions[0].untrimmedBoundsData[1];
		tileMapData.tileOrigin = this.tileMap.spriteCollection.spriteDefinitions[0].untrimmedBoundsData[0] - tileMap.spriteCollection.spriteDefinitions[0].untrimmedBoundsData[1] * 0.5f;
	}
	
	public override void OnInspectorGUI()
	{
		if (Application.isPlaying)
		{
			tk2dGuiUtility.InfoBox("Editor disabled while game is running.", tk2dGuiUtility.WarningLevel.Error);
			return;
		}
		
		if (!tileMap.AllowEdit)
		{
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Edit"))
			{
				tileMap.BeginEditMode();
				InitEditor();
				Repaint();
			}
			if (GUILayout.Button("All", GUILayout.ExpandWidth(false)))
			{
				tk2dTileMap[] allTileMaps = Resources.FindObjectsOfTypeAll(typeof(tk2dTileMap)) as tk2dTileMap[];
				foreach (var tm in allTileMaps)
				{
					if (!EditorUtility.IsPersistent(tm) && !tm.AllowEdit)
					{
						tm.BeginEditMode();
						EditorUtility.SetDirty(tm);
					}
				}
				InitEditor();
			}
			GUILayout.EndHorizontal();
			return;
		}
		
		// Commit
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Commit"))
		{
			tileMap.EndEditMode();
			Repaint();
		}
		if (GUILayout.Button("All", GUILayout.ExpandWidth(false)))
		{
			tk2dTileMap[] allTileMaps = Resources.FindObjectsOfTypeAll(typeof(tk2dTileMap)) as tk2dTileMap[];
			foreach (var tm in allTileMaps)
			{
				if (!EditorUtility.IsPersistent(tm) && tm.AllowEdit)
				{
					tm.EndEditMode();
					EditorUtility.SetDirty(tm);
				}
			}
		}
		GUILayout.EndHorizontal();
		EditorGUILayout.Separator();

		
		if (tileMap.editorDataGUID.Length > 0 && editorData == null)
		{
			// try to load it in
			LoadTileMapData();
			// failed, so the asset is lost
			if (editorData == null)
			{
				tileMap.editorDataGUID = "";
			}
		}
		
		if (editorData == null || tileMap.data == null || !tileMap.AreSpritesInitialized() ||
			tileMap.spriteCollection == null)
		{
			DrawSettingsPanel();
		}
		else
		{
			// In case things have changed
			if (tk2dRuntime.TileMap.BuilderUtil.InitDataStore(tileMap))
				Build(true);
			
			string[] toolBarButtonNames = System.Enum.GetNames(typeof(tk2dTileMapEditorData.EditMode));
			
			editorData.editMode = (tk2dTileMapEditorData.EditMode)GUILayout.Toolbar((int)editorData.editMode, toolBarButtonNames );
			switch (editorData.editMode)
			{
			case tk2dTileMapEditorData.EditMode.Paint: DrawPaintPanel(); break;
			case tk2dTileMapEditorData.EditMode.Color: DrawColorPaintPanel(); break;
			case tk2dTileMapEditorData.EditMode.Settings: DrawSettingsPanel(); break;
			case tk2dTileMapEditorData.EditMode.Data: DrawTileDataSetupPanel(); break;
			}
		}
	}
	
	void OnSceneGUI()
	{
		if (!Ready)
		{
			return;
		}

		if (sceneGUI != null)
		{
			sceneGUI.OnSceneGUI();
		}
		
		if (!Application.isPlaying && tileMap.AllowEdit)
		{
			// build if necessary
			if (tk2dRuntime.TileMap.BuilderUtil.InitDataStore(tileMap))
				Build(true);
			else		
				Build(false);
		}
	}
	
    [MenuItem("GameObject/Create Other/tk2d/TileMap (Beta)", false, 13850)]
	static void Create()
	{
		tk2dSpriteCollectionData sprColl = null;
		if (sprColl == null)
		{
			// try to inherit from other TileMaps in scene
			tk2dTileMap sceneTileMaps = GameObject.FindObjectOfType(typeof(tk2dTileMap)) as tk2dTileMap;
			if (sceneTileMaps)
			{
				sprColl = sceneTileMaps.spriteCollection;
			}
		}

		if (sprColl == null)
		{
			tk2dSpriteCollectionIndex[] spriteCollections = tk2dEditorUtility.GetOrCreateIndex().GetSpriteCollectionIndex();
			foreach (var v in spriteCollections)
			{
				GameObject scgo = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(v.spriteCollectionDataGUID), typeof(GameObject)) as GameObject;
				var sc = scgo.GetComponent<tk2dSpriteCollectionData>();
				if (sc != null && sc.spriteDefinitions != null && sc.spriteDefinitions.Length > 0 && sc.allowMultipleAtlases == false)
				{
					sprColl = sc;
					break;
				}
			}

			if (sprColl == null)
			{
				EditorUtility.DisplayDialog("Create TileMap", "Unable to create sprite as no SpriteCollections have been found.", "Ok");
				return;
			}
		}

		GameObject go = tk2dEditorUtility.CreateGameObjectInScene("TileMap");
		go.transform.position = Vector3.zero;
		go.transform.rotation = Quaternion.identity;
		tk2dTileMap tileMap = go.AddComponent<tk2dTileMap>();
		tileMap.spriteCollection = sprColl;
		
		tileMap.Build(tk2dTileMap.BuildFlags.ForceBuild);
	}
}
