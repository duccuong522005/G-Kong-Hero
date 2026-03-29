using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class AchievementConfig {
    public string id;
    public string title;
    public int rewardAmount = 10;
    public bool isCompleted;
    [HideInInspector]
    public bool isClaimed;
    public GameObject panel; // contain item UI
    public Button claimButton;
    public Text titleText;
    public Text rewardText;
}

public class AchievementManager : MonoBehaviour {
    public static AchievementManager Instance;

    public List<AchievementConfig> achievements = new List<AchievementConfig>();

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Không giữ object qua scene vì các tham chiếu UI trong menu sẽ mất khi sang scene khác.
        // Manager chỉ cần tồn tại trong menu, lưu/trích dữ liệu bằng PlayerPrefs.
    }

    void Start() {
        LoadAchievementState();
        RefreshUI();
    }

    public void ResetAllAchievements() {
        foreach (var item in achievements) {
            item.isCompleted = false;
            item.isClaimed = false;
        }
        SaveAchievementState();
        RefreshUI();
    }

    public void RefreshUI() {
        foreach (var item in achievements) {
            if (item.panel != null) item.panel.SetActive(true);
            if (item.titleText != null) item.titleText.text = item.title;
            if (item.rewardText != null) item.rewardText.text = "Reward: " + item.rewardAmount + " ";

            if (!item.isCompleted) {
                SetButtonText(item, "Locked");
                SetClaimButtonInteractable(item, false);
                SetClaimButtonColor(item, Color.black);
            } else if (!item.isClaimed) {
                SetButtonText(item, "Claim " + item.rewardAmount);
                SetClaimButtonInteractable(item, true);
                SetClaimButtonColor(item, Color.red);
            } else {
                SetButtonText(item, "Claimed");
                SetClaimButtonInteractable(item, false);
                SetClaimButtonColor(item, Color.gray);
            }
        }
    }

    void SetPanelColor(AchievementConfig item, Color color) {
        if (item.panel == null) return;
        var img = item.panel.GetComponent<Image>();
        if (img != null) img.color = color;
    }

    void SetButtonText(AchievementConfig item, string text) {
        if (item.claimButton == null) return;
        var txt = item.claimButton.GetComponentInChildren<Text>();
        if (txt != null) txt.text = text;
    }


    void SetClaimButtonInteractable(AchievementConfig item, bool state) {
        if (item.claimButton == null) return;
        item.claimButton.interactable = state;
    }

    void SetClaimButtonColor(AchievementConfig item, Color color) {
        if (item.claimButton == null) return;
        var cb = item.claimButton.colors;
        cb.normalColor = color;
        cb.highlightedColor = color;
        cb.pressedColor = color * 0.9f;
        cb.disabledColor = Color.gray;
        item.claimButton.colors = cb;
    }

    public void CompleteAchievement(string id) {
        var item = achievements.Find(a => a.id == id);
        if (item == null) { Debug.LogWarning("Achievement not found: " + id); return; }
        if (!item.isCompleted) {
            item.isCompleted = true;
            item.isClaimed = false; // Mở khóa mới, chưa claim
            Debug.Log("Achievement unlocked: " + id + " - " + item.title);
            SaveAchievementState();
            RefreshUI();
        }
    }

    public void ClaimAchievement(string id) {
        var item = achievements.Find(a => a.id == id);
        if (item == null) { Debug.LogWarning("Achievement not found: " + id); return; }
        if (item.isCompleted && !item.isClaimed) {
            item.isClaimed = true;
            GlobalValue.SavedCoins += item.rewardAmount;
            Debug.Log("Achievement claimed: " + item.title + " reward=" + item.rewardAmount + " coins. Total Coins=" + GlobalValue.SavedCoins);
            SaveAchievementState();
            RefreshUI();
        } else if (!item.isCompleted) {
            Debug.LogWarning("Achievement not completed yet: " + id);
        } else {
            Debug.LogWarning("Achievement already claimed: " + id);
        }
    }

    public void ToggleAchievement(string id) {
        var item = achievements.Find(a => a.id == id);
        if (item == null) return;
        item.isCompleted = !item.isCompleted;
        if (!item.isCompleted) item.isClaimed = false;
        SaveAchievementState();
        RefreshUI();
    }

    public void LoadAchievementState() {
        // Nếu chưa có key, reset tất cả về locked để tránh trạng thái pre-unlocked do inspector để nhầm true
        bool hasAny = false;
        foreach (var item in achievements) {
            if (PlayerPrefs.HasKey(item.id + "_completed") || PlayerPrefs.HasKey(item.id + "_claimed")) {
                hasAny = true;
                break;
            }
        }

        if (!hasAny) {
            foreach (var item in achievements) {
                item.isCompleted = false;
                item.isClaimed = false;
            }
            return;
        }

        foreach (var item in achievements) {
            item.isCompleted = PlayerPrefs.GetInt(item.id + "_completed", 0) == 1;
            item.isClaimed = PlayerPrefs.GetInt(item.id + "_claimed", 0) == 1;
        }
    }

    public void SaveAchievementState() {
        foreach (var item in achievements) {
            PlayerPrefs.SetInt(item.id + "_completed", item.isCompleted ? 1 : 0);
            PlayerPrefs.SetInt(item.id + "_claimed", item.isClaimed ? 1 : 0);
        }
        PlayerPrefs.Save();
    }
}
