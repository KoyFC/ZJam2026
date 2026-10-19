using System.Collections;
using UnityEngine;

public class ShieldBoss : MonoBehaviour
{
    [SerializeField] private GameObject m_Shield;
    private bool m_IsShieldActive = true;

    void Start()
    {
        m_Shield.SetActive(true);
        StartCoroutine(changeShieldState());

    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    IEnumerator changeShieldState()
    {
        while (true)
        {
            if (m_IsShieldActive)
            {
                m_Shield.SetActive(false);
                m_IsShieldActive = false;
                yield return new WaitForSeconds(5f);
            }
            else
            {
                m_Shield.SetActive(true);
                m_IsShieldActive = true;
                yield return new WaitForSeconds(5f);
            }
            

        }

       

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(m_IsShieldActive)
        {
            if(collision.gameObject.CompareTag("PlayerMissile"))
            {
                collision.gameObject.transform.position += collision.gameObject.GetComponent<Projectile>().Speed * Time.deltaTime * -transform.forward;
            }
        }
    }


}
