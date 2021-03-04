using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class scr_Boss : MonoBehaviour
{

    Rigidbody2D rb;
    public float velCaminata = 6f;
    int direccion = -1;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(velCaminata * direccion, rb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("menu"))
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
