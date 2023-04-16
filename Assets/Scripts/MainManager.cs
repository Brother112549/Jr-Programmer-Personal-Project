using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MainManager : MonoBehaviour {
    // ENCAPSULATION
    public static MainManager Instance { get; private set; }
    public string highScoreName { get; private set; }
    public int highScore { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadScore();
    }

    [System.Serializable]
    class SaveData {
        public string Name;
        public int Score;
    }

    public void SaveScore(string name, int score) {
        if(score <= highScore) {
            //only save best score
            return;
        }

        SaveData data = new SaveData();
        data.Name = name;
        data.Score = score;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadScore() {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path)) {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            highScoreName = data.Name;
            highScore = data.Score;
        }
    }
}
