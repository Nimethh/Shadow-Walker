using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string nextSceneName;
    [SerializeField]
    Animator animator;
    private bool goToNextScene = false;

    void Start()
    {
        animator = GameObject.Find("LevelFadePanel").GetComponent<Animator>();
    }

    void Update()
    {
        if(goToNextScene)
        {
            StartCoroutine(LoadNextScene(nextSceneName));
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Debug.Log("PlayerEntered");
            goToNextScene = true;
        }
    }

    IEnumerator LoadNextScene(string p_nextSceneName)
    {
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(p_nextSceneName);
    }
}