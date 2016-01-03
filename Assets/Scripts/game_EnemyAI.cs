using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class game_EnemyAI : MonoBehaviour {

	public int[] number = new int[7];
	public int unknownNumber = 0;
	/// <summary>
	/// 0 = Easy AI
	/// 1 = Mid AI
	/// 2 = Hard AI
	/// 3 = Cheating AI
	/// </summary>
	[Range(0,3)]
	public int AiType;
	public int AiID;

	private List<Tile> TileHolder = new List<Tile>();
	private List<KeyValuePair<int,float>> Choice = new List<KeyValuePair<int,float>>();
	private int[] RemovedTiles = new int[7];


	// Use this for initialization
	void Start () {
		GetChoices();
	}

	public void Reset()
	{
		for(int i = 0; i < 7; i++)
		{
			RemovedTiles[i] = 0;
		}
	}

	public int GetAiID()
	{
		return AiID;
	}

	public int GetAiType()
	{
		return AiType;
	}
	
	public void GetChoices()
	{
		Choice.Clear();
		TileHolder = TileController.tileController.GetList(AiID + 4);
		unknownNumber = TileController.tileController.GetList(8).Count;
		for(int i = 0; i < 7; i++)
		{
			number[i] = 0;
			int numcount = 0;
			foreach(Tile T in TileHolder)
			{
				if(T.GetTileValue() == i + 1)
				{
					numcount++;
					number[i] = numcount;
				}
			}
			float num = (float)((float)((i + 1) - number[i])/(float)unknownNumber) * 100;
			AddChoice(i + 1, num);

			//Debug.Log("Number " + (i + 1).ToString() + " has " + num +"%");
		}
		//RemoveZeroes();
		SortChoices();
		ShowChoices();
	}

	void AddChoice(int num, float chance)
	{
		//Debug.Log("ADDED");
		Choice.Add(new KeyValuePair<int,float>(num, chance));
	}

	public void RemoveChoice(int num)
	{
		int counter = 0, index = 0;
		bool found = false;
		foreach(KeyValuePair<int,float> tile in Choice)
		{
			if(tile.Key == num)
			{
				index = counter;
				found = true;
			}
			counter++;
		}
		if(found)
		{
			Choice.RemoveAt(index);
		}
	}

	public void RemoveChoice(int num, int index)
	{
		RemovedTiles[index] = num;
	}

	public void RemoveZeroes()
	{
		int counter = 0, index = 0;
		bool found = false;
		for(int  i = 0; i < Choice.Count; i++)
		{
			counter = 0;
			foreach(KeyValuePair<int,float> tile in Choice)
			{
				if(tile.Value == 0)
				{
					index = counter;
					found = true;
				}
				counter++;
			}
			if(found)
				Choice.RemoveAt(index);
		}
	}

	void SortChoices()
	{
		Choice.Sort(delegate(KeyValuePair<int,float> firstPair, KeyValuePair<int,float> nextPair)
		{
			return nextPair.Value.CompareTo(firstPair.Value);
		}
		);
	}

	void ShowChoices()
	{
		//Debug.Log("COUNT: " + Choice.Count);
		int number;
		foreach(KeyValuePair<int,float> tile in Choice)
		{
			number = System.Convert.ToInt32(tile.Key);
			Debug.Log("Tile: " + (number) + " has " + tile.Value + "%");
		}
	}

	public int GetBestChoice()
	{
		foreach(KeyValuePair<int,float> tile in Choice)
		{
			return tile.Key;
		}
		return 0;
	}

	public int GetBestChoice(int ai)
	{
		int counter = 0, index = 0;
		foreach(KeyValuePair<int,float> tile in Choice)
		{
			for(int i = 0; i < RemovedTiles.Length; i++)
			{
				if(tile.Key == RemovedTiles[i])
					index++;	
			}
		}
		foreach(KeyValuePair<int,float> tile in Choice)
		{
			if(counter == index)
			return tile.Key;
			counter++;
		}
		return 0;
	}
}
