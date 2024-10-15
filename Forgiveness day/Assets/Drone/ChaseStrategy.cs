public class ChaseStrategy : IDroneStrategy
{
    public void Execute(DroneAI drone)
    {
        drone.Agent.SetDestination(drone.Player.position);
        drone.LookAtPlayer();
        drone.MaintainHeight();
    }
}