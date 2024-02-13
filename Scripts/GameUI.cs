using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image fadePLane;
    public GameObject gameOverUI;
    // Start is called before the first frame update
    void Start()
    {
        FindAnyObjectByType<Player>().onDeath += GameUI_onDeath;
    }

    private void GameUI_onDeath()
    {
        StartCoroutine(GameOverUI(Color.clear,Color.black,1));
        gameOverUI.SetActive(true);
    }

    IEnumerator GameOverUI(Color from,Color to,float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePLane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    public void RestartBtn() => SceneManager.LoadScene("Game");

    // Update is called once per frame
    void Update()
    {
        
    }
}
