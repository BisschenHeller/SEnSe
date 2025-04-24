using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBodyOfWater : MonoBehaviour
{
    [SerializeField]
    public float fillTime = 20.0f;

    [SerializeField]
    public float drainTime = 8.0f;

    [SerializeField]
    private float maxLevel;

    [SerializeField]
    private float minLevel;

    [SerializeField]
    [Range(0,1)]
    private float currentLerp;

    [SerializeField]
    private Transform schleuse;
    // Start is called before the first frame update
    void Start()
    {
        currentLerp = 0;
        FillingUp();
    }

    private void FillingUp() {
        DOTween.To(() => currentLerp, x => currentLerp = x, 1.0f, fillTime).SetEase(Ease.Linear).OnComplete(Drain) ;
    }

    private void Drain()
    {
        schleuse.DOMove(schleuse.transform.position + Vector3.up, 1f).SetEase(Ease.InOutBounce)
            .OnComplete(() => schleuse.DOMove(schleuse.transform.position - Vector3.up, 1f).SetEase(Ease.InCubic).SetDelay(drainTime-1));
      
        DOTween.To(() => currentLerp, x => currentLerp = x, 0.0f, drainTime).SetEase(Ease.Linear).SetDelay(0.5f).OnComplete(FillingUp);
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(minLevel, maxLevel, currentLerp), transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, Mathf.Lerp(minLevel, maxLevel, currentLerp), transform.position.z);
    }
}
