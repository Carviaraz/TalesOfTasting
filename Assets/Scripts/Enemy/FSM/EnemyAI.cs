using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private string initialStateID;

    [Header("States")]
    [SerializeField] private FSMState[] states;

    public FSMState CurrentState {  get; set; }
    public Transform Player { get; set; }

    private void Start()
    {
        ChangeState(initialStateID);
    }

    private void Update()
    {
        CurrentState.ExecuteState(this);
    }

    public void ChangeState(string newStateID)
    {
        FSMState newState = GetState(newStateID);

        if (newState == null) return;

        CurrentState = newState;
    }

    private FSMState GetState(string newStateID) 
    {
        for (int i = 0; i < states.Length; i++)
        {
            if (states[i].StateID == newStateID)
            {
                return states[i];
            }
        }

        return null;
    }
}
