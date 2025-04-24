using TMPro;
using UnityEngine;

public class StateMachineDebugText : MonoBehaviour
{
    private SensorEnabledMovementStateMachine stateMachine;

    private TextMeshProUGUI textMeshProUGUI;

    // Start is called before the first frame update
    void Start()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        stateMachine = FindAnyObjectByType<SensorEnabledMovementStateMachine>();    
    }

    // Update is called once per frame
    void Update()
    {
        textMeshProUGUI.text = stateMachine.ToDebugString();
    }
}
