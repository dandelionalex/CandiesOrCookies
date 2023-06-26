using System.Collections;
using System.Collections.Generic;
using PickMaster.Model;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class SelectLevel : MonoBehaviour
{
    [SerializeField] private ScrollRect scroll;

    [SerializeField] private Button backButton;


    private void Start()
    {
        scroll.normalizedPosition = new Vector2(0, 0);
        backButton.onClick.AddListener(() => { BackToMenu(); });
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene(SceneNames.MENU);
    }
}
