using R3;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReactiveExample : MonoBehaviour
{
    public Button coinButton;

    public Button cancelButton;

    private IDisposable clickSubscription;
    CancellationTokenSource cts;

    [SerializeField]
    private TextMeshProUGUI coinsText;

    private int coins = 0;

    public void Awake()
    {
        coinsText.text = "Coins: 0";
        cts = new CancellationTokenSource();
        cancelButton.onClick.AddListener(() => cts.Cancel());
    }

    public void Start()
    {
        clickSubscription = coinButton.OnClickAsObservable().Subscribe(_ =>
        {
            coinsText.text = string.Format("Coins: {0}", ++coins);
            Debug.Log("Coins!");
        });
    }

    public void OnDestroy()
    {
        clickSubscription?.Dispose();
        cts?.Dispose();
    }
}