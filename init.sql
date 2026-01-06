SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

CREATE DATABASE IF NOT EXISTS `kiwdy_bd` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `kiwdy_bd`;

CREATE TABLE `Cursos` (
  `IdCurso` int(11) NOT NULL,
  `Titulo` varchar(100) NOT NULL,
  `Descripcion` varchar(500) NOT NULL,
  `PortadaUrl` varchar(100) DEFAULT NULL,
  `Precio` decimal(10,0) NOT NULL,
  `Habilitado` tinyint(1) NOT NULL,
  `IdUsuarioInstructor` int(11) NOT NULL,
  `Eliminado` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE `Examenes` (
  `IdExamen` int(11) NOT NULL,
  `FechaYHora` datetime NOT NULL,
  `Modalidad` enum('VIRTUAL','PRESENCIAL') NOT NULL,
  `Link` varchar(255) NOT NULL,
  `Direccion` varchar(255) NOT NULL,
  `Nota` varchar(200) NOT NULL,
  `IdUsuarioInstructor` int(11) NOT NULL,
  `IdInscripcion` int(11) NOT NULL,
  `Eliminado` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE `Inscripciones` (
  `IdInscripcion` int(11) NOT NULL,
  `FechaInicio` date NOT NULL,
  `FechaFin` date DEFAULT NULL,
  `Habilitado` tinyint(1) NOT NULL,
  `IdCurso` int(11) NOT NULL,
  `IdUsuarioAlumno` int(11) NOT NULL,
  `Eliminado` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE `Materiales` (
  `IdMaterial` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Url` varchar(255) NOT NULL,
  `IdSeccion` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE `Secciones` (
  `IdSeccion` int(11) NOT NULL,
  `Titulo` varchar(100) NOT NULL,
  `Contenido` text NOT NULL,
  `VideoUrl` varchar(300) DEFAULT NULL,
  `Orden` int(11) NOT NULL,
  `IdCurso` int(11) NOT NULL,
  `Eliminado` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE `SeccionesCompletadas` (
  `IdInscripcion` int(11) NOT NULL,
  `IdSeccion` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE `Usuarios` (
  `IdUsuario` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Apellido` varchar(100) NOT NULL,
  `Email` varchar(255) NOT NULL,
  `Telefono` varchar(20) NOT NULL,
  `Clave` varchar(255) NOT NULL,
  `Rol` int(11) NOT NULL,
  `Eliminado` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


ALTER TABLE `Cursos`
  ADD PRIMARY KEY (`IdCurso`),
  ADD KEY `IdUsuarioInstructor` (`IdUsuarioInstructor`);

ALTER TABLE `Examenes`
  ADD PRIMARY KEY (`IdExamen`);

ALTER TABLE `Inscripciones`
  ADD PRIMARY KEY (`IdInscripcion`),
  ADD UNIQUE KEY `IdCurso` (`IdCurso`,`IdUsuarioAlumno`,`Eliminado`),
  ADD KEY `IdUsuarioAlumno` (`IdUsuarioAlumno`);

ALTER TABLE `Materiales`
  ADD PRIMARY KEY (`IdMaterial`),
  ADD KEY `IdSeccion` (`IdSeccion`);

ALTER TABLE `Secciones`
  ADD PRIMARY KEY (`IdSeccion`),
  ADD UNIQUE KEY `Orden` (`Orden`,`IdCurso`,`Eliminado`),
  ADD KEY `IdCurso` (`IdCurso`);

ALTER TABLE `SeccionesCompletadas`
  ADD PRIMARY KEY (`IdInscripcion`,`IdSeccion`),
  ADD KEY `IdSeccion` (`IdSeccion`);

ALTER TABLE `Usuarios`
  ADD PRIMARY KEY (`IdUsuario`),
  ADD UNIQUE KEY `Email` (`Email`,`Rol`,`Eliminado`) USING BTREE;


ALTER TABLE `Cursos`
  MODIFY `IdCurso` int(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `Examenes`
  MODIFY `IdExamen` int(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `Inscripciones`
  MODIFY `IdInscripcion` int(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `Materiales`
  MODIFY `IdMaterial` int(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `Secciones`
  MODIFY `IdSeccion` int(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `Usuarios`
  MODIFY `IdUsuario` int(11) NOT NULL AUTO_INCREMENT;


ALTER TABLE `Cursos`
  ADD CONSTRAINT `Cursos_ibfk_1` FOREIGN KEY (`IdUsuarioInstructor`) REFERENCES `Usuarios` (`IdUsuario`);

ALTER TABLE `Inscripciones`
  ADD CONSTRAINT `Inscripciones_ibfk_1` FOREIGN KEY (`IdUsuarioAlumno`) REFERENCES `Usuarios` (`IdUsuario`);

ALTER TABLE `Materiales`
  ADD CONSTRAINT `Materiales_ibfk_1` FOREIGN KEY (`IdSeccion`) REFERENCES `Secciones` (`IdSeccion`);

ALTER TABLE `Secciones`
  ADD CONSTRAINT `Secciones_ibfk_1` FOREIGN KEY (`IdCurso`) REFERENCES `Cursos` (`IdCurso`);

ALTER TABLE `SeccionesCompletadas`
  ADD CONSTRAINT `SeccionesCompletadas_ibfk_1` FOREIGN KEY (`IdInscripcion`) REFERENCES `Inscripciones` (`IdInscripcion`),
  ADD CONSTRAINT `SeccionesCompletadas_ibfk_2` FOREIGN KEY (`IdSeccion`) REFERENCES `Secciones` (`IdSeccion`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
