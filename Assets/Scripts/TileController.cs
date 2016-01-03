﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileController : MonoBehaviour {

	public static TileController tileController { get; private set; }
	private List<Tile> TileList = new List<Tile>();
	private List<Tile> Player1 = new List<Tile>();
	private List<Tile> Player2 = new List<Tile>();
	private List<Tile> Player3 = new List<Tile>();
	private List<Tile> Player4 = new List<Tile>();
	private List<Tile> ShownTile = new List<Tile>();
	private List<Tile> HiddenTile = new List<Tile>();
	private List<Tile> Player2KnownTiles = new List<Tile>();
	private List<Tile> Player3KnownTiles = new List<Tile>();
	private List<Tile> Player4KnownTiles = new List<Tile>();
	private List<KeyValuePair<int,GameObject>> Player1TilesReference = new List<KeyValuePair<int,GameObject>>();
	private List<KeyValuePair<int,GameObject>> Player2TilesReference = new List<KeyValuePair<int,GameObject>>();
	private List<KeyValuePair<int,GameObject>> Player3TilesReference = new List<KeyValuePair<int,GameObject>>();
	private List<KeyValuePair<int,GameObject>> Player4TilesReference = new List<KeyValuePair<int,GameObject>>();
	private int distribNum;


	void Awake()
	{
		if (tileController != null && tileController != this)
		{
			Destroy(gameObject);
		}
		
		tileController = this;
		
		DontDestroyOnLoad(gameObject);
		Begin();
	}

	public void Reset()
	{
		TileList.Clear();
		Player1.Clear();
		Player2.Clear();
		Player3.Clear();
		Player4.Clear();
		ShownTile.Clear();
		HiddenTile.Clear();
		Player2KnownTiles.Clear();
		Player3KnownTiles.Clear();
		Player4KnownTiles.Clear();
		Player1TilesReference.Clear();
		Player2TilesReference.Clear();
		Player3TilesReference.Clear();
		Player4TilesReference.Clear();
		distribNum = 0;
		gameObject.GetComponent<game_UIController>().Reset();
		Begin();
	}

	void Begin()
	{
		for(int i=1; i<=7; i++)
			for(int y=7-i; y<7; y++)
				TileList.Add (new Tile(i)); 
		
		//TileList.
		distribNum = 0;
		int count = 0;
		while(TileList.Count > 0)
		{
			int randomnum = Random.Range(0, TileList.Count);
			
			if(count < 4)
				distribNum = -2;
			else if(count < 8)
				distribNum = -1;
			else if(count == 8)
				distribNum = 0;
			
			DistributeTiles(TileList[randomnum].GetTileValue());
			TileList.RemoveAt(randomnum);
			
			distribNum = (distribNum > 2) ? 0 : ++distribNum;
			
			count++;
		}
	}

	void Start()
	{
//		foreach(Tile T in Player1)
//		{
//			Debug.Log("P1 Tiles:"+T.GetTileValue());
//		}
//		foreach(Tile T in Player2)
//		{
//			Debug.Log("P2 Tiles:"+T.GetTileValue());
//		}
//		foreach(Tile T in Player3)
//		{
//			Debug.Log("P3 Tiles:"+T.GetTileValue());
//		}
//		foreach(Tile T in Player4)
//		{
//			Debug.Log("P4 Tiles:"+T.GetTileValue());
//		}
//		foreach(Tile T in HiddenTile)
//		{
//			Debug.Log("Hidden Tiles:"+T.GetTileValue());
//		}
//		foreach(Tile T in ShownTile)
//		{
//			Debug.Log("Shown Tiles:"+T.GetTileValue());
//		}
//		foreach(Tile T in Player2KnownTiles)
//		{
//			Debug.Log("P2 Known:"+T.GetTileValue());
//		}
	}

	void DistributeTiles(int value)
	{
		InstantiateTiles(value, distribNum);
		switch(distribNum)
		{
		case -2:
			HiddenTile.Add(new Tile(value));
			break;
		case -1:
			ShownTile.Add(new Tile(value));
			AddKnownTiles(value);
			break;
		case 0:
			Player1.Add(new Tile(value));
			AddKnownTiles(value);
			break;
		case 1:
			Player2.Add(new Tile(value));
			AddKnownTiles(value, distribNum);
			break;
		case 2:
			Player3.Add(new Tile(value));
			AddKnownTiles(value, distribNum);
			break;
		case 3:
			Player4.Add(new Tile(value));
			AddKnownTiles(value, distribNum);
			break;
		}
	}

	void AddKnownTiles(int value, int id = 0)
	{
		if(id == 0)
		{
			Player2KnownTiles.Add(new Tile(value));
			Player3KnownTiles.Add(new Tile(value));
			Player4KnownTiles.Add(new Tile(value));
		}
		else if(id == 1)
		{
			Player3KnownTiles.Add(new Tile(value));
			Player4KnownTiles.Add(new Tile(value));
		}
		else if(id == 2)
		{
			Player2KnownTiles.Add(new Tile(value));
			Player4KnownTiles.Add(new Tile(value));
		}
		else if(id == 3)
		{
			Player2KnownTiles.Add(new Tile(value));
			Player3KnownTiles.Add(new Tile(value));
		}
		else if(id == 4)
		{
			Player2KnownTiles.Add(new Tile(value));
		}
		else if(id == 5)
		{
			Player3KnownTiles.Add(new Tile(value));
		}
		else if(id == 6)
		{
			Player4KnownTiles.Add(new Tile(value));
		}
	}

	public List<Tile> GetList(int value = 0)
	{
		switch(value)
		{
		case 0:
			return ShownTile;
			break;
		case 1:
			return Player1;
			break;
		case 2:
			return Player2;
			break;
		case 3:
			return Player3;
			break;
		case 4:
			return Player4;
			break;
		case 5:
			return Player2KnownTiles;
			break;
		case 6:
			return Player3KnownTiles;
			break;
		case 7:
			return Player4KnownTiles;
			break;
		case 8:
			return HiddenTile;
			break;
		default:
			return null;
			break;
		}
	}

	public void SetList(List<Tile> TileList_new, int value = -1)
	{
		switch(value)
		{
		case 0:
			ShownTile = TileList_new;
			break;
		case 1:
			Player1 = TileList_new;
			break;
		case 2:
			Player2 = TileList_new;
			break;
		case 3:
			Player3 = TileList_new;
			break;
		case 4:
			Player4 = TileList_new;
			break;
		case 5:
			Player2KnownTiles = TileList_new;
			break;
		case 6:
			Player3KnownTiles = TileList_new;
			break;
		case 7:
			Player4KnownTiles = TileList_new;
			break;
		default:

			break;
		}
	}

	public bool CheckList(int value, int List)
	{
		List++;
		List<Tile> TempList = GetList(List);
		//Debug.Log(GetList(List).Count);
		int counter = 0;
		foreach(Tile T in TempList)
		{
			if(T.GetTileValue() == value)
			{
				TempList.RemoveAt(counter);
				SetList(TempList, List);
				ShownTile.Add(T);
				return true;
			}
			counter++;
		}
		return false;
	}
	#region SendResponse
	public void SendResponse(int value)
	{
		if(CheckList(value, 0))
		{
			InstantiateTiles(value, -1);
			ReduceTile(GetTileReference(value, 0));
			//Debug.Log("Correct");
			gameObject.GetComponent<game_UIController>().DetermineResult(true);
		}
		else
		{
			//Debug.Log("Wrong");
			gameObject.GetComponent<game_UIController>().DetermineResult(false);
		}
	}

	public bool SendResponse(int value, int ai)
	{
		if(CheckList(value, ai))
		{
			InstantiateTiles(value, -1);
			ReduceTile(GetTileReference(value, ai));
			AddKnownTiles(value, ai + 3);
			return true;
		}
		else
		{
			return false;
		}
	}

	public void SendResponse(int value, int ai, int cheat)
	{
		if(CheckList(value, ai))
		{
			InstantiateTiles(value, -1);
			ReduceTile(GetTileReference(value, ai));
			AddKnownTiles(value, ai + 3);
		}
	}
	#endregion

	void InstantiateTiles(int value, int type)
	{
		GameObject target;
		if(type == -2 || type == 0)
		{
			target = Instantiate((GameObject)Resources.Load("Prefabs/GameAssets/TileHidden"));
		}
		else
		{
			target = Instantiate((GameObject)Resources.Load("Prefabs/GameAssets/Tile"));
			target.GetComponentInChildren<game_TileAsset>().LoadAsset(value);
		}

		if(type >= 0 && type <=3)
		{
			SetTileReference(target, value, type);
			//AllPlayerTiles[type] = target;
		}
		target.transform.SetParent(Getholder(value, type), false);
	}

	Transform Getholder(int value, int type)
	{
		switch(type)
		{
		case -2:
			return gameObject.GetComponent<game_UIController>().GetTileHolder(0, 2).transform;
			break;
		case -1:
			return gameObject.GetComponent<game_UIController>().GetTileHolder(value - 1, 0).transform;
			break;
		default:
			return gameObject.GetComponent<game_UIController>().GetTileHolder(type, 1).transform;
			break;
		}
	}

	void ReduceTile(GameObject target)
	{
		Destroy(target);
	}

	public int GetTile(List<Tile> TileList_new)
	{
		int TileValue= 0, counter = 0, 
		randomN = Random.Range(0, TileList_new.Count - 1);
		foreach(Tile T in TileList_new)
		{
			if(randomN ==counter)
				return T.GetTileValue();
			counter++;
		}
		return 0;
	}

	void SetTileReference(GameObject target, int value, int type)
	{
		switch(type)
		{
		case 0:
			Player1TilesReference.Add(new KeyValuePair<int,GameObject>(value, target));
			break;
		case 1:
			//Player2KnownTiles.Add(new Tile(value));
			Player2TilesReference.Add(new KeyValuePair<int,GameObject>(value, target));
			break;
		case 2:
			//Player3KnownTiles.Add(new Tile(value));
			Player3TilesReference.Add(new KeyValuePair<int,GameObject>(value, target));
			break;
		case 3:
			//Player4KnownTiles.Add(new Tile(value));
			Player4TilesReference.Add(new KeyValuePair<int,GameObject>(value, target));
			break;
		}
	}

	GameObject GetTileReference(int value, int type)
	{
		List<KeyValuePair<int,GameObject>> TempList = new List<KeyValuePair<int,GameObject>>();
		GameObject Temp_gmobj = null;
		if(type == 0)
			TempList = Player1TilesReference;
		else if(type == 1)
			TempList = Player2TilesReference;
		else if(type == 2)
			TempList = Player3TilesReference;
		else if(type == 3)
			TempList = Player4TilesReference;

		foreach(KeyValuePair<int,GameObject> tile in TempList)
		{
			if(tile.Key == value)
			{
				Temp_gmobj = tile.Value;
			}
		}
		TempList.Remove(new KeyValuePair<int,GameObject>(value, Temp_gmobj));

		if(type == 0)
			Player1TilesReference = TempList;
		else if(type == 1)
			Player2TilesReference = TempList;
		else if(type == 2)
			Player3TilesReference = TempList;
		else if(type == 3)
			Player4TilesReference = TempList;

		return Temp_gmobj;
	}
}
