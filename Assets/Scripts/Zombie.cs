using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public ParticleSystem bloodEffect;
    public float movementSpeed = 1f;
    public float rotationSpeed = 120f;
    public string enemyDeadLayer = "EnemyDead";
    
    private ZombieAnimationController animationController;
    private Rigidbody rb;
    private NavMeshAgent agent;
    private BasicPlayerController target;
    private AudioSource audioSource;
    private bool isPlayerVisible;
    
    public AudioClip hitSound;
    public AudioClip dieSound;
    public float attackDamage = 10f;
    public float attackDistance = 2f;
    private float attackCooldown = .5f;
    private float nextAttackTime;
    public float maxHealth = 100f;
    private float currentHealth;
    public bool isDead;
    private bool isMoving;
    private Vector3 lastTargetPosition;
    private Coroutine stopZombieRoutine;
    
    private void Awake()
    {
        animationController = GetComponent<ZombieAnimationController>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        lastTargetPosition = transform.position;
    }

    private void Start()
    {
        agent.speed = movementSpeed;
        agent.angularSpeed = rotationSpeed;
        
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isDead) return;
        
        CheckVision();
        
        if (isPlayerVisible)
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= attackDistance && Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + 1f / attackCooldown;
                Attack();
            }
            else
            {
                MoveTowardsTarget();
            }
        }
        else
        {
            Wander();
        }
    }
    
    private int rayCount = 18;
    private float detectionAngle = 360f;
    private float detectionDistance = 15f;
    private void CheckVision()
    {
        var rays = RaycastUtils.GetViewRays(transform, rayCount, detectionAngle);
        
        foreach (var ray in rays)
        {
            if (!Physics.Raycast(ray, out var hit, detectionDistance)) continue;
            if (!hit.collider.CompareTag("Player")) continue;
            
            isPlayerVisible = true;
            target = hit.collider.transform.GetComponent<BasicPlayerController>();
            return;
        }
    }
    private void Attack()
    {
        // Stop moving
        isMoving = false;
        agent.isStopped = true;
        animationController.SetAttacking();
        target.TakeDamage(attackDamage);
    }
    private void MoveTowardsTarget()
    {
        if (!isMoving)
        {
            animationController.SetMoving();
            isMoving = true;
        }
        
        agent.SetDestination(target.transform.position);
    }
    private void Wander()
    {
        // Choose random position to move in a radius, but the random should be in front of the zombie
        if (Vector3.Distance(transform.position, lastTargetPosition) <= 1)
        {
            if (!isMoving)
            {
                animationController.SetMoving();
                isMoving = true;
            }
            var randomDirection = UnityEngine.Random.insideUnitCircle * 5;
            var randomPosition = new Vector3(randomDirection.x, 0, randomDirection.y);
            var finalPosition = transform.position + transform.TransformDirection(randomPosition);
            agent.SetDestination(finalPosition);
            lastTargetPosition = finalPosition;
        }
    }
    public void TakeDamage(RaycastHit hit, float damage)
    {
        // Spawn blood effect
        if (bloodEffect)
        {
            var blood = Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(blood.gameObject, blood.main.duration);   
        }
        
        if (isDead) return;
        
        animationController.SetHitReaction();
        if (stopZombieRoutine != null)
        {
            StopCoroutine(stopZombieRoutine);
        }
        stopZombieRoutine = StartCoroutine(StopZombieForSeconds(.75f));
        
        
        // Apply damage to zombie
        currentHealth -= damage;
        isDead = currentHealth <= 0;
        
        if (isDead)
        {
            Die();
        }
        else
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    private IEnumerator StopZombieForSeconds(float second)
    {
        agent.speed = 0;
        yield return new WaitForSeconds(second);
        agent.speed = movementSpeed;
    }
    
    private void Die()
    {
        // Play death animation
        animationController.SetDying();
        
        audioSource.PlayOneShot(dieSound);
        
        // Disable movement
        agent.isStopped = true;
        
        SetLayerAllChildren();
        
        GameManager.Instance.score += 10;
        
        Destroy(gameObject, 5f);
    }
    private void SetLayerAllChildren()
    {
        var children = transform.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children)
        {
            child.gameObject.layer = LayerMask.NameToLayer(enemyDeadLayer);
        }
    }
}
