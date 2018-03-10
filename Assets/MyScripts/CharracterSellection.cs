using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharracterSellection : MonoBehaviour
{
	public static CharracterSellection instance;
    private GameObject[] characterList;
//    public GameObject man;
//    public GameObject girl;

    public AudioSource bntZvuk;

    private int index = 0;
	void Awake()
	{
		instance = this;
	}
    void Start()
    {
//        man.SetActive(false);
//        girl.SetActive(false);
        characterList = new GameObject[transform.childCount];
//        for (int i = 0; i < 2; i++)
//            characterList[i] = transform.GetChild(i).gameObject;
//        foreach (GameObject go in characterList)
//            go.SetActive(false);
//        if (characterList[0])
//            characterList[0].SetActive(true);
    }


    public void ToogleLeft()
    {
        characterList[index].SetActive(false);
        index--;
        if (index < 0)
            index = characterList.Length - 1;
        characterList[index].SetActive(true);
//        bntZvuk.Play();s
 
    }


    public void ToogleRight()
    {
        characterList[index].SetActive(false);
        index++;
        if (index == characterList.Length)
            index = 0;
        characterList[index].SetActive(true);
//        bntZvuk.Play();
    }


    public void ConfirmSellection()
    {
//        if (index == 1)
//        {
//            man.SetActive(true);
//            girl.SetActive(false);
//        }
//        if (index == 0)
//        {
//            man.SetActive(false);
//            girl.SetActive(true);
//        }
    }
}
