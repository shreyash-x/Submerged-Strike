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


        private void Awake()
        {
            StartCoroutine(TransitionIn());
        }

        public void LoadGame()
        {
            StartCoroutine(TransitionOut(1));
        }
        public void LoadMenu()
        {
            StartCoroutine(TransitionOut(0));
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
        
        
        private IEnumerator TransitionOut(int sceneIndex)
        {
            fadeImage.gameObject.SetActive(true);
            var asyncOperation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
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
    }
}
