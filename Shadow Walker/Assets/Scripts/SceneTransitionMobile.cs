using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionMobile : MonoBehaviour
{
    [SerializeField]
    string nextSceneName;
    [SerializeField]
    string previousSceneName;
    [SerializeField]
    string firstSceneName;
    [SerializeField]
    string thisSceneName;
    [SerializeField]
    string videoSceneName;

    [SerializeField]
    float afkTimer;
    [SerializeField]
    float afkTimerCountDown;
    [SerializeField]
    Animator animator;
    bool goToNextScene = false;
    GameObject levelFadePanel;

    void Start()
    {
        levelFadePanel = GameObject.Find("LevelFadePanel");
        animator = levelFadePanel.GetComponent<Animator>();
        //levelFadePanel.GetComponent<Image>().enabled = false;
        afkTimerCountDown = afkTimer;
    }

    void Update()
    {
        if(goToNextScene)
        {
            StartCoroutine(LoadNextScene(nextSceneName));
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(LoadNextScene(nextSceneName));
        }
        else if(Input.GetKeyDown(KeyCode.N))
        {
            StartCoroutine(LoadPreviousScene(previousSceneName));
        }
        else if(Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(LoadFirstScene(firstSceneName));
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ReloadScene(thisSceneName));
        }

        if(!Input.anyKeyDown && SceneManager.GetActiveScene().name != videoSceneName)
        {
            if (afkTimerCountDown <= 0)
            {
                StartCoroutine(LoadVideoScene(videoSceneName));
            }
            else
                afkTimerCountDown -= Time.deltaTime;
        }
        else if(Input.anyKeyDown && SceneManager.GetActiveScene().name == videoSceneName)
        {
            StartCoroutine(LoadFirstScene(firstSceneName));
        }
        else
        {
            afkTimerCountDown = afkTimer;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            goToNextScene = true;
        }
    }

    IEnumerator LoadNextScene(string p_nextSceneName)
    {
        levelFadePanel.GetComponent<Image>().enabled = true;
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(p_nextSceneName);
    }

    IEnumerator LoadPreviousScene(string p_previousSceneName)
    {
        levelFadePanel.GetComponent<Image>().enabled = true;
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(p_previousSceneName);
    }

    IEnumerator LoadFirstScene(string p_firstSceneName)
    {
        levelFadePanel.GetComponent<Image>().enabled = true;
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(p_firstSceneName);
    }

    IEnumerator ReloadScene(string p_thisSceneName)
    {
        levelFadePanel.GetComponent<Image>().enabled = true;
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(p_thisSceneName);
    }

    IEnumerator LoadVideoScene(string p_videoScene)
    {
        levelFadePanel.GetComponent<Image>().enabled = true;
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(p_videoScene);
    }
}