using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashboard : MonoBehaviour 
{
	public static Dashboard instance;
	public GameObject ObjectForCoin;

	void Awake()
	{
		instance = this;
	}
}
