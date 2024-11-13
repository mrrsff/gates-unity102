using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public float movementSpeed = 1f;
    public float rotationSpeed = 120f;
    
    private ZombieAnimationController animationController;
    private Rigidbody rb;
    private NavMeshAgent agent;
    private Transform target;
    private bool isPlayerVisible;
    
    // ..., add other bools as needed
    private bool isDead;
    
    
    private void Awake()
    {
        animationController = GetComponent<ZombieAnimationController>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;
        agent.angularSpeed = rotationSpeed;
    }

    private void Update()
    {
        CheckVision();
        
        if (isPlayerVisible)
        {
            MoveTowardsTarget();
        }
    }
    
    // You can use this variables to configure the vision of the zombie
    private int rayCount = 5;
    private float detectionAngle = 60f;
    private float detectionDistance = 10f;
    private void CheckVision()
    {
        // TODO: Shoot raycasts in the forward direction of the zombie
        var rays = RaycastUtils.GetViewRays(transform, rayCount, detectionAngle);
    }
    
    private void MoveTowardsTarget()
    {
        // TODO: Find a way to optimize this method to avoid calling SetDestination every frame
        // Move the zombie towards the target
        agent.SetDestination(target.position);
    }
}
