using UnityEngine;
using UnityEngine.AI;

public class DroneAI : MonoBehaviour
{
    public NavMeshAgent Agent;
    public float DetectionRadius = 10f;
    public float ChaseRadius = 15f;
    public float AttackRadius = 5f;
    public float MaxHealth = 100f;
    public float Health;
    public float projectileSpeed = 20f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public ParticleSystem deathEffect;
    public Transform Player;

    private IDroneStrategy currentStrategy;

    private void Start()
    {
        // Находим игрока по тегу "Player"
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            Player = playerObject.transform; // Присваиваем ссылку на игрока
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player has the tag 'Player'.");
        }

        Health = MaxHealth;
        SetStrategy(new PatrolStrategy());
    }

    private void Update()
    {

        if (Health <= 0)
        {
            Die(); 
            return;
        }

        currentStrategy.Execute(this); // Выполняем текущую стратегию

        // Логика смены стратегий в зависимости от игрового состояния
        if (Health <= MaxHealth * 0.15f) 
        {
            SetStrategy(new FleeStrategy());
        }
        else if (PlayerInAttackRange())
        {
            if (!(currentStrategy is AttackStrategy)) 
            {
                SetStrategy(new AttackStrategy());
            }
        }
        else if (PlayerInChaseRange())
        {
            if (!(currentStrategy is ChaseStrategy)) 
            {
                SetStrategy(new ChaseStrategy());
            }
        }
        else
        {
            if (!(currentStrategy is PatrolStrategy)) 
            {
                SetStrategy(new PatrolStrategy());
            }
        }
    }

    public void SetStrategy(IDroneStrategy newStrategy)
    {
        currentStrategy = newStrategy;
    }

    public void TakeDamage(float damage)
    {
        Health -= damage; 
        Health = Mathf.Clamp(Health, 0, MaxHealth); 

        Debug.Log($"Drone took {damage} damage! Current health: {Health}");
    }

    private void Die()
    {


        if (deathEffect != null)
        {
            deathEffect.Play();
        }


        Destroy(gameObject, 0.3f);
    }

    public bool PlayerInDetectionRange()
    {
        return Vector3.Distance(transform.position, Player.position) <= DetectionRadius;
    }

    public bool PlayerInChaseRange()
    {
        return Vector3.Distance(transform.position, Player.position) <= ChaseRadius;
    }

    public bool PlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, Player.position) <= AttackRadius;
    }

    public void LookAtPlayer()
    {
        Vector3 lookDirection = Player.position - transform.position;
        lookDirection.y = 0; // Оставляем только поворот по оси Y
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    public void MaintainHeight()
    {
        Vector3 newPosition = transform.position;
        newPosition.y = 3f; // Устанавливаем высоту в 3 метра
        transform.position = newPosition;
    }

    public void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        Vector3 direction = (Player.position - firePoint.position).normalized;
        rb.velocity = direction * projectileSpeed;

        Debug.Log("Drone is shooting a projectile at the player!");
    }

    public void StartHealing(float healRate)
    {
        Health += healRate * Time.deltaTime;
        Health = Mathf.Clamp(Health, 0, MaxHealth); // Ограничиваем здоровье
    }
}