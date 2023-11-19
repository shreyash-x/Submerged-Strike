using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class LoadUI : MonoBehaviour
    {
        [SerializeField] private float transitionDuration;
        [SerializeField] private float moveDelta;
        [SerializeField] private GameObject[] thingsToMoveLeft;
        [SerializeField] private GameObject[] thingsToMoveRight;

        [SerializeField] private Image fadeImage;

        [SerializeField] private GameObject levelSelectMenu = null;


        private void Awake()
        {
            StartCoroutine(TransitionIn());
        }

        public void LoadGame()
        {
            StartCoroutine(LoadLevelSelect());
        }
        public void LoadMenu()
        {
            StartCoroutine(TransitionOutHome());
        }

        public void LoadLevel(int level)
        {
            StartCoroutine(TransitionLoadLevel(level));
        }

        private IEnumerator TransitionIn()
        {
            fadeImage.gameObject.SetActive(true);
            var color = Color.black;
            float elapsed = 0;
            while (elapsed < transitionDuration)
            {
                color.a = Mathf.Lerp(1, 0, elapsed / transitionDuration);
                fadeImage.color = color;

                elapsed += Time.deltaTime;
                yield return null;
            }
            fadeImage.gameObject.SetActive(false);
        }


        private IEnumerator TransitionOutHome()
        {
            fadeImage.gameObject.SetActive(true);
            var asyncOperation = SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Single);
            asyncOperation.allowSceneActivation = false;

            var color = Color.black;
            float elapsed = 0;
            while (elapsed < transitionDuration)
            {
                color.a = Mathf.Lerp(0, 1, elapsed / transitionDuration);
                fadeImage.color = color;

                foreach (var o in thingsToMoveLeft)
                {
                    o.transform.position += Vector3.left * moveDelta * Time.deltaTime;
                }

                foreach (var o in thingsToMoveRight)
                {
                    o.transform.position += Vector3.right * moveDelta * Time.deltaTime;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }
            asyncOperation.allowSceneActivation = true;
        }


        private IEnumerator LoadLevelSelect()
        {

            float elapsed = 0;
            bool halfWay = false;
            while (elapsed < transitionDuration)
            {
                if (elapsed > transitionDuration / 2 && !halfWay)
                {
                    levelSelectMenu.SetActive(true);
                    halfWay = true;
                }

                foreach (var o in thingsToMoveLeft)
                {
                    o.transform.position += Vector3.left * moveDelta * Time.deltaTime;
                }

                foreach (var o in thingsToMoveRight)
                {
                    o.transform.position += Vector3.right * moveDelta * Time.deltaTime;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator TransitionLoadLevel(int level)
        {
            var sceneName = $"Level{level}";
            fadeImage.gameObject.SetActive(true);
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            asyncOperation.allowSceneActivation = false;
            GameObject[] gameObjectsToMove;
            if (levelSelectMenu != null)
            {
                gameObjectsToMove = new GameObject[levelSelectMenu.transform.childCount];
                for (int i = 0; i < levelSelectMenu.transform.childCount; i++)
                {
                    gameObjectsToMove[i] = levelSelectMenu.transform.GetChild(i).gameObject;
                }
            }
            else
            {
                gameObjectsToMove = new GameObject[0];
            }

            var color = Color.black;
            float elapsed = 0;
            while (elapsed < transitionDuration)
            {
                color.a = Mathf.Lerp(0, 1, elapsed / transitionDuration);
                fadeImage.color = color;
                foreach (var o in gameObjectsToMove)
                {
                    // check if object has a tag BG
                    if (o.CompareTag("BG"))
                    {
                        o.transform.position += Vector3.up * moveDelta * Time.deltaTime;
                    }
                    else
                    {
                        o.transform.position += Vector3.down * moveDelta * Time.deltaTime;
                    }

                }
                elapsed += Time.deltaTime;
                yield return null;
            }
            asyncOperation.allowSceneActivation = true;
        }
    }
}
