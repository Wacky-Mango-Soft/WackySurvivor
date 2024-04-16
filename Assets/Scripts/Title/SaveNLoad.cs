using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;


[System.Serializable]
public class SaveData
{
    public Vector3 playerPos;
    public Vector3 playerRot;

    public int currentHp;
    public int currentSp;
    public int currentDp;
    public int currentHungry;
    public int currentThirsty;
    public int currentSatisfy;

    public int day;
    public float time;

    public List<int> invenArrayNumber = new List<int>();
    public List<string> invenItemName = new List<string>();
    public List<int> invenItemCount = new List<int>();
}

public class SaveNLoad : MonoBehaviour
{
    private SaveData saveData = new SaveData();
    private string SAVE_DATA_DIRECTORY;
    private string SAVE_FILENAME = "/SaveFile.txt";
    private PlayerController thePlayer;
    private Inventory theInven;
    private StatusController theStatus;

    // Start is called before the first frame update
    void Start()
    {
        SAVE_DATA_DIRECTORY = Application.dataPath + "/Saves/";

        if (!Directory.Exists(SAVE_DATA_DIRECTORY))
            Directory.CreateDirectory(SAVE_DATA_DIRECTORY);
    }

    public void SaveData()
    {
        thePlayer = FindObjectOfType<PlayerController>();
        theInven = FindObjectOfType<Inventory>();
        theStatus = FindObjectOfType<StatusController>();

        saveData.playerPos = thePlayer.transform.position;
        saveData.playerRot = thePlayer.transform.eulerAngles;

        saveData.currentHp = theStatus.CurrentHp;
        saveData.currentSp = theStatus.CurrentSp;
        saveData.currentDp = theStatus.CurrentDp;
        saveData.currentHungry = theStatus.CurrentHungry;
        saveData.currentThirsty = theStatus.CurrentThirsty;
        saveData.currentSatisfy = theStatus.CurrentSatisfy;

        saveData.time = TimeManager.instance.Time;
        saveData.day = TimeManager.instance.Day;

        Slot[] slots = theInven.GetSlots();
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                saveData.invenArrayNumber.Add(i);
                saveData.invenItemName.Add(slots[i].item.itemName);
                saveData.invenItemCount.Add(slots[i].itemCount);
            }
        }

        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, json);

        Debug.Log("저장 완료");
        Debug.Log(json);
    }

    public void LoadData()
    {
        if (File.Exists(SAVE_DATA_DIRECTORY + SAVE_FILENAME)) 
        {
            thePlayer = FindObjectOfType<PlayerController>();
            theInven = FindObjectOfType<Inventory>();
            theStatus = FindObjectOfType<StatusController>();

            string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME);

            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            thePlayer.transform.position = saveData.playerPos + Vector3.up;
            thePlayer.transform.eulerAngles = saveData.playerRot;

            theStatus.CurrentHp = saveData.currentHp;
            theStatus.CurrentSp = saveData.currentSp;
            theStatus.CurrentDp = saveData.currentDp;
            theStatus.CurrentHungry = saveData.currentHungry;
            theStatus.CurrentThirsty = saveData.currentThirsty;
            theStatus.CurrentSatisfy = saveData.currentSatisfy;

            TimeManager.instance.Day = saveData.day;
            TimeManager.instance.Time = saveData.time;

            // 로드시 체력 원복을 위한 임시코드
            thePlayer.GetTheStatusController().SetFullHP();

            for (int i = 0; i < saveData.invenItemName.Count; i++)
            {
                theInven.LoadToInven(saveData.invenArrayNumber[i], saveData.invenItemName[i], saveData.invenItemCount[i]);
            }

            Debug.Log("로드 완료");
        }
        else
        {
            Debug.Log("세이브 파일이 존재하지 않습니다");
        }
    }

    public IEnumerator AutoSaveCoroutine() {
        GameManager.instance.isSaveDelay = true;
        SaveData();
        yield return new WaitForSeconds(60f);
        GameManager.instance.isSaveDelay = false;

    }
}
