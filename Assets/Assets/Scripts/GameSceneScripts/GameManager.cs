using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private List<GameObject> allRopes = new List<GameObject>();

    private void Awake()
    {
        foreach (var rope in GameObject.FindGameObjectsWithTag("Rope"))
            allRopes.Add(rope);
    }

    public void ResetLevel()
    {
        foreach (var rope in allRopes)
            rope.SetActive(true);
    }
}
