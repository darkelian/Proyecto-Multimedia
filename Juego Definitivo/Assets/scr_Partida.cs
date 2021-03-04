using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Partida 
{
    public static bool hayPartida= false;
    public static class infoProta
    {
        public static Vector2 posicion;
        public static int vida;
        public static int cantidadBalas;
        public static int cantRunas;
        public static bool arma;
    }
    // Clase balas
    public class tipoPaqueteBalas
    {
        public bool activo;
    }

    // Clase vidas
    public class tipoPaqueteVidas
    {
        public bool activoVida;
    }

    // Clase Npc
    public class tipoPaqueteNpc
    {
        public bool activoNpc;
        public Vector2 posicion;
        public float vidaNpc;
    }
    // listas
    public static List<tipoPaqueteBalas> infoPaqueteBalas= new List<tipoPaqueteBalas>();
    public static List<tipoPaqueteVidas> infoPaqueteVidas = new List<tipoPaqueteVidas>();
    public static List<tipoPaqueteNpc> infoPaqueteNpc = new List<tipoPaqueteNpc>();
}
