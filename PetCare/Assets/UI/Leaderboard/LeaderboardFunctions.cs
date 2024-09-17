using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public static class LeaderboardFunctions
{
    private static string m_ServerIP = "oracle.goblin-blenny.ts.net";
    private static int m_ServerPort = 443;
    public static LeaderboardEntries RequestLeaderboardScores(){
        HttpClient client = new HttpClient();
        try
        {
            var response = client.GetAsync($"https://{m_ServerIP}:{m_ServerPort}/leaderboard").Result;
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
                Task<string> responseString = responseContent.ReadAsStringAsync();
                string json = "{\"entries\":" + responseString.Result + "}";
                LeaderboardEntries entries = JsonUtility.FromJson<LeaderboardEntries>(json);
                return entries;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        return null;
    }

    public static void SubmitScore(LeaderboardEntry entry)
    {
        HttpClient client = new HttpClient();
        try
        {
            string json = JsonUtility.ToJson(entry);
            var response = client.PostAsync($"https://{m_ServerIP}:{m_ServerPort}/leaderboard", new StringContent(json)).Result;
            if (response.IsSuccessStatusCode)
            {
                Debug.Log("Score submitted successfully");
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}

[Serializable]
public class LeaderboardEntries
{
    public List<LeaderboardEntry> entries;

    public LeaderboardEntries(List<LeaderboardEntry> entries)
    {
        this.entries = entries;
    }
}

[Serializable]
public class LeaderboardEntry
{
    public string player;
    public string dog;
    public int score;

    public LeaderboardEntry(string playerName, string dogName, int score)
    {
        player = playerName;
        dog = dogName;
        this.score = score;
    }
}