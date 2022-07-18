using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] Slider HpSlider;



    #region PlayerStatus

    // 플레이어 체력 제어
    public void ChangeHpSlider(float _ChangeValue)
    {
        StartCoroutine(CoChangeSlider(HpSlider, _ChangeValue, 0.3f));
    }

    IEnumerator CoChangeSlider(Slider _slider, float _ChangeValue, float _ChangeTime)
    {
        float timer = 0;
        float SliderValue = _slider.value; //초기 슬라이더 값
        float ResultValue = SliderValue + _ChangeValue;
        Debug.Log(ResultValue);
        while (timer <= _ChangeTime)
        {
            timer += Time.deltaTime;
            //Debug.Log(Mathf.Lerp(SliderValue, ResultValue, timer / _ChangeTime));
            _slider.value += Mathf.Lerp(SliderValue, ResultValue, timer / _ChangeTime)-_slider.value;
            yield return null;
        }

       _slider.value = SliderValue + _ChangeValue;



    }

    #endregion
}
