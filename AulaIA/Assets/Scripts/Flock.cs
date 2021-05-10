using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockManager myManager;     //chama o flockmanager
    public float speed;                //configura a velocidade dos peixes
    bool turning = false;


    void Start()
    {
        speed = Random.Range(myManager.minSpeed, myManager.maxSpeed);    //Importa as configurações do manager para a velocidade  
    }


    void Update()
    {
        Bounds b = new Bounds(myManager.transform.position, myManager.swinLimits * 2); 

        RaycastHit hit = new RaycastHit();                                             //cria um raycast para os peixes
        Vector3 direction = myManager.transform.position - transform.position;         
        
        if(!b.Contains(transform.position))                                                 //faz com que os peixes n colidam
        {
            turning = true;
            direction = myManager.transform.position - transform.position;
        }
        else if(Physics.Raycast(transform.position, this.transform.forward * 50, out hit))
        {
            turning = true;
            direction = Vector3.Reflect(this.transform.forward, hit.normal);
        }
        else
            turning = false;

        if(turning)       
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), myManager.rotationSpeed * Time.deltaTime);
        }
        else
        {
            if (Random.Range(0, 100) < 10)      //gera uma velocidade de rotação para que o peixe retorne ao grupo
                speed = Random.Range(myManager.minSpeed, myManager.maxSpeed);
            if(Random.Range(0, 100)< 20) 
                ApplyRules();
        }
       
        transform.Translate(0, 0, Time.deltaTime * speed);        //movimento dos peixes
    }

    void ApplyRules()
    {
        GameObject[] gos;
        gos = myManager.allFish;           //importa as informações do allfish

        Vector3 vcentro = Vector3.zero;    //encontra o ponto central dos peixes
        Vector3 vavoid = Vector3.zero;     //anula colisão
        float gSpeed = 0.01f;              //faz com que movimente
        float nDistance;                   //checa a distância entre eles
        int groupSize = 0;                 //se o distanciamento entre eles for grande, gera outro grupo
   
        foreach(GameObject go in gos)      
        {
            if(go != this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                if(nDistance <= myManager.neighbourDistance)
                {
                    vcentro += go.transform.position;
                    groupSize++;

                    if(nDistance < 1.0)
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }

                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlock.speed;

                }
            }
        }
        if(groupSize>0)                     //aplica rotações e verifica se o grupo é maior que zero
        {
            vcentro = vcentro / groupSize + (myManager.goalPos - this.transform.position);
            speed = gSpeed / groupSize;

            Vector3 direction = (vcentro + vavoid) - transform.position;
            if(direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), myManager.rotationSpeed * Time.deltaTime);
            }


        }
    
    }
}

