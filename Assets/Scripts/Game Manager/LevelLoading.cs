using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelLoading : MonoBehaviour {


    public string[] loadingScreenTips;
    public Text loadingScreenHint;
    public Image loadingImage;

	
	public void StartLoadingLevel(int level) {
        gameObject.SetActive(true);
        StartCoroutine(ShowLoadingAnimation(level));
    }

    IEnumerator ShowLoadingAnimation(int level) {
        //Loading screen hint
        loadingScreenHint.text = "Hint: " + loadingScreenTips[Random.Range(0, loadingScreenTips.Length)];

        AsyncOperation aSync = Application.LoadLevelAsync(level);
        while(!aSync.isDone) {
            loadingImage.fillAmount = aSync.progress;

            yield return null;
        }
    }
}
