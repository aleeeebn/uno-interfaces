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
