using System;
using System.Collections;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    [SerializeField] float _timeToOtherEnd = 1f;

    [SerializeField] RectTransform _goodZone;
    [SerializeField] RectTransform _indicator;
    [SerializeField] RectTransform _line;

    public bool WasLastRunSuccessful { get; set; }

    public IEnumerator StartMiniGame()
    {
        //var minPos = _line.rect.x

        var minPos = _indicator.rect.width / 2f;
        var maxPos = minPos + _line.rect.width;

        var timeToOtherEnd = _timeToOtherEnd;
        var distancePerStep = ((maxPos - minPos) / timeToOtherEnd) * Time.deltaTime;
        var currentPosition = minPos;

        _indicator.anchoredPosition = new Vector2(minPos, _indicator.anchoredPosition.y);


        var deltaDistance = distancePerStep;
        while (true)
        {
            //TODO check if win
            if (Input.GetKeyDown(KeyCode.Space))
                break;

            if (_indicator.anchoredPosition.x >= maxPos)
                deltaDistance = distancePerStep * -1f;
            else if (_indicator.anchoredPosition.x <= minPos)
                deltaDistance = distancePerStep;

            _indicator.anchoredPosition += new Vector2(deltaDistance, 0f);

            yield return new WaitForSeconds(Time.deltaTime);
        }

        var indicatorXPos = _indicator.anchoredPosition.x;
        var minGoodZone = _goodZone.anchoredPosition.x - (_goodZone.rect.width / 2f);
        var maxGoodZone = _goodZone.anchoredPosition.x + (_goodZone.rect.width / 2f);

        WasLastRunSuccessful = indicatorXPos >= minGoodZone && indicatorXPos <= maxGoodZone;
    }
}
