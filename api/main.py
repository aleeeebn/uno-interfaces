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
    conexion = obtener_conexion()
    cursor = conexion.cursor()
    cursor.execute("SELECT id_jugador, nombre FROM jugadores")
    datos = cursor.fetchall()
    conexion.close()
    return datos

@app.post("/partidas")
def crear_partida():
    conexion = obtener_conexion()
    cursor = conexion.cursor()
    cursor.execute("INSERT INTO partidas () VALUES ()")
    id_partida = cursor.lastrowid
    cursor.execute(
        "INSERT INTO partida_jugadores (id_partida, id_jugador) "
        "SELECT %s, id_jugador FROM jugadores",
        (id_partida,)
    )
    conexion.commit()
    conexion.close()
    return {"id_partida": id_partida}

@app.post("/partidas/{id_partida}/terminar")
def terminar_partida(id_partida: int, id_ganador: int):
    conexion = obtener_conexion()
    cursor = conexion.cursor()
    cursor.execute(
        "UPDATE partidas SET fecha_fin = NOW(), id_ganador = %s, estado = 'terminada' "
        "WHERE id_partida = %s",(id_ganador, id_partida)
    )
    conexion.commit()
    conexion.close()
    return {"id_partida": id_partida, "id_ganador" : id_ganador}

@app.post("/movimientos")
def registrar_movimiento(id_partida: int, id_jugador: int, accion: str, color_carta: str = None, valor_carta: int = None):
    conexion = obtener_conexion()
    cursor = conexion.cursor()
    cursor.execute(
        "INSERT INTO log_movimientos "
        "(id_partida, id_jugador, accion, color_carta, valor_carta) "
        "VALUES (%s, %s, %s, %s, %s)",
        (id_partida, id_jugador, accion, color_carta, valor_carta)
    )
    conexion.commit()
    id_movimiento = cursor.lastrowid
    conexion.close()
    return {"id_movimiento": id_movimiento}

@app.get("/historial")
def historial():
    conexion = obtener_conexion()
    cursor = conexion.cursor()
    cursor.execute(
        "SELECT j.id_jugador, j.nombre, "
        "COUNT(p.id_partida) AS jugadas, "
        "COUNT(CASE WHEN p.id_ganador = j.id_jugador THEN p.id_partida END) AS ganadas "
        "FROM jugadores j "
        "LEFT JOIN partida_jugadores pj ON pj.id_jugador = j.id_jugador "
        "LEFT JOIN partidas p ON p.id_partida = pj.id_partida AND p.estado = 'terminada' "
        "GROUP BY j.id_jugador, j.nombre"
    )
    datos = cursor.fetchall()
    conexion.close()
    for fila in datos:
        fila["perdidas"] = fila["jugadas"] - fila["ganadas"]
    return datos
