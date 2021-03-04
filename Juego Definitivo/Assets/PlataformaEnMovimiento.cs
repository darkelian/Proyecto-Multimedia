using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformaEnMovimiento : MonoBehaviour
{
    public GameObject ObjectAmover;
    
    public Transform Starpoint;
    public Transform Endpoint;
    public float Velocidad;
    Vector3 MoverHacia;
    // Start is called before the first frame update
    void Start()
    {
        MoverHacia = Endpoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        ObjectAmover.transform.position = Vector3.MoveTowards(ObjectAmover.transform.position, MoverHacia, Velocidad*Time.deltaTime);
        
        if (ObjectAmover.transform.position == Endpoint.position)
        {
            MoverHacia = Starpoint.position;
        }

        if (ObjectAmover.transform.position == Starpoint.position)
        {
            MoverHacia = Endpoint.position;
        }
    }
}
