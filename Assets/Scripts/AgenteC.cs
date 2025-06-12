using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgenteC : Agent
{

    private float jumpForce = 6f;
    private Rigidbody rb;
    private bool isGrounded = true;
    private int placasPisadas = 0;
    private float levelTimeLimit = 20f;
    private float timeRemaining;
    private float lastDistanceToGoal;
    private int numExitos = 0;
    private int numMaxExitos = 50;

    [SerializeField] private Transform puertalvl1Transform;           // Posicion de la puerta
    [SerializeField] private List<MeshRenderer> placasMeshRenderers;  // Lista de placas


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    //Método llamado cada vez que inicia un episodio.
    public override void OnEpisodeBegin()   //resetear posiciones de agente y puerta y colores de placas
    {

        if (numExitos >= numMaxExitos)     //detener el entrenamiento tras x exitos (para tener una condición con la que dar un nivel por aprendido)
        {

            // Detener la ejecución en el editor de Unity
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }


        float randomX = Random.Range(-3.5f, 3.5f);
        transform.localPosition = new Vector3(randomX, 0.5f, -5);
        transform.rotation = Quaternion.Euler(0, -90, 0);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //Nivel 1-4 descomentar la siguiente linea
        puertalvl1Transform.localPosition = new Vector3(-0.05f, 1.3f, 6.535f);
        //Nivel 5 descomentar la siguiente linea
        //puertalvl1Transform.localPosition = new Vector3(-5.18f, 7.12f, -6.47f);


        //restaurar todas las placas a color rojo
        foreach (MeshRenderer placa in placasMeshRenderers)
        {
            if (placa != null)
            {
                placa.material.color = Color.red;
            }
        }

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

    //Método propio para añadir la fuerza que provoca el salto
    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    //Método que recoge las observaciones que se pasan a la red en un vector
    public override void CollectObservations(VectorSensor sensor)
    {

    }

    //Método llamado cuando hay colisión física. Usado para determinar si el agente está en el suelo
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


    //Método con el que se da lógica a las acciones. Tomamos los números del vector y decidimos que significa cada uno
    public override void OnActionReceived(ActionBuffers actions)
    {

        // Verificar si el agente está volcado
        if (Vector3.Dot(transform.up, Vector3.up) < 0.9f)  // 0.9f es el umbral de tolerancia
        {
            // Si no está de pie, no permitimos el movimiento
            return;
        }

        AddReward(-0.0005f);  //Este agente es penalizado por cada acción realizada

        float moveForward = actions.ContinuousActions[0];
        float rotateY = actions.ContinuousActions[1];
        float moveJump = actions.ContinuousActions[2];

        //movimiento hacia delante o atras
        float moveSpeed = 2.5f;
        rb.MovePosition(transform.position + transform.right * moveForward * moveSpeed * Time.deltaTime);

        //rotacion respecto a Y
        transform.Rotate(0f, rotateY * moveSpeed, 0f, Space.Self);

        //saltar
        if (isGrounded && moveJump >= 0.2f)
        {

            Jump();

            // Aplica un movimiento hacia adelante durante el salto con una fuerza proporcional al movimiento hacia adelante
            float forwardSpeed = 3f;
            rb.AddForce(transform.right * forwardSpeed, ForceMode.Impulse); // Movimiento hacia adelante mientras saltas
        }
        
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

    //Método llamado cuando hay colisión con un target. Es con el que determinamos la mayoría de las recompensas
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Meta"))
        {
            AddReward(15f);
            numExitos++;
            EndEpisode();
        }
        if (collider.gameObject.CompareTag("Pared"))
        {
            AddReward(-0.1f);

        }
        if (collider.gameObject.CompareTag("Placa"))
        {
            MeshRenderer placaMeshRenderer = collider.GetComponent<MeshRenderer>();

            if (placaMeshRenderer != null)
            {
                if (placaMeshRenderer.material.color != Color.green)  //si no es verde es que no se ha pisado antes
                {
                    AddReward(4f);

                    placaMeshRenderer.material.color = Color.green;
                    placasPisadas++;
                    if (placasPisadas == placasMeshRenderers.Count)
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

    //Método para mover el agente mediante el teclado. Usado para comprobar el movimiento antes de entrenar
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        continuousActions[0] = Input.GetAxisRaw("Vertical");
        continuousActions[1] = Input.GetAxisRaw("Horizontal");
        //continuousActions[2] = Input.GetKey(KeyCode.LeftArrow) ? -1f :
        //                   Input.GetKey(KeyCode.RightArrow) ? 1f : 0f;
        continuousActions[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;


    }





}