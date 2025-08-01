using UnityEngine;

public class HealthSys : MonoBehaviour
{
    [SerializeField] public int currHealth;
    [SerializeField] public int maxHealth;


    public void changeHealth(int changeAmount)
    {
        currHealth += changeAmount;
        
        if(currHealth > maxHealth) currHealth = maxHealth;
        
        if(currHealth < 0 ) gameObject.SetActive(false);
    }
}
