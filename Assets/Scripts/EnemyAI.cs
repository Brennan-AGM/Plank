using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour {

	public int[] number = new int[7];
	public int unknownNumber = 0;
	public int highestChancenum = 0;
	public int AiID = 0;

	private List<Tile> TileHolder = new List<Tile>();
	private List<KeyValuePair<int,float>> Choice = new List<KeyValuePair<int,float>>();


	// Use this for initialization
	void Start () {
		GetChoices();
	}
	
	void GetChoices()
	{
		Choice.Clear();
		TileHolder = TileController.tileController.GetList(5);
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
			AddChoice(i, num);

			//Debug.Log("Number " + (i + 1).ToString() + " has " + num +"%");
		}
		ShowChoices();
	}

	void AddChoice(int num, float chance)
	{
		Debug.Log("ADDED");
		Choice.Add(new KeyValuePair<int,float>(num, chance));
	}

	void RemoveChoice(int num)
	{
		float chance = 0.0f;
		foreach(KeyValuePair<int,float> tile in Choice)
		{
			if(tile.Key == num)
			{
				chance = tile.Value;
			}
		}
		Choice.Remove(new KeyValuePair<int,float>(num, chance));
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
		Debug.Log("COUNT: " + Choice.Count);
		int number;
		foreach(KeyValuePair<int,float> tile in Choice)
		{
			number = System.Convert.ToInt32(tile.Key);
			Debug.Log("Tile: " + (number + 1) + " has " + tile.Value + "%");
		}
	}
}
