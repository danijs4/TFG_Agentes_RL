using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

//////////////////////////////////////VERSION ANTIGUA////////////////////////////////////////

public class AgenteN1 : Agent
{

    private float jumpForce = 5f;
    private Rigidbody rb;
    private bool isGrounded = true;
    private int placasPisadas = 0;
    private float levelTimeLimit = 20f;
    private float timeRemaining;
    private float lastDistanceToGoal;

    [SerializeField] private Transform puertalvl1Transform;
    [SerializeField] private MeshRenderer placaMeshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        
    }

    public override void OnEpisodeBegin()   //resetear posiciones de agente y puerta y colores de placas
    {
        float randomX = Random.Range(-3.5f, 3.5f);
        transform.localPosition = new Vector3(randomX, 0.5f, -5);
        transform.rotation = Quaternion.Euler(0, -90, 0);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        puertalvl1Transform.localPosition = new Vector3(-0.05f , 1.3f, 6.535f);
        placaMeshRenderer.material.color = Color.red;
        placasPisadas = 0;

        timeRemaining = levelTimeLimit;

        lastDistanceToGoal = Vector3.Distance(transform.localPosition, puertalvl1Transform.localPosition);


    }

    // Update is called once per frame
    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            
            EndEpisode();
        }
    }

    //se llama menos veces mas eficiente
    private void FixedUpdate()
    {
        if (transform.localPosition.y < -4f)
        {
            AddReward(-5f);
            EndEpisode(); 
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    public override void CollectObservations(VectorSensor sensor)   //observaciones con las que el agente toma decisiones
    {/*
        //posicion en x,y,z
        sensor.AddObservation(transform.localPosition);

        //rotacion eje Y 
        sensor.AddObservation(transform.rotation.eulerAngles.y);

        //distancia a la meta
        float distanciaMeta = Vector3.Distance(puertalvl1Transform.localPosition, transform.localPosition);
        sensor.AddObservation(distanciaMeta);

        // velocidad actual
        //sensor.AddObservation(rb.velocity);

        // Si está en el suelo
        sensor.AddObservation(isGrounded ? 1f : 0f);
        */

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        if (collision.gameObject.CompareTag("Placa"))
        {
            isGrounded = true;
        }
    }



    public override void OnActionReceived(ActionBuffers actions)
    {

        // Verificar si el agente está volcado
        if (Vector3.Dot(transform.up, Vector3.up) < 0.9f)  // 0.9f es el umbral de tolerancia
        {
            // Si no está de pie, no permitimos el movimiento
            return;
        }

        float moveForward = actions.ContinuousActions[0];
        float rotateY = actions.ContinuousActions[1];

        float moveJump = actions.ContinuousActions[2];
        
        //movimiento hacia delante o atras

        float moveSpeed = 2.5f;
        rb.MovePosition(transform.position + transform.right * moveForward * moveSpeed * Time.deltaTime);

        //rotacion respecto a Y

        transform.Rotate(0f, rotateY * moveSpeed, 0f, Space.Self);

        //saltar
        if (isGrounded && moveJump >= 0.5f) 
        {

            Jump();

            // Aplica un movimiento hacia adelante durante el salto con una fuerza proporcional al movimiento hacia adelante
            float forwardSpeed = moveForward * 2.0f; 
            rb.AddForce(transform.right * forwardSpeed, ForceMode.Impulse); // Movimiento hacia adelante mientras saltas
        }
        /*
        float distanciaActual = Vector3.Distance(transform.localPosition, puertalvl1Transform.localPosition);
        float progreso = lastDistanceToGoal - distanciaActual; // Diferencia de distancia

        if (progreso > 0)
        {
            AddReward(0.05f * progreso);  // Recompensa positiva si se acerca
        }
        else
        {
            AddReward(-0.02f * Mathf.Abs(progreso)); // Penaliza si se aleja
        }

        lastDistanceToGoal = distanciaActual; // Actualiza la distancia anterior*/

    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Meta"))
        {
            AddReward(15f);
            EndEpisode();
        }
        if (collider.gameObject.CompareTag("Pared"))
        {
            AddReward(-0.1f);
            
        }
        if (collider.gameObject.CompareTag("Placa"))
        {
            //MeshRenderer placaMeshRenderer = collider.GetComponent<MeshRenderer>();

            if (placaMeshRenderer != null)
            {
                if (placaMeshRenderer.material.color != Color.green)  //si no es verde es que no se ha pisado antes
                {
                    AddReward(4f);

                    placaMeshRenderer.material.color = Color.green;
                    placasPisadas++;
                    if (placasPisadas == 1) //cambiar segun numero de placas
                    {
                        puertalvl1Transform.localPosition += new Vector3(0, 3, 0);  //abrir puerta
                    }
                }
                else
                {
                    //restar recompensa porque vuelve a placas pisadas
                    AddReward(-1f);
                }


            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        continuousActions[0] = Input.GetAxisRaw("Vertical");
        continuousActions[1] = Input.GetAxisRaw("Horizontal");
        //continuousActions[2] = Input.GetKey(KeyCode.LeftArrow) ? -1f :
             //                   Input.GetKey(KeyCode.RightArrow) ? 1f : 0f;
        //discreteActions[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
        

    }


    

    
}
