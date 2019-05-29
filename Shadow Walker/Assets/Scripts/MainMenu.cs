using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    Animator animator;
    Button button;
    float transitionTimer = 1.5f;
    [SerializeField]
    float transitionTimerCooldown;
    bool moveToNextLevel;
    string nextLevelName;
    GameObject levelSelectionPanel;
    GameObject mainMenuPanel;

    void Start()
    {
        animator = GameObject.Find("LevelFadePanel").GetComponent<Animator>();
        levelSelectionPanel = GameObject.Find("LevelSelectionPanel");
        mainMenuPanel = GameObject.Find("MainMenuPanel");
        levelSelectionPanel.SetActive(false);
        transitionTimerCooldown = transitionTimer;
    }

    private void Update()
    {
        if(moveToNextLevel)
        {
            animator.SetTrigger("FadeOut");
            if(transitionTimerCooldown <= 0)
            {
                SceneManager.LoadScene(nextLevelName);
            }
            else
            {
                transitionTimerCooldown -= Time.deltaTime;
            }
        }
    }

    public void PuzzleMode()
    {
        levelSelectionPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void LoadScene(string p_sceneName)
    {
        moveToNextLevel = true;
        nextLevelName = p_sceneName;
        //animator.SetTrigger("FadeOut");
        //SceneManager.LoadScene(p_sceneName);
    }

    public IEnumerator LoadSceneEnumerator(string p_sceneName)
    {
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(p_sceneName);
    }
}
