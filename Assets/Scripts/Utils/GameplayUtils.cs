using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector.Editor.GettingStarted;

namespace MatchingGame.Gameplay
{
    public static class GameplayUtils
    {
        public static Dictionary<string, CardDataConfig> GetRndCardFromTargetAmount(int targetAmount, GameDifficult gameDifficult, CardCategoryDataSO data)
        {
            Dictionary<string, CardDataConfig> stageConfigs = new Dictionary<string, CardDataConfig>();

            List<string> idList = new List<string>(data.GetObjByGameDiff(gameDifficult));
            for (int i = 0; i < targetAmount; i++)
            {
                string ranConfig;
                do
                {
                    int ranNum = Random.Range(0, idList.Count);
                    ranConfig = idList.ElementAt(ranNum);
                }
                while (!data.cardDataConfigDict.TryGetValue(ranConfig, out var obj));

                stageConfigs.Add(ranConfig, data.cardDataConfigDict[ranConfig]);
                idList.Remove(ranConfig);
            }

            return stageConfigs;
        }

        public static List<CardProperty> CreateCardPair(GameDifficult difficult, Dictionary<string, CardDataConfig> dataPairs)
        {
            List<CardProperty> cardProperties = new List<CardProperty>();
            foreach (KeyValuePair<string, CardDataConfig> pair in dataPairs)
            {
                if (difficult == GameDifficult.EASY)
                {
                    cardProperties.Add(new CardProperty(pair.Key, pair.Value.easySprite));
                    cardProperties.Add(new CardProperty(pair.Key, pair.Value.easySprite));
                }
                else if (difficult == GameDifficult.NORMAL)
                {
                    cardProperties.Add(new CardProperty(pair.Key, pair.Value.normalSprite));
                    cardProperties.Add(new CardProperty(pair.Key, pair.Value.normalSprite));
                }
                else if (difficult == GameDifficult.HARD)
                {
                    if (pair.Value.isEasy)
                        cardProperties.Add(new CardProperty(pair.Key, pair.Value.easySprite));
                    else if (pair.Value.isNormal)
                        cardProperties.Add(new CardProperty(pair.Key, pair.Value.normalSprite));

                    cardProperties.Add(new CardProperty(pair.Key, pair.Value.hardSprite));
                }
                else if (difficult == GameDifficult.ADVANCE)
                {
                    if (pair.Value.isEasy)
                        cardProperties.Add(new CardProperty(pair.Key, pair.Value.easySprite));
                    else if (pair.Value.isNormal)
                        cardProperties.Add(new CardProperty(pair.Key, pair.Value.normalSprite));

                    cardProperties.Add(new CardProperty(pair.Key, pair.Value.advanceSprite));
                }
            }

            return cardProperties;
        }

        public static List<CardProperty> ShuffleCard(List<CardProperty> cardList, int roundShuffle = 1)
        {
            for (int i = 0; i < roundShuffle; i++)
            {
                int lastIndex = cardList.Count - 1;
                while (lastIndex > 0)
                {
                    CardProperty tempValue = cardList[lastIndex];
                    int randomIndex = Random.Range(0, lastIndex);
                    cardList[lastIndex] = cardList[randomIndex];
                    cardList[randomIndex] = tempValue;
                    lastIndex--;
                }
            }

            return cardList;
        }
    }
}