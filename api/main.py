from fastapi import FastAPI
import pymysql
import os
from dotenv import load_dotenv

load_dotenv()
app=FastAPI()

def obtener_conexion():
    return pymysql.connect(
        host=os.getenv("DB_HOST"),
        port=int(os.getenv("DB_PORT")),
        user=os.getenv("DB_USER"),
        password=os.getenv("DB_PASSWORD"),
        database=os.getenv("DB_NAME"),
        cursorclass=pymysql.cursors.DictCursor
    )

@app.get("/jugadores")
def jugadores():
    conexion=obtener_conexion()
    try:
       cursor=conexion.cursor()
       cursor.execute("SELECT id_jugador, nombre FROM jugadores")
       filas=cursor.fetchall()
       return {"jugadores":filas}
    finally:
        conexion.close()

@app.post("/partidas")
def crear_partida():
    conexion = obtener_conexion()
    try:
        cursor = conexion.cursor()
        cursor.execute("INSERT INTO partidas () VALUES ()")
        id_partida = cursor.lastrowid
        cursor.execute("SELECT id_jugador FROM jugadores")
        filas = cursor.fetchall()
        for fila in filas:
            cursor.execute(
                "INSERT INTO partida_jugadores (id_partida, id_jugador) VALUES (%s, %s)",
                (id_partida, fila["id_jugador"])
            )
        conexion.commit()
        return {"id_partida": id_partida}
    except Exception:
        conexion.rollback()
        raise
    finally:
        conexion.close()
