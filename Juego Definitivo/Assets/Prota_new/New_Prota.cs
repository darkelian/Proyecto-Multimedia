using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class New_Prota : MonoBehaviour
{ 
    Animator anim;
    Rigidbody2D rb;
    public bool enPiso;
    public Transform refPie;
    public float fuerzaSalto;
    public float velX;

    public Transform contArma;
    bool tieneArma;
    public Transform mira;
    public Transform refManoArma;
    public Transform refOjos;
    public Transform cabeza;
    public float magnitudPateoArma = 300f;
    public float magnitudReaccionDispararo=300f;
    public GameObject particulasArma;
    public Transform puntaArma;

    // Sacudir camara
    public Transform camaraSacudir;
    public GameObject Sangre_npc;
    public GameObject Particulas_sangre;
    // Energia
    int Energia_Max = 5;
    int Energia_Actual;
    // Mascara daño
    public Image mascara_daño;
    // Barra de vida
    public Image Barra_6;
    public Image Barra_5;
    public Image Barra_4;
    public Image Barra_3;
    public Image Barra_2;
    public Image Barra_1;
    // Barra runas
    public Image Runa1;
    public Image Runa2; 
    public Image Runa3;
    public Image Runa4;
    public Image Runa5;
    //contador runas
    int contador_runa=0;
    // contador balas
    public TMPro.TextMeshProUGUI textoContadorBalas;
    int cantidadBalas;

    // Capa de game over
    public Image Tela;
    float valorAlfaTela;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        Energia_Actual = Energia_Max;
        // Fade In inicial
        Tela.color = new Color(0, 0, 0, 1);
        valorAlfaTela = 0;
        if (Partida.hayPartida) cargarPartida();
        if(Partida.infoProta.arma == true)
        {
            contArma.gameObject.SetActive(true);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Energia_Actual <= 0) return;
        float movX;
        movX = Input.GetAxis("Horizontal");
        anim.SetFloat("absMovX",Mathf.Abs(movX));
        rb.velocity = new Vector2(velX * movX, rb.velocity.y);
        enPiso = Physics2D.OverlapCircle(refPie.position, 1f, 1 << 8);
        anim.SetBool("enPiso", enPiso);
        
        if (Input.GetButton("Jump") && enPiso)
        {
            rb.AddForce(new Vector2(0, fuerzaSalto), ForceMode2D.Impulse);
        }

        // Girarse 
        if (tieneArma)
        {
            if (mira.transform.position.x < transform.position.x) transform.localScale = new Vector3(-1, 1, 1);
            if (mira.transform.position.x > transform.position.x) transform.localScale = new Vector3(1, 1, 1);

        }
        else
        {
            if (movX < 0) transform.localScale = new Vector3(-1, 1, 1);
            if (movX > 0) transform.localScale = new Vector3(1, 1, 1);

        }
        if (tieneArma)
        {
            //Detertar el mouse y colocar mira
            mira.position = Camera.main.ScreenToWorldPoint(new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                -Camera.main.transform.position.z));
            refManoArma.position = mira.position;

            if (Input.GetButtonDown("Fire1"))
            {
                if(cantidadBalas>0) disparar();
                else
                {
                    // Avisar que no hay balas
                    textoContadorBalas.color = Color.red;
                    textoContadorBalas.fontSize = 50;
                }
            }
           
        }

        // Escena muerte
        if (contador_runa == 3)
        {
            cargarNivelBoss();
        }
        if (Input.GetKeyDown(KeyCode.P)) cargarNivelBoss();
        if (Input.GetKeyDown(KeyCode.O)) cargarPartida();

        // se muestre la cantidad de balas
        textoContadorBalas.text = cantidadBalas.ToString();
    }

    void guardarPartida()
    {
        Partida.infoProta.cantidadBalas= cantidadBalas;
        Partida.infoProta.cantRunas = contador_runa;
        Partida.infoProta.vida = Energia_Actual;
        Partida.infoProta.posicion = transform.position;
        Partida.infoProta.arma = tieneArma;
        
        Partida.hayPartida = true;
        // Guardar el estado de cada paquete de balas en la lista llamada infoBalas
        Partida.infoPaqueteBalas.Clear();
        Transform todosLosPaquetes = GameObject.Find("Paquete Balas").transform;
        foreach(Transform paquete in todosLosPaquetes)
        {
            Partida.tipoPaqueteBalas iTemPaquete = new Partida.tipoPaqueteBalas
            {
                activo = paquete.gameObject.activeSelf
            };
            Partida.infoPaqueteBalas.Add(iTemPaquete);
        }
        // Guardar el estado de cada paquete de vidas en la lista llamada infoBalas
        Partida.infoPaqueteVidas.Clear();
        Transform todosLosPaquetesVida = GameObject.Find("Paquete Vidas").transform;
        foreach (Transform paquete in todosLosPaquetesVida)
        {
            Partida.tipoPaqueteVidas iTemPaquete = new Partida.tipoPaqueteVidas
            {
                activoVida = paquete.gameObject.activeSelf
            };
            Partida.infoPaqueteVidas.Add(iTemPaquete);
        }

        // Guardar el estado de cada Npc en la lista llamada infoNpc
        Partida.infoPaqueteNpc.Clear();
        Transform todosLosNpc = GameObject.Find("npc").transform;
        foreach (Transform npc in todosLosNpc)
        {
            Partida.tipoPaqueteNpc iTemNpc = new Partida.tipoPaqueteNpc
            {
                activoNpc = npc.gameObject.activeSelf,
                posicion = npc.position,
                //vidaNpc = GetComponent<scr_npcmalos>().vida_npc
            };
            Partida.infoPaqueteNpc.Add(iTemNpc);
        }
    }
    void cargarPartida()
    {
        cantidadBalas = Partida.infoProta.cantidadBalas ;
        contador_runa = Partida.infoProta.cantRunas;
        Energia_Actual = Partida.infoProta.vida ;
        transform.position = Partida.infoProta.posicion;
        tieneArma = Partida.infoProta.arma; 
        // Cargar el estado de cada paquete de balas leyendo la lista llamada infoBalas
        Transform todosLosPaquetes = GameObject.Find("Paquete Balas").transform;
        int i = 0;
        foreach (Transform paquete in todosLosPaquetes)
        {
            paquete.gameObject.SetActive(Partida.infoPaqueteBalas[i++].activo);
        }
        // Cargar el estado de cada paquete de vidas leyendo la lista llamada infoBalas
        Transform todosLosPaquetesVida = GameObject.Find("Paquete Vidas").transform;
        int j = 0;
        foreach (Transform paquete in todosLosPaquetesVida)
        {
            paquete.gameObject.SetActive(Partida.infoPaqueteVidas[j++].activoVida);
        }

        // Cargar el estado de cada Npc
        Transform todosLosNpc = GameObject.Find("npc").transform;
        int k = 0;
        foreach (Transform npc in todosLosNpc)
        {
            npc.gameObject.SetActive(Partida.infoPaqueteNpc[k++].activoNpc);
        }
    }

    void disparar()
    {
        Vector3 direccion = (mira.position - contArma.position).normalized;

        // Pateo arma 
        rb.AddForce(magnitudPateoArma *-direccion, ForceMode2D.Impulse);
        // Particulas 
        Instantiate(particulasArma,puntaArma.position,Quaternion.identity);
        // Sacudir camara
        sacudirCamara(.5f);
        RaycastHit2D hit = Physics2D.Raycast(contArma.position, direccion, 1000f,~(1<<9));
        if (hit.collider != null)
        {
            // Le dio a algo
            if (hit.collider.gameObject.CompareTag("npc_malo")) {

                // le dio a al npc malos
                // Impulso
                hit.rigidbody.AddForce(magnitudReaccionDispararo * direccion, ForceMode2D.Impulse);
                // Particulas
                Instantiate(Sangre_npc, hit.point, Quaternion.identity);
                // Quitar vida npc_malo
                hit.transform.GetComponent<scr_npcmalos>().morir();
                //Destroy(hit.collider.gameObject);

            }
            
        }

        // RESTAR MUNICION
        cantidadBalas -= 1;
    }
    private void LateUpdate()
    {
        if (tieneArma)
        {
            // Girar la cabeza hacia el mouse
            cabeza.up = refOjos.position - mira.position;

            // Arma mire el maouse
            contArma.up = contArma.position - mira.position;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("arma"))
        {
            tieneArma = true;
            Destroy(collision.gameObject);
            contArma.gameObject.SetActive(true);
            cantidadBalas += 35;
            textoContadorBalas.color = Color.green;
            textoContadorBalas.fontSize = 50;
        }
        if (collision.gameObject.CompareTag("balas"))
        {
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
            cantidadBalas += 35;
            textoContadorBalas.color = Color.green;
            textoContadorBalas.fontSize = 50;
        }
        // Llenar Barra de vida
        if (collision.gameObject.CompareTag("vida"))
        {
            collision.gameObject.SetActive(false);
            Energia_Actual = 5;
        }
        if (collision.gameObject.CompareTag("runa"))
        {
            Destroy(collision.gameObject);
            contador_runa += 1;
        }
        if (collision.gameObject.CompareTag("fuego"))
        {
            Energia_Actual = 0;
            anim.SetTrigger("muerte");
        }
        if(collision.gameObject.CompareTag("checkpoint"))
        {
            guardarPartida();
        }
    }

    void sacudirCamara(float maximo)
    {
        magnitudPateoArma = maximo;
    }
    private void FixedUpdate()
    {
        if(magnitudPateoArma > .01f)
        {
            // Sacudir camara
            camaraSacudir.rotation = Quaternion.Euler(
                Random.Range(-magnitudPateoArma, magnitudPateoArma),
                Random.Range(-magnitudPateoArma, magnitudPateoArma),
                Random.Range(-magnitudPateoArma, magnitudPateoArma)
                );
            magnitudPateoArma *= .9f;
        }
        Vector3 puntoInicial = Camera.main.transform.position;
        Vector3 puntoFinal = transform.position - new Vector3(0, 0, 20);

        Camera.main.transform.position = Vector3.Lerp(puntoInicial, puntoFinal, .035f);
        actualizarDisplay();

        // Manejar la tela negra
        float valorAlfa= Mathf.Lerp(Tela.color.a,valorAlfaTela,.05f);
        Tela.color = new Color(0, 0, 0, valorAlfa);

        // Reiniciar escena cuando se complete el FadeOut
        if (valorAlfa > 0.9f && valorAlfaTela==1) SceneManager.LoadScene("Scenes/Nivel 1");
        // Reducir tamaño contador
        textoContadorBalas.color = Color.Lerp(textoContadorBalas.color, Color.white,.028f);
        textoContadorBalas.fontSize = Mathf.Lerp(textoContadorBalas.fontSize, 36, .028f);

    }

    void actualizarDisplay()
    {
        float valorAlfa = 1f / Energia_Max * (Energia_Max - Energia_Actual);
        mascara_daño.color = new Color(1, 1, 1, valorAlfa);
        // Cambiar barras de vida
        if (Energia_Actual == 5)
        {
            Barra_6.color= new Color(1, 1, 1, 1);
        }
        else
        {
            Barra_6.color = new Color(1, 1, 1, 0);
        }
        if (Energia_Actual == 4)
        {
            Barra_5.color = new Color(1, 1, 1, 1);
        }
        else
        {
            Barra_5.color = new Color(1, 1, 1, 0);
        }
        if (Energia_Actual == 3)
        {
            Barra_4.color = new Color(1, 1, 1, 1);
        }
        else
        {
            Barra_4.color = new Color(1, 1, 1, 0);
        }
        if (Energia_Actual == 2)
        {
            Barra_3.color = new Color(1, 1, 1, 1);
        }
        else
        {
            Barra_3.color = new Color(1, 1, 1, 0);
        }
        if (Energia_Actual == 1)
        {
            Barra_2.color = new Color(1, 1, 1, 1);
        }
        else
        {
            Barra_2.color = new Color(1, 1, 1, 0);
        }
        if (Energia_Actual == 0)
        {
            Barra_1.color = new Color(1, 1, 1, 1);
        }
        else
        {
            Barra_1.color = new Color(1, 1, 1, 0);
        }
        //inicio runa
        if (contador_runa == 0)
        {
            Runa1.color = new Color(1, 1, 1, 1);
        }
        else
        {
            Runa1.color = new Color(1, 1, 1, 0);
        }
        if (contador_runa == 1)
        {
            Runa2.color = new Color(1, 1, 1, 1);
        }
        else
        {
            Runa2.color = new Color(1, 1, 1, 0);
        }
        if (contador_runa == 2)
        {
            Runa3.color = new Color(1, 1, 1, 1);
        }
        else
        {
            Runa3.color = new Color(1, 1, 1, 0);
        }
        if (contador_runa == 3)
        {
            Runa4.color = new Color(1, 1, 1, 1);
        }
        else
        {
            Runa4.color = new Color(1, 1, 1, 0);
        }
        if (contador_runa == 4)
        {
            Runa5.color = new Color(1, 1, 1, 1);
        }
        else
        {
            Runa5.color = new Color(1, 1, 1, 0);
        }
 
    }


    public void recibir_golpe(Vector2 posicion)
    {
        // reducir la energia
        Energia_Actual -= 1;
        if (Energia_Actual <= 0)
        {
            //Instantiate(Particulas_sangre, posicion, Quaternion.identity);
            actualizarDisplay();

            // Comienzo muerte
            anim.SetTrigger("muerte");
        }
        else
        {
            Debug.Log("auch!" + Energia_Actual);
            // Particulas de sangre
            Instantiate(Particulas_sangre, posicion, Quaternion.identity);
            // Disparar la animacion
            anim.SetTrigger("auch");

        }


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "PlataformaMovible")
        {
            transform.parent = collision.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "PlataformaMovible")
        {
            transform.parent = null;
        }
    }

    public void iniciarFadeOut()
    {
        valorAlfaTela = 1;
    }

    void cargarNivelBoss()
    {
        iniciarFadeOut();
        SceneManager.LoadScene("Boss 1");
    }
}

