using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private float _textTimeInterval = 1f;

    [SerializeField] private GameObject[] _startTexts;
    [SerializeField] private GameObject[] _area1Texts;
    [SerializeField] private GameObject[] _area5Texts;
    [SerializeField] private GameObject[] _endTexts;

    [SerializeField]
    private GameObject
        _roseText, _violetText, _sunflowerText, _marigoldText, _lavenderText, _jasmineText;


    public IEnumerator PlayStartTexts() { yield return PlayTextSequence(_startTexts); }
    public IEnumerator PlayArea1Texts() { yield return PlayTextSequence(_area1Texts); }
    public IEnumerator PlayArea5Texts() { yield return PlayTextSequence(_area5Texts); }
    public IEnumerator PlayEndTexts() { yield return PlayTextSequence(_endTexts); }
    public void PlayFlowerText(FlowerType type)
    {
        switch (type)
        {
            case FlowerType.Rose: _roseText.SetActive(true); break;
            case FlowerType.Violet: _violetText.SetActive(true); break;
            case FlowerType.Sunflower: _sunflowerText.SetActive(true); break;
            case FlowerType.Marigold: _marigoldText.SetActive(true); break;
            case FlowerType.Lavender: _lavenderText.SetActive(true); break;
            case FlowerType.Jasmine: _jasmineText.SetActive(true); break;
        }
    }

    private IEnumerator PlayTextSequence(GameObject[] texts)
    {
        foreach (var text in texts)
        {
            text.SetActive(true);
            yield return new WaitForSeconds(_textTimeInterval);
        }
    }
}
