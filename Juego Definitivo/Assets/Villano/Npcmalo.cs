using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npcmalo : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    float limite_caminata_izq;
    float limite_caminata_der;
    public float velocidad_caminata= 6f;
    int direccion = 1;
    enum Tipo_comportamiento_npcmalo { pasivo, persecucion, ataque };
    Tipo_comportamiento_npcmalo Comportamiento = Tipo_comportamiento_npcmalo.pasivo;
    float entradazonapersecucion = 15f;
    float salidazonapersecucion = 25f;
    float distanciaataque = 2f;
    float distancia_con_prota;
    public Transform prota;
    bool golpe_valido = false;
    // Start is called before the first frame update
    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        limite_caminata_izq = transform.position.x + GetComponent<CircleCollider2D>().radius;
        limite_caminata_der = transform.position.x - GetComponent<CircleCollider2D>().radius;
        anim = transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    public void Update()
    {
        distancia_con_prota =Mathf.Abs (prota.position.x - transform.position.x);
        switch (Comportamiento)
        {
            case Tipo_comportamiento_npcmalo.pasivo:
                // Desplazarse (caminando)
                rb.velocity = new Vector2(velocidad_caminata * direccion, rb.velocity.y);
                // Girarse segun los limites de caminata
                if (transform.position.x > limite_caminata_izq) direccion = -1;
                if (transform.position.x < limite_caminata_der) direccion = 1;
                // Velocidad del animator
                anim.speed = 1f;
                // Entrar en zona de persecucion
                if (distancia_con_prota < entradazonapersecucion) Comportamiento = Tipo_comportamiento_npcmalo.persecucion;
                
                break;
            case Tipo_comportamiento_npcmalo.persecucion:
                // Desplazarse (caminando)
                rb.velocity = new Vector2(velocidad_caminata * 1.2f * direccion, rb.velocity.y);
                // Girarse segun los limites de caminata
                if (prota.position.x > transform.position.x) direccion = 1;
                if (prota.position.x > transform.position.x) direccion = -1;
                // Velocidad del animator
                anim.speed= 1.5f;
                // Volver a la zona pasiva
                if (distancia_con_prota > salidazonapersecucion) Comportamiento = Tipo_comportamiento_npcmalo.pasivo;
                // Entre a la zona de ataque
                if (distancia_con_prota < distanciaataque) Comportamiento = Tipo_comportamiento_npcmalo.ataque;
                break;
            case Tipo_comportamiento_npcmalo.ataque:
                anim.SetTrigger("atacar!");
                // Girarse segun la posicion del prota
                if (prota.position.x > limite_caminata_izq) direccion = 1;
                if (prota.position.x < limite_caminata_der) direccion = -1;
                // Velocidad del animator
                anim.speed = 4f;

                // volver a la zona de persecución
                if (distancia_con_prota > distanciaataque)
                {
                    Comportamiento = Tipo_comportamiento_npcmalo.persecucion;
                    anim.ResetTrigger("atacar!");
                }
                transform.localScale = new Vector3(1.3f * direccion, 1.3f, 1f);
                break;
        }
        
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && golpe_valido)
        {
            golpe_valido = false;
            prota.GetComponent<protagonista>().recibir_golpe(collision.contacts[0].point);
        }
    }

    public void Golpevalido_inicio()
    {
        golpe_valido = true;
    }

    public void Golpe_fin()
    {
        golpe_valido = false;
    }
}
