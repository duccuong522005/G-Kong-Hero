using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Quản lý bảng xếp hạng Offline cho game 'Kong Hero Adventure' với 8 màn chơi.
/// Sử dụng cấu trúc Serialized List giống AchievementManager.
/// </summary>
public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    [Header("Danh sách Leaderboard Entries")]
    public List<LeaderboardEntry> allEntries = new List<LeaderboardEntry>();

    [Header("Inspector Default Entries")]
    public List<LeaderboardEntry> inspectorEntries = new List<LeaderboardEntry>();

    [Header("Top 3 UI")]
    public Text[] rankTexts = new Text[3];
    public Text[] nameTexts = new Text[3];
    public Text[] levelTexts = new Text[3];

    [Header("Người chơi hiện tại")]
    public string currentPlayerName = "You";

    public int levelsPerWorld = 4;

    private const string KEY_COUNT = "Leaderboard_Count";
    private const string KEY_NAME = "Leaderboard_Name_";
    private const string KEY_LEVEL = "Leaderboard_Level_";
    private const string KEY_PENDING_SCORE = "Leaderboard_Pending_You_Level";
    /// <summary>Persisted entry count; bounded so a corrupt PlayerPrefs value cannot freeze the main thread.</summary>
    private const int MaxPersistedLeaderboardEntries = 64;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Không giữ object qua scene vì các tham chiếu UI trong menu sẽ mất khi chuyển scene.
            // Chỉ giữ logic không UI (data) qua PlayerPrefs.
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (levelsPerWorld <= 0)
        {
            levelsPerWorld = 4;
        }
    }

    void Start()
    {
        // Nếu trong Inspector bạn đã nhập trực tiếp vào allEntries, dùng nó làm dữ liệu mặc định
        if ((inspectorEntries == null || inspectorEntries.Count == 0) && allEntries != null && allEntries.Count > 0)
        {
            inspectorEntries = new List<LeaderboardEntry>();
            foreach (var e in allEntries)
            {
                inspectorEntries.Add(new LeaderboardEntry { playerName = e.playerName, levelsCompleted = e.levelsCompleted });
            }
        }

        LoadLeaderboard();

        // Nếu score mới được lưu tạm khi leaderboard manager không tồn tại (level scene), áp dụng lại ở menu.
        ApplyPendingPlayerScore();

        RefreshUI();
    }

    /// <summary>
    /// Cập nhật tiến độ người chơi hiện tại (currentPlayerName), cơ chế cướp ngôi.
    /// </summary>
    /// <param name="newLevel">Số màn mới vượt qua (0-8)</param>
    /// <summary>
    /// Chuyển level map/world thành level global 1..8.
    /// </summary>
    public int GetGlobalLevel(int worldPlaying, int levelPlaying)
    {
        // levelsPerWorld = 4, map1=1-4, map2=5-8
        return (worldPlaying - 1) * levelsPerWorld + levelPlaying;
    }

    /// <summary>
    /// Cập nhật tiến độ người chơi hiện tại (currentPlayerName).
    /// </summary>
    /// <param name="newGlobalLevel">Số level toàn cục (1..8)</param>
    public void UpdatePlayerScore(int newGlobalLevel)
    {
        if (newGlobalLevel <= 0)
        {
            Debug.Log("Leaderboard: newGlobalLevel <= 0 -> ignore");
            return;
        }

        int clampedLevel = Mathf.Clamp(newGlobalLevel, 0, levelsPerWorld * 2);

        LeaderboardEntry playerEntry = allEntries.Find(e => e.playerName == currentPlayerName);
        if (playerEntry != null)
        {
            if (clampedLevel > playerEntry.levelsCompleted)
            {
                playerEntry.levelsCompleted = clampedLevel;
            }
            else
            {
                Debug.Log("Leaderboard: player current level not higher -> no update");
                return;
            }
        }
        else
        {
            allEntries.Add(new LeaderboardEntry
            {
                playerName = currentPlayerName,
                levelsCompleted = clampedLevel
            });
        }

        SortAndRefreshUI();
    }

    /// <summary>
    /// Cập nhật điểm của bất kỳ người chơi nào (tên + level), dùng khi cần.
    /// </summary>
    public void UpdatePlayerScore(string playerName, int newLevel)
    {
        int clampedLevel = Mathf.Clamp(newLevel, 0, 8);
        LeaderboardEntry entry = allEntries.Find(e => e.playerName == playerName);

        if (entry != null)
        {
            if (clampedLevel > entry.levelsCompleted)
            {
                entry.levelsCompleted = clampedLevel;
            }
            else
            {
                return;
            }
        }
        else
        {
            allEntries.Add(new LeaderboardEntry
            {
                playerName = playerName,
                levelsCompleted = clampedLevel
            });
        }

        SortAndRefreshUI();
    }

    public void SortAndRefreshUI()
    {
        allEntries.Sort((a, b) => b.levelsCompleted.CompareTo(a.levelsCompleted));
        SaveLeaderboard();
        RefreshUI();
    }

    public void LoadLeaderboard()
    {
        // Nếu có dữ liệu lưu trước thì tải lên
        allEntries.Clear();

        int storedCount = PlayerPrefs.GetInt(KEY_COUNT, 0);
        if (storedCount < 0 || storedCount > MaxPersistedLeaderboardEntries)
        {
            Debug.LogWarning("Leaderboard: invalid stored count " + storedCount + "; resetting saved leaderboard.");
            PlayerPrefs.DeleteKey(KEY_COUNT);
            for (int i = 0; i < MaxPersistedLeaderboardEntries; i++)
            {
                PlayerPrefs.DeleteKey(KEY_NAME + i);
                PlayerPrefs.DeleteKey(KEY_LEVEL + i);
            }
            PlayerPrefs.Save();
            storedCount = 0;
        }
        if (storedCount > 0)
        {
            for (int i = 0; i < storedCount; i++)
            {
                string name = PlayerPrefs.GetString(KEY_NAME + i, "");
                int level = PlayerPrefs.GetInt(KEY_LEVEL + i, 0);
                if (!string.IsNullOrEmpty(name))
                {
                    allEntries.Add(new LeaderboardEntry { playerName = name, levelsCompleted = level });
                }
            }
        }

        // Nếu không có dữ liệu PlayerPrefs thì dùng dữ liệu Inspector (hoặc All Entries đã nhập) làm mặc định
        if (allEntries.Count == 0 && inspectorEntries != null && inspectorEntries.Count > 0)
        {
            foreach (var entry in inspectorEntries)
            {
                if (!string.IsNullOrWhiteSpace(entry.playerName))
                {
                    allEntries.Add(new LeaderboardEntry { playerName = entry.playerName, levelsCompleted = entry.levelsCompleted });
                }
            }
        }

        SortAndRefreshUI();
    }

    public void SaveLeaderboard()
    {
        int count = Mathf.Min(allEntries.Count, MaxPersistedLeaderboardEntries);
        PlayerPrefs.SetInt(KEY_COUNT, count);
        for (int i = 0; i < count; i++)
        {
            PlayerPrefs.SetString(KEY_NAME + i, allEntries[i].playerName);
            PlayerPrefs.SetInt(KEY_LEVEL + i, allEntries[i].levelsCompleted);
        }
        for (int i = count; i < MaxPersistedLeaderboardEntries; i++)
        {
            PlayerPrefs.DeleteKey(KEY_NAME + i);
            PlayerPrefs.DeleteKey(KEY_LEVEL + i);
        }
        PlayerPrefs.Save();
    }

    public void ApplyPendingPlayerScore()
    {
        int pending = PlayerPrefs.GetInt(KEY_PENDING_SCORE, 0);
        if (pending > 0)
        {
            Debug.Log("Leaderboard: apply pending score " + pending);
            UpdatePlayerScore(pending);
            PlayerPrefs.DeleteKey(KEY_PENDING_SCORE);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Chỉ hiển thị Top 3 trên UI (3 text slot cố định), các vị trí còn lại không hiển thị.
    /// </summary>
    public void RefreshUI()
    {
        // Sắp xếp lại chắc chắn
        allEntries.Sort((a, b) => b.levelsCompleted.CompareTo(a.levelsCompleted));
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log("Leaderboard RefreshUI: count=" + allEntries.Count);
#endif

        for (int i = 0; i < 3; i++)
        {
            if (i < allEntries.Count)
            {
                string displayName = (allEntries[i].playerName == currentPlayerName) ? "You" : allEntries[i].playerName;
                if (rankTexts != null && i < rankTexts.Length && rankTexts[i] != null) rankTexts[i].text = "Hạng " + (i + 1);
                if (nameTexts != null && i < nameTexts.Length && nameTexts[i] != null) nameTexts[i].text = displayName;
                if (levelTexts != null && i < levelTexts.Length && levelTexts[i] != null) levelTexts[i].text = allEntries[i].levelsCompleted.ToString();
            }
            else
            {
                if (rankTexts != null && i < rankTexts.Length && rankTexts[i] != null) rankTexts[i].text = "-";
                if (nameTexts != null && i < nameTexts.Length && nameTexts[i] != null) nameTexts[i].text = "-";
                if (levelTexts != null && i < levelTexts.Length && levelTexts[i] != null) levelTexts[i].text = "-";
            }
        }
    }
}

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int levelsCompleted;
}
