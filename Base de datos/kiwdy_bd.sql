-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: localhost
-- Tiempo de generación: 24-02-2026 a las 17:04:51
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `kiwdy_bd`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Cursos`
--

CREATE TABLE `Cursos` (
  `IdCurso` int(11) NOT NULL,
  `Titulo` varchar(100) NOT NULL,
  `Descripcion` text NOT NULL,
  `PortadaUrl` varchar(100) DEFAULT NULL,
  `Precio` decimal(18,2) NOT NULL,
  `Habilitado` tinyint(1) NOT NULL,
  `NotaAprobacion` decimal(18,2) NOT NULL DEFAULT -1.00,
  `IdUsuarioInstructor` int(11) NOT NULL,
  `Eliminado` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Examenes`
--

CREATE TABLE `Examenes` (
  `IdExamen` int(11) NOT NULL,
  `FechaYHora` datetime NOT NULL,
  `Modalidad` int(11) NOT NULL,
  `Link` varchar(255) DEFAULT NULL,
  `Direccion` varchar(255) DEFAULT NULL,
  `Nota` int(11) DEFAULT -1,
  `IdInscripcion` int(11) NOT NULL,
  `Eliminado` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Inscripciones`
--

CREATE TABLE `Inscripciones` (
  `IdInscripcion` int(11) NOT NULL,
  `FechaInicio` date DEFAULT NULL,
  `FechaFin` date DEFAULT NULL,
  `Estado` int(11) NOT NULL,
  `Certificado` varchar(300) DEFAULT NULL,
  `IdCurso` int(11) NOT NULL,
  `IdUsuarioAlumno` int(11) NOT NULL,
  `Eliminado` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Materiales`
--

CREATE TABLE `Materiales` (
  `IdMaterial` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Url` varchar(255) NOT NULL,
  `IdSeccion` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Secciones`
--

CREATE TABLE `Secciones` (
  `IdSeccion` int(11) NOT NULL,
  `Titulo` varchar(100) NOT NULL,
  `Contenido` text NOT NULL,
  `VideoUrl` varchar(300) DEFAULT NULL,
  `Orden` int(11) NOT NULL,
  `IdCurso` int(11) NOT NULL,
  `Eliminado` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `SeccionesCompletadas`
--

CREATE TABLE `SeccionesCompletadas` (
  `IdInscripcion` int(11) NOT NULL,
  `IdSeccion` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Usuarios`
--

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

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `Cursos`
--
ALTER TABLE `Cursos`
  ADD PRIMARY KEY (`IdCurso`),
  ADD KEY `IdUsuarioInstructor` (`IdUsuarioInstructor`);

--
-- Indices de la tabla `Examenes`
--
ALTER TABLE `Examenes`
  ADD PRIMARY KEY (`IdExamen`);

--
-- Indices de la tabla `Inscripciones`
--
ALTER TABLE `Inscripciones`
  ADD PRIMARY KEY (`IdInscripcion`),
  ADD UNIQUE KEY `IdCurso` (`IdCurso`,`IdUsuarioAlumno`,`Eliminado`),
  ADD KEY `IdUsuarioAlumno` (`IdUsuarioAlumno`);

--
-- Indices de la tabla `Materiales`
--
ALTER TABLE `Materiales`
  ADD PRIMARY KEY (`IdMaterial`),
  ADD KEY `IdSeccion` (`IdSeccion`);

--
-- Indices de la tabla `Secciones`
--
ALTER TABLE `Secciones`
  ADD PRIMARY KEY (`IdSeccion`),
  ADD UNIQUE KEY `Orden` (`Orden`,`IdCurso`,`Eliminado`),
  ADD KEY `IdCurso` (`IdCurso`);

--
-- Indices de la tabla `SeccionesCompletadas`
--
ALTER TABLE `SeccionesCompletadas`
  ADD PRIMARY KEY (`IdInscripcion`,`IdSeccion`),
  ADD KEY `IdSeccion` (`IdSeccion`);

--
-- Indices de la tabla `Usuarios`
--
ALTER TABLE `Usuarios`
  ADD PRIMARY KEY (`IdUsuario`),
  ADD UNIQUE KEY `Email` (`Email`,`Rol`,`Eliminado`) USING BTREE;

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `Cursos`
--
ALTER TABLE `Cursos`
  MODIFY `IdCurso` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `Examenes`
--
ALTER TABLE `Examenes`
  MODIFY `IdExamen` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `Inscripciones`
--
ALTER TABLE `Inscripciones`
  MODIFY `IdInscripcion` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `Materiales`
--
ALTER TABLE `Materiales`
  MODIFY `IdMaterial` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `Secciones`
--
ALTER TABLE `Secciones`
  MODIFY `IdSeccion` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `Usuarios`
--
ALTER TABLE `Usuarios`
  MODIFY `IdUsuario` int(11) NOT NULL AUTO_INCREMENT;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `Cursos`
--
ALTER TABLE `Cursos`
  ADD CONSTRAINT `Cursos_ibfk_1` FOREIGN KEY (`IdUsuarioInstructor`) REFERENCES `Usuarios` (`IdUsuario`);

--
-- Filtros para la tabla `Inscripciones`
--
ALTER TABLE `Inscripciones`
  ADD CONSTRAINT `Inscripciones_ibfk_1` FOREIGN KEY (`IdUsuarioAlumno`) REFERENCES `Usuarios` (`IdUsuario`);

--
-- Filtros para la tabla `Materiales`
--
ALTER TABLE `Materiales`
  ADD CONSTRAINT `Materiales_ibfk_1` FOREIGN KEY (`IdSeccion`) REFERENCES `Secciones` (`IdSeccion`);

--
-- Filtros para la tabla `Secciones`
--
ALTER TABLE `Secciones`
  ADD CONSTRAINT `Secciones_ibfk_1` FOREIGN KEY (`IdCurso`) REFERENCES `Cursos` (`IdCurso`);

--
-- Filtros para la tabla `SeccionesCompletadas`
--
ALTER TABLE `SeccionesCompletadas`
  ADD CONSTRAINT `SeccionesCompletadas_ibfk_1` FOREIGN KEY (`IdInscripcion`) REFERENCES `Inscripciones` (`IdInscripcion`),
  ADD CONSTRAINT `SeccionesCompletadas_ibfk_2` FOREIGN KEY (`IdSeccion`) REFERENCES `Secciones` (`IdSeccion`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
