using System.Collections;
using UnityEngine;

public class TouchInteraction : MonoBehaviour
{
    private MoveWay avion;
    private int toques = 0;
    public float aumentoVelocidad = 0.3f;

    public GameObject efectoExplosion;
    public Material dissolveMaterial;
    private bool cayendo = false;

    void Start()
    {
        avion = GetComponent<MoveWay>();
        if (efectoExplosion != null)
            efectoExplosion.SetActive(false);
    }

    void OnMouseDown()
    {
        if (cayendo || avion == null) return;

        toques++;
        StartCoroutine(Tambalear());

        avion.speed += aumentoVelocidad;

        if (toques >= 3)
        {
            StartCoroutine(Caer());
        }
    }

    IEnumerator Tambalear()
    {
        float duracion = 0.4f;
        float angulo = 15f;
        float tiempo = 0;

        while (tiempo < duracion)
        {
            float rot = Mathf.Sin(tiempo * 15f) * angulo;
            transform.localRotation = Quaternion.Euler(0, 0, rot);
            tiempo += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = Quaternion.identity;
    }

    IEnumerator Caer()
    {
        cayendo = true;
        avion.ActivarRutaCaida();
        yield break;
    }

    public void ExplotaEnImpacto()
    {
        StartCoroutine(ExplotaTrasImpacto());
    }

    IEnumerator ExplotaTrasImpacto()
    {
        if (dissolveMaterial != null)
        {
            MeshRenderer render = GetComponent<MeshRenderer>();
            if (render != null)
                render.material = dissolveMaterial;
        }

        float dis = 0f;
        while (dis < 1f)
        {
            dissolveMaterial.SetFloat("_DissolveAmount", dis);
            dis += Time.deltaTime * 0.5f;
            yield return null;
        }

        if (efectoExplosion != null)
        {
            efectoExplosion.transform.position = transform.position;
            efectoExplosion.SetActive(true);

            AudioSource audio = efectoExplosion.GetComponent<AudioSource>();
            if (audio != null && !audio.isPlaying)
                audio.Play();

            foreach (ParticleSystem ps in efectoExplosion.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Play();
            }
        }

        gameObject.SetActive(false);
    }
}
