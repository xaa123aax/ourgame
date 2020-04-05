using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamChange : MonoBehaviour
{
    public int BluePlayerpumber = 0;
    public int RedPlayerpumber = 0;





    private void OnTriggerEnter(Collider other)
    {

        if (gameObject.tag == "RedTeam")
        {
            if (other.CompareTag("BluePlayer"))
            {
                {
                    RedPlayerpumber++;
                    BluePlayerpumber--;
                }

            }
            if (other.CompareTag("Player"))
            {
                {
                    RedPlayerpumber++;
                }

            }
        }
        else if (gameObject.tag == "BlueTeam")
        {
            if (other.CompareTag("RedPlayer"))
            {
                {
                    RedPlayerpumber--;
                    BluePlayerpumber++;
                }

            }
            if (other.CompareTag("Player"))
            {
                {
                    BluePlayerpumber++;
                }

            }
        }
        else if (gameObject.tag == "whiteteam")
        {


            if (other.CompareTag("BluePlayer"))
            {
                {
                    BluePlayerpumber--;
                }
            }

            if (other.CompareTag("RedPlayer"))
            {
                {
                    RedPlayerpumber--;
                }

            }

        }
    }

}



