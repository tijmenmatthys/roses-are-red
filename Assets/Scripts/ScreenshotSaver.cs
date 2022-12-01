using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScreenshotSaver : MonoBehaviour
{
    [SerializeField] private bool _saveScreenshotOnStart = true;
    [SerializeField] private bool _saveScreenshotOnPressingS = true;
    [Tooltip("If the render at game start isn't perfect yet, wait a bit.")]
    [SerializeField] private float _delay = 1f;
    [SerializeField] private string _path = "Export/";
    [SerializeField] private string _fileName = "Screenshot";
    [SerializeField] private int _resolutionMultiplier = 2;

    // Start is called before the first frame update
    void Start()
    {
        if (_saveScreenshotOnStart)
            StartCoroutine(SaveFrameDelayed());
    }

    private void Update()
    {
        if (_saveScreenshotOnPressingS && Input.GetKeyDown(KeyCode.S))
            SaveCurrentFrame();
    }

    public void SaveCurrentFrame()
    {
        if (!Directory.Exists(_path))
            Directory.CreateDirectory(_path);

        int version = 0;
        while (File.Exists(FullFilename(version)))
            version++;

        ScreenCapture.CaptureScreenshot(FullFilename(version), _resolutionMultiplier);
        Debug.Log($"Saved screenshot at {FullFilename(version)}");
    }

    private string FullFilename(int id)
    {
        return $"{_path}{_fileName}_{id:D3}.png";
    }

    private IEnumerator SaveFrameDelayed()
    {
        yield return new WaitForSeconds(_delay);
        SaveCurrentFrame();
    }
}
