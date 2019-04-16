using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JudgeText : MonoBehaviour
{
    private Text judgeText;

    private float elapsedTime;

    private void Awake() {
        judgeText = GetComponent<Text>();
        judgeText.enabled = false;
    }

    private void Update() {
        if (judgeText.IsActive())
            elapsedTime += Time.deltaTime;

        if (elapsedTime < 1f)
            return;

        elapsedTime = 0f;
        judgeText.enabled = false;
    }

    public void Draw(Judge judge) {
        judgeText.text = judge.ToString();
        judgeText.enabled = true;
        elapsedTime = 0f;
    }
}
