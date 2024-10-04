using Newtonsoft.Json;
using UnityEngine;

namespace FastFoodRush
{
    public static class SaveSystem
    {
        private const string LastPlayRestaurantId = "LastPlayRestaurantId";
        private const string MainTutorialStep = "mainTutorialStep";
        
        //레스토랑 id는 scene name으로 지정
        public static void SaveLastPlayRestaurantId(string id)
        {
            PlayerPrefs.SetString(LastPlayRestaurantId, id);
        }

        public static string LoadLastPlayRestaurantId()
        {
            string id = PlayerPrefs.GetString(LastPlayRestaurantId);
            if (!id.Contains("Restaurant"))
            {
                Debug.LogWarning($"failed last play restaurant id");
                return null;
            }

            return id;
        }
        
        public static void SaveRestaurantData(string key, RestaurantData restaurantData)
        {
            var jsonString = JsonConvert.SerializeObject(restaurantData);
            PlayerPrefs.SetString(key, jsonString);
        }

        public static RestaurantData LoadRestaurantData(string restaurantId)
        {
            var data = PlayerPrefs.GetString(restaurantId);
            RestaurantData restaurantData = JsonConvert.DeserializeObject<RestaurantData>(data);
            if (restaurantData == null)
            {
                Debug.LogWarning($"restaurant data is null {restaurantId}");
                return null;
            }

            return restaurantData;
        }

        public static void DeleteAllData()
        {
            PlayerPrefs.DeleteAll();
        }

        public static int GetMainTutorialStep()
        {
            return PlayerPrefs.GetInt(MainTutorialStep);
        }

        public static void SaveMainTutorialStep(int mainTutorialStep)
        {
            PlayerPrefs.SetInt(MainTutorialStep, mainTutorialStep);
        }
    }
}