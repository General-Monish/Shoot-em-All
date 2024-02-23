using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image fadePLane;
    public GameObject gameOverUI;

    public RectTransform newWaveBanner;
    public TextMeshProUGUI waveCount;
    public TextMeshProUGUI enemiesCount;

    Spawner spawner;

    private void Awake()
    {
        spawner = FindAnyObjectByType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    public void OnNewWave(int waveNumber)
    {
        string[] numbers = { "One", "Two", "Three", "Four", "Five" };
        waveCount.text = "Wave:" + numbers[waveNumber - 1];
        string enemyCountString = ((spawner.waves[waveNumber - 1].infinite) ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount+"");
        enemiesCount.text = "Enemies:" + enemyCountString;

        StartCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }
    // Start is called before the first frame update
    void Start()
    {
        FindAnyObjectByType<Player>().onDeath += GameUI_onDeath;
    }

    IEnumerator AnimateNewWaveBanner()
    {
        float delayTime=2f;
        float animatePercent = 0;
        float speed=1f;
        int dir = 1;

        float endDelayTime = Time.time + 1 / speed + delayTime;
        while (animatePercent>=0)
        {
            animatePercent += Time.deltaTime * speed * dir;
            if (animatePercent >= 1)
            {
                animatePercent = 1;
                if (Time.time > endDelayTime)
                {
                    dir = -1;
                }
            }
            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-255, 840, animatePercent);
            yield return null;
        }
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
