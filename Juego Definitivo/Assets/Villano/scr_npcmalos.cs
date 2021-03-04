using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_npcmalos : MonoBehaviour
{
    Rigidbody2D rb;

    float limiteCaminataIzq;
    float limiteCaminataDer;

    public float velCaminata = 6f;
    int direccion = 1;

    enum tipoComportamientoNpc {pasivo,persecucion,ataque }
    tipoComportamientoNpc comportamiento = tipoComportamientoNpc.pasivo;

    float entradaZonaPersecucion = 15f;
    float salidaZonaPersecucion =25f;
    float distanciaAtaque = 2f;

    float distanciaConProta;
    public Transform Prota;
    public float umbralVelocidad;
    public Transform refPiso;

    Animator anim;

    public float vida_npc = 3;

    bool GolpeValido = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        limiteCaminataIzq = transform.position.x - GetComponent<CircleCollider2D>().radius;
        limiteCaminataDer = transform.position.x + GetComponent<CircleCollider2D>().radius;
        anim = transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        distanciaConProta = Mathf.Abs(Prota.position.x - transform.position.x);
        bool pocadistanciaVertical = Mathf.Abs(Prota.position.y - transform.position.y) < 10f;
        bool enPiso = Physics2D.OverlapCircle(refPiso.position, 1f, 1 << 8);
        if (rb.velocity.magnitude< umbralVelocidad)
        {

        
        switch (comportamiento)
        {
            case tipoComportamientoNpc.pasivo:
                // Desplazarse (caminando)
                rb.velocity = new Vector2(velCaminata * direccion, rb.velocity.y);
                // Girarse segun los limites de la caminata
                if (transform.position.x < limiteCaminataIzq) direccion = 1;
                if (transform.position.x > limiteCaminataDer) direccion = -1;
                anim.speed = 1f;

                // Entrar zona de persecucion
                if (distanciaConProta < entradaZonaPersecucion && pocadistanciaVertical==true) comportamiento = tipoComportamientoNpc.persecucion;
                break;
            case tipoComportamientoNpc.persecucion:
                
                // Desplazarse (corriendo)
                rb.velocity = new Vector2(velCaminata * 1.2f * direccion, rb.velocity.y);
                
                // Girarse segun los limites de la caminata
                if (Prota.position.x > transform.position.x) direccion = 1;
                if (Prota.position.x < transform.position.x) direccion = -1;

                // Velocidad del animator
                anim.speed = 3f;
                // Volver a la  zona pasiva
                if (distanciaConProta > salidaZonaPersecucion || pocadistanciaVertical==false) comportamiento = tipoComportamientoNpc.pasivo;

                // Entre a la zona de ataque 
                if (distanciaConProta < distanciaAtaque) comportamiento = tipoComportamientoNpc.ataque;
                break;
            case tipoComportamientoNpc.ataque:
                anim.SetTrigger("atacar!");

                // Girarse segun la posicion del Prota
                if (Prota.position.x > transform.position.x) direccion = 1;
                if (Prota.position.x < transform.position.x) direccion = -1;

                // Velocidad del animator
                anim.speed = 1f;

                // Volver a la  zona de persecucion
                if (distanciaConProta > distanciaAtaque)
                {
                    comportamiento = tipoComportamientoNpc.persecucion;
                    anim.ResetTrigger("atacar!");
                }
                break;
        }
            // Si no hay piso poner velocidad en 0
            if (!enPiso) rb.velocity = new Vector3(0, rb.velocity.y);
        }
        transform.localScale = new Vector3(1.3f * direccion, 1.3f, 1);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && GolpeValido)
        {
            GolpeValido = false;
            Prota.GetComponent<New_Prota>().recibir_golpe(collision.contacts[0].point);
        }
    }

    public void morir()
    {
        vida_npc -= 1;
        if (vida_npc <= 0) {
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }

    public void golpeValido_inicio()
    {
       GolpeValido = true;
    }

    public void golpeValido_fin()
    {
        GolpeValido = false;
    }
}
