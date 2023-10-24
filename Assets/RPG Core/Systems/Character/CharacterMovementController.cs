using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class CharacterMovementController : MonoBehaviour
{
    private NavMeshAgent _agent;

    [Inject]
    public void Initialize(NavMeshAgent agent)
    {
        _agent = agent;
    }

    public void MoveTo(Vector3 destination)
    {
        _agent.destination = destination;
    }
}
