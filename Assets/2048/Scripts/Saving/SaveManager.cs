using System;
using System.IO;
using UnityEngine;
using _2048.Scripts.Views;

namespace _2048.Scripts.Saving
{
    [Serializable]
    public sealed class BestScoreSaveData
    {
        public int bestScore;
    }

    public sealed class SaveManager : MonoBehaviour
    {
        [SerializeField] private string savePath = "2048-save.json";

        private ScoreView _scoreView;
        private string FullSavePath => Path.Combine(Application.persistentDataPath, Path.GetFileName(savePath));

        private void Awake()
        {
            _scoreView = FindFirstObjectByType<ScoreView>();
        }

        public void Save()
        {
            if (_scoreView == null)
                return;

            try
            {
                var data = new BestScoreSaveData { bestScore = _scoreView.BestScore };
                File.WriteAllText(FullSavePath, JsonUtility.ToJson(data, true));
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to save best score to '{FullSavePath}': {exception.Message}");
            }
        }

        public int Load()
        {
            if (!File.Exists(FullSavePath))
                return 0;

            try
            {
                var data = JsonUtility.FromJson<BestScoreSaveData>(File.ReadAllText(FullSavePath));
                return data == null ? 0 : Mathf.Max(0, data.bestScore);
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to load best score from '{FullSavePath}': {exception.Message}");
                return 0;
            }
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                Save();
        }
    }
}