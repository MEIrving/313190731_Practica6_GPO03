using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWay : MonoBehaviour
{
    public WayCreator[] pathFollow;
    public int currentWayPointID;
    public float rotSpeed;
    public float speed;

    public float reachDistance = 0.1f;
    public int way = 0;

    public ParticleSystem efectoDespegue;
    private bool yaDespego = false;
    private float tiempoDespegue = 0f;

    private bool cayendo = false;
    private bool colisiono = false;

    void Start()
    {
        way = 0;
        currentWayPointID = 0;
        speed = 0;

        if (efectoDespegue != null)
            efectoDespegue.Stop();
    }

    void Update()
    {
        // Iniciar despegue al primer toque
        if (!yaDespego && (Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            yaDespego = true;
            tiempoDespegue = 0f;

            if (efectoDespegue != null)
                efectoDespegue.Play();
        }

        // Aumentar velocidad suavemente
        if (yaDespego && speed < 0.3f && !cayendo)
        {
            tiempoDespegue += Time.deltaTime;
            speed = Mathf.Lerp(0f, 0.3f, tiempoDespegue / 2f);
        }

        // Aceleración extra con dos dedos
        if (Input.touchCount > 1 && yaDespego && !cayendo)
        {
            speed = 0.7f;
        }

        // Movimiento normal por rutas
        if (yaDespego && !cayendo)
        {
            if (pathFollow.Length == 0 || way >= pathFollow.Length) return;

            Vector3 objetivo = pathFollow[way].path_objs[currentWayPointID].position;
            float distance = Vector3.Distance(objetivo, transform.position);
            transform.position = Vector3.MoveTowards(transform.position, objetivo, Time.deltaTime * speed);

            // Mantener horizontal
            Vector3 direccion = objetivo - transform.position;
            direccion.y = 0;

            if (direccion == Vector3.zero) direccion = transform.forward;

            Quaternion rotacion = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacion, Time.deltaTime * rotSpeed);

            if (distance <= reachDistance)
                currentWayPointID++;

            if (currentWayPointID >= pathFollow[way].path_objs.Count)
            {
                currentWayPointID = 0;

                if (way == 0)
                {
                    way = Random.Range(1, 4);
                    speed = 0.3f;
                }
            }
        }

        // Efecto de caída: rotación descontrolada
        if (cayendo)
        {
            // Inclinación hacia abajo + giro
            transform.Rotate(new Vector3(120f, 0f, 100f) * Time.deltaTime);
        }
    }

    public void ActivarRutaCaida()
    {
        speed = 0;
        cayendo = true;

        // Activar física real
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (cayendo && !colisiono && collision.gameObject.CompareTag("Suelo"))
        {
            colisiono = true;

            TouchInteraction ti = GetComponent<TouchInteraction>();
            if (ti != null)
                ti.ExplotaEnImpacto();
        }
    }
}
