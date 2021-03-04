using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class protagonista : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb;
    public float fuerzaSalto;
    public bool piso;
    public Transform refPie;
    public float velx = 10f;
    public GameObject Particulas_sangre;

    // Energia
    int Energia_Max = 6;
    int Energia_Actual;

    public Image mascara_daño;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        Energia_Actual = Energia_Max;
    }

    // Update is called once per frame
    void Update()
    {
        float movX;
        movX =Input.GetAxis("Horizontal");
        anim.SetFloat("mov", Mathf.Abs(movX));
        rb.velocity = new Vector2(velx * movX, rb.velocity.y);
        
        piso = Physics2D.OverlapCircle(refPie.position, 5f, 1 << 8);
        anim.SetBool("enpiso", piso);

        if (Input.GetButtonDown("Jump") && piso)
        {

            rb.AddForce(new Vector2(0, fuerzaSalto), ForceMode2D.Impulse);

        }

        if(movX < 0) transform.localScale = new Vector3(-1, 1, 1);
        if(movX > 0) transform.localScale = new Vector3(1, 1, 1);

    }

     private void FixedUpdate()
     {

         Vector3 puntoInicial = Camera.main.transform.position;
         Vector3 puntoFinal = transform.position - new Vector3(0, 0, 20);

         Camera.main.transform.position = Vector3.Lerp(puntoInicial, puntoFinal, .035f);
        
        float valorAlfa = 1 / Energia_Max * (Energia_Max-Energia_Actual);
        mascara_daño.color = new Color(1, 1, 1, valorAlfa);
     }

    public void recibir_golpe(Vector2 posicion)
    {
        // reducir la energia
        Energia_Actual -= 1;
        if (Energia_Actual <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("auch!" + Energia_Actual);
            // Particulas de sangre
            Instantiate(Particulas_sangre, posicion, Quaternion.identity);
            // Disparar la animacion
            anim.SetTrigger("auch!");
        }
       
        
    }
 }
