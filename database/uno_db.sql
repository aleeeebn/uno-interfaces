-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema uno_db
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema uno_db
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `uno_db` DEFAULT CHARACTER SET utf8 ;
USE `uno_db` ;

-- -----------------------------------------------------
-- Table `uno_db`.`jugadores`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `uno_db`.`jugadores` (
  `id_jugador` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`id_jugador`),
  UNIQUE INDEX `nombre_UNIQUE` (`nombre` ASC) VISIBLE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `uno_db`.`partidas`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `uno_db`.`partidas` (
  `id_partida` INT NOT NULL AUTO_INCREMENT,
  `fecha_inicio` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_fin` DATETIME NULL,
  `id_ganador` INT NULL,
  `estado` VARCHAR(15) NOT NULL DEFAULT 'en_curso',
  PRIMARY KEY (`id_partida`),
  INDEX `fk_partidas_jugadores1_idx` (`id_ganador` ASC) VISIBLE,
  CONSTRAINT `fk_partidas_jugadores1`
    FOREIGN KEY (`id_ganador`)
    REFERENCES `uno_db`.`jugadores` (`id_jugador`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `uno_db`.`partida_jugadores`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `uno_db`.`partida_jugadores` (
  `id_partida` INT NOT NULL,
  `id_jugador` INT NOT NULL,
  PRIMARY KEY (`id_partida`, `id_jugador`),
  INDEX `fk_partida_jugadores_jugadores1_idx` (`id_jugador` ASC) VISIBLE,
  CONSTRAINT `fk_partida_jugadores_partidas`
    FOREIGN KEY (`id_partida`)
    REFERENCES `uno_db`.`partidas` (`id_partida`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_partida_jugadores_jugadores1`
    FOREIGN KEY (`id_jugador`)
    REFERENCES `uno_db`.`jugadores` (`id_jugador`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `uno_db`.`log_movimientos`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `uno_db`.`log_movimientos` (
  `id_movimiento` INT NOT NULL AUTO_INCREMENT,
  `id_partida` INT NOT NULL,
  `id_jugador` INT NOT NULL,
  `accion` VARCHAR(20) NOT NULL,
  `color_carta` VARCHAR(10) NULL,
  `valor_carta` INT NULL,
  `fecha_hora` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_movimiento`),
  INDEX `fk_log_movimientos_partidas1_idx` (`id_partida` ASC) VISIBLE,
  INDEX `fk_log_movimientos_jugadores1_idx` (`id_jugador` ASC) VISIBLE,
  CONSTRAINT `fk_log_movimientos_partidas1`
    FOREIGN KEY (`id_partida`)
    REFERENCES `uno_db`.`partidas` (`id_partida`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_log_movimientos_jugadores1`
    FOREIGN KEY (`id_jugador`)
    REFERENCES `uno_db`.`jugadores` (`id_jugador`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
