using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    UI_Start start;
    LoadingSlideManager loading;
    [SerializeField] private string sceneName = "town(pr)";
    public AudioSource BGM_Title;
    public AudioSource[] SE_Title;
    private bool loading_Start;

    

    private void Start()
    {
        start = FindObjectOfType<UI_Start>();
        loading = FindObjectOfType<LoadingSlideManager>();
    }

    public void ClickStart()
    {
        SE_Title[1].Play();
        start.isFadeToBlack = true;
        StartCoroutine(WaitTitle_BlackScreen());
    }

    public void ClickExit()
    {
        SE_Title[1].Play();
        Application.Quit();
    }


    public IEnumerator WaitTitle_BlackScreen()
    {

        yield return new WaitUntil(() => !start.isFadeToBlack);
        BGM_Title.Stop();
        StartCoroutine(Loading());

    }

    IEnumerator Loading()
    {
        if (loading_Start)
        {
            yield break;
        }

        loading_Start = true;

        LoadingSlideManager.loadingFinished = false;
        StartCoroutine(loading.LoadingAppearSlide());

        yield return new WaitUntil(() => LoadingSlideManager.loadingFinished);
        loading_Start = false;
        SceneManager.LoadScene(sceneName);
    }
}
