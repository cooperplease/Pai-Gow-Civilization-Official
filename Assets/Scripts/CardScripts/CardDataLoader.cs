using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CardDataLoader
{
    public Dictionary<String, CardData> cardDatabase { get; private set; }

    public CardDataLoader(TextAsset csv, String cardArtPath)
    {
        string[] data = csv.text.Split("\r\n",'\n');
        
        cardDatabase = new Dictionary<string, CardData>();
        foreach (string line in data)
        {
            Debug.Log(line);
            String[] fields =  line.Split('\t');
            String name = fields[0];
            String keywords = fields[1];
            List<Keywords> keywordsList = new List<Keywords>();
            foreach (string keyword in keywords.Split(','))
            {
                switch (keyword)
                {
                    case "TUNNEL":
                        keywordsList.Add(Keywords.TUNNEL);
                        break;
                    case "QUICK":
                        keywordsList.Add(Keywords.QUICK);
                        break;
                    case "POWERTHROUGH":
                        keywordsList.Add(Keywords.POWERTHROUGH);
                        break;
                    default:
                        Debug.Log(keyword + " failed to find keyword from csv");
                        break;
                }
            }
            int power = int.Parse(fields[2]);
            int toughness = int.Parse(fields[3]);
            String description = fields[4];
            String flavorText = fields[5];
            Sprite sprite = Resources.Load<Sprite>((cardArtPath+'/'+fields[6]));
            Debug.Log("path: "+ cardArtPath+'/'+fields[6]);
            Debug.Log(sprite);
            //todo: add abilities loading
            CardData newCardData = new CardData(name,description,flavorText, power, toughness,sprite,null,keywordsList);
            cardDatabase.Add(name,newCardData);
        }
    }
    

}
