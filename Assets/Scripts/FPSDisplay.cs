using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPSDisplay : MonoBehaviour {

    private float deltaTime = 0;

    private TextMeshProUGUI text;

    private void Start() {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update() {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        float msec = deltaTime * 1000;
        float fps = 1.0f / deltaTime;
        string fpsText = $"{msec:0.0} ms ({fps:0.} fps)";
        text.text = fpsText;

        // if (fps < _fpsWarningThreshold && !_fadeStarted) {
        //     Debug.Log(fps);
        //     StartCoroutine(FlashImage());
        //     _fadeStarted = true;
        // }
        // if (fps >= _fpsWarningThreshold) {
        //     _fadeStarted = false;
        //     StopCoroutine(FlashImage());
        // }
    }
}
