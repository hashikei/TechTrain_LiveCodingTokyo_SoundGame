using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combo : MonoBehaviour
{
    private Text comboText;

    private float elapsedTime;

    private void Awake() {
        comboText = GetComponent<Text>();
        comboText.enabled = false;
    }

    private void Update() {
        if (comboText.IsActive())
            elapsedTime += Time.deltaTime;

        if (elapsedTime < 1f)
            return;

        elapsedTime = 0f;
        comboText.enabled = false;
    }

    public void Draw(int combo) {
        comboText.text = combo.ToString() + "Combo!!";
        comboText.enabled = true;
        elapsedTime = 0f;
    }
}
