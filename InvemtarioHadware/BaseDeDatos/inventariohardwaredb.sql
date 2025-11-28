-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 28-11-2025 a las 17:35:41
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.0.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inventariohardwaredb`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `asignacioneshw`
--

CREATE TABLE `asignacioneshw` (
  `idAsignacion` int(11) NOT NULL,
  `FechaAsignacion` date NOT NULL,
  `FechaDevolucion` date DEFAULT NULL,
  `idHardware` int(11) NOT NULL,
  `idEmpleado` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `asignacioneshw`
--

INSERT INTO `asignacioneshw` (`idAsignacion`, `FechaAsignacion`, `FechaDevolucion`, `idHardware`, `idEmpleado`) VALUES
(1, '2025-11-26', '2025-11-26', 1, 1),
(2, '2025-11-28', NULL, 2, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `empleados`
--

CREATE TABLE `empleados` (
  `idEmpleado` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Puesto` varchar(50) DEFAULT NULL,
  `Departamento` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `empleados`
--

INSERT INTO `empleados` (`idEmpleado`, `Nombre`, `Puesto`, `Departamento`) VALUES
(1, 'Juan Perez', 'Gerente', 'Ventas'),
(2, 'Ana Lopez', 'Desarrolladora', 'Sistemas');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `hardware`
--

CREATE TABLE `hardware` (
  `idHardware` int(11) NOT NULL,
  `Serie` varchar(100) DEFAULT NULL,
  `FechaCompra` date DEFAULT NULL,
  `Estado` varchar(50) DEFAULT NULL,
  `idModelo` int(11) NOT NULL,
  `idUbicacion` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `hardware`
--

INSERT INTO `hardware` (`idHardware`, `Serie`, `FechaCompra`, `Estado`, `idModelo`, `idUbicacion`) VALUES
(1, 'PC001', '2025-11-27', 'En Reparación', 1, 2),
(2, 'SN-555', '2025-11-28', 'En Reparación', 5, 5);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `mantenimientoshw`
--

CREATE TABLE `mantenimientoshw` (
  `idMantenimiento` int(11) NOT NULL,
  `Fecha` date NOT NULL,
  `Descripcion` text DEFAULT NULL,
  `Costo` decimal(10,2) DEFAULT NULL,
  `idHardware` int(11) NOT NULL,
  `idProveedor` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `mantenimientoshw`
--

INSERT INTO `mantenimientoshw` (`idMantenimiento`, `Fecha`, `Descripcion`, `Costo`, `idHardware`, `idProveedor`) VALUES
(1, '2025-11-26', 'No prende', 10500.00, 1, 1),
(2, '2025-11-28', 'Pantalla rota', 100.00, 2, 3);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `marcas`
--

CREATE TABLE `marcas` (
  `idMarca` int(11) NOT NULL,
  `NombreMarca` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `marcas`
--

INSERT INTO `marcas` (`idMarca`, `NombreMarca`) VALUES
(1, 'Dell'),
(2, 'HP'),
(3, 'Cisco'),
(4, 'Lenovo'),
(5, 'Asus'),
(6, 'Sony');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `modelos`
--

CREATE TABLE `modelos` (
  `idModelo` int(11) NOT NULL,
  `NombreModelo` varchar(100) NOT NULL,
  `idMarca` int(11) NOT NULL,
  `idTipoHardware` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `modelos`
--

INSERT INTO `modelos` (`idModelo`, `NombreModelo`, `idMarca`, `idTipoHardware`) VALUES
(1, 'Latitude 5420', 1, 1),
(2, 'ProDesk 600', 2, 2),
(3, 'Catalyst 2960', 3, 4),
(4, 'Redmi Note 10 Pro', 5, 5),
(5, 'Vaio Z 10', 6, 2);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `proveedores`
--

CREATE TABLE `proveedores` (
  `idProveedor` int(11) NOT NULL,
  `NombreProveedor` varchar(100) NOT NULL,
  `Telefono` varchar(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `proveedores`
--

INSERT INTO `proveedores` (`idProveedor`, `NombreProveedor`, `Telefono`) VALUES
(1, 'Office Depot', '555-0199'),
(2, 'Cisco Services', '800-1234'),
(3, 'Halcones', '7221054000');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `roles`
--

CREATE TABLE `roles` (
  `idRol` int(11) NOT NULL,
  `NombreRol` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `roles`
--

INSERT INTO `roles` (`idRol`, `NombreRol`) VALUES
(1, 'Administrador'),
(2, 'Usuario');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `sedes`
--

CREATE TABLE `sedes` (
  `idSede` int(11) NOT NULL,
  `NombreSede` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `sedes`
--

INSERT INTO `sedes` (`idSede`, `NombreSede`) VALUES
(1, 'Edificio Central'),
(2, 'Planta Norte'),
(3, 'Edificio T'),
(4, 'Almacen Central');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tiposhardware`
--

CREATE TABLE `tiposhardware` (
  `idTipoHardware` int(11) NOT NULL,
  `NombreTipo` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tiposhardware`
--

INSERT INTO `tiposhardware` (`idTipoHardware`, `NombreTipo`) VALUES
(1, 'Laptop'),
(2, 'Desktop'),
(3, 'Impresora'),
(4, 'Switch'),
(5, 'Telefono'),
(6, 'Vaio Z');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `ubicaciones`
--

CREATE TABLE `ubicaciones` (
  `idUbicacion` int(11) NOT NULL,
  `NombreUbicacion` varchar(100) NOT NULL,
  `idSede` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `ubicaciones`
--

INSERT INTO `ubicaciones` (`idUbicacion`, `NombreUbicacion`, `idSede`) VALUES
(1, 'Sala de Servidores', 1),
(2, 'Recepción', 1),
(3, 'Almacén', 2),
(4, 'TCL-1', 3),
(5, 'Estante 4', 4);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `idUsuario` int(11) NOT NULL,
  `Username` varchar(50) NOT NULL,
  `PasswordHash` varchar(255) NOT NULL,
  `idRol` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuarios`
--

INSERT INTO `usuarios` (`idUsuario`, `Username`, `PasswordHash`, `idRol`) VALUES
(1, 'Fabian', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 1),
(3, 'Lalo', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 2);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `asignacioneshw`
--
ALTER TABLE `asignacioneshw`
  ADD PRIMARY KEY (`idAsignacion`),
  ADD KEY `idHardware` (`idHardware`),
  ADD KEY `idEmpleado` (`idEmpleado`);

--
-- Indices de la tabla `empleados`
--
ALTER TABLE `empleados`
  ADD PRIMARY KEY (`idEmpleado`);

--
-- Indices de la tabla `hardware`
--
ALTER TABLE `hardware`
  ADD PRIMARY KEY (`idHardware`),
  ADD UNIQUE KEY `Serie` (`Serie`),
  ADD KEY `idModelo` (`idModelo`),
  ADD KEY `idUbicacion` (`idUbicacion`);

--
-- Indices de la tabla `mantenimientoshw`
--
ALTER TABLE `mantenimientoshw`
  ADD PRIMARY KEY (`idMantenimiento`),
  ADD KEY `idHardware` (`idHardware`),
  ADD KEY `idProveedor` (`idProveedor`);

--
-- Indices de la tabla `marcas`
--
ALTER TABLE `marcas`
  ADD PRIMARY KEY (`idMarca`);

--
-- Indices de la tabla `modelos`
--
ALTER TABLE `modelos`
  ADD PRIMARY KEY (`idModelo`),
  ADD KEY `idMarca` (`idMarca`),
  ADD KEY `idTipoHardware` (`idTipoHardware`);

--
-- Indices de la tabla `proveedores`
--
ALTER TABLE `proveedores`
  ADD PRIMARY KEY (`idProveedor`);

--
-- Indices de la tabla `roles`
--
ALTER TABLE `roles`
  ADD PRIMARY KEY (`idRol`);

--
-- Indices de la tabla `sedes`
--
ALTER TABLE `sedes`
  ADD PRIMARY KEY (`idSede`);

--
-- Indices de la tabla `tiposhardware`
--
ALTER TABLE `tiposhardware`
  ADD PRIMARY KEY (`idTipoHardware`);

--
-- Indices de la tabla `ubicaciones`
--
ALTER TABLE `ubicaciones`
  ADD PRIMARY KEY (`idUbicacion`),
  ADD KEY `idSede` (`idSede`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`idUsuario`),
  ADD UNIQUE KEY `Username` (`Username`),
  ADD KEY `idRol` (`idRol`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `asignacioneshw`
--
ALTER TABLE `asignacioneshw`
  MODIFY `idAsignacion` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `empleados`
--
ALTER TABLE `empleados`
  MODIFY `idEmpleado` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `hardware`
--
ALTER TABLE `hardware`
  MODIFY `idHardware` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `mantenimientoshw`
--
ALTER TABLE `mantenimientoshw`
  MODIFY `idMantenimiento` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `marcas`
--
ALTER TABLE `marcas`
  MODIFY `idMarca` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT de la tabla `modelos`
--
ALTER TABLE `modelos`
  MODIFY `idModelo` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT de la tabla `proveedores`
--
ALTER TABLE `proveedores`
  MODIFY `idProveedor` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `roles`
--
ALTER TABLE `roles`
  MODIFY `idRol` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `sedes`
--
ALTER TABLE `sedes`
  MODIFY `idSede` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `tiposhardware`
--
ALTER TABLE `tiposhardware`
  MODIFY `idTipoHardware` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT de la tabla `ubicaciones`
--
ALTER TABLE `ubicaciones`
  MODIFY `idUbicacion` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `idUsuario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `asignacioneshw`
--
ALTER TABLE `asignacioneshw`
  ADD CONSTRAINT `asignacioneshw_ibfk_1` FOREIGN KEY (`idHardware`) REFERENCES `hardware` (`idHardware`),
  ADD CONSTRAINT `asignacioneshw_ibfk_2` FOREIGN KEY (`idEmpleado`) REFERENCES `empleados` (`idEmpleado`);

--
-- Filtros para la tabla `hardware`
--
ALTER TABLE `hardware`
  ADD CONSTRAINT `hardware_ibfk_1` FOREIGN KEY (`idModelo`) REFERENCES `modelos` (`idModelo`),
  ADD CONSTRAINT `hardware_ibfk_2` FOREIGN KEY (`idUbicacion`) REFERENCES `ubicaciones` (`idUbicacion`);

--
-- Filtros para la tabla `mantenimientoshw`
--
ALTER TABLE `mantenimientoshw`
  ADD CONSTRAINT `mantenimientoshw_ibfk_1` FOREIGN KEY (`idHardware`) REFERENCES `hardware` (`idHardware`),
  ADD CONSTRAINT `mantenimientoshw_ibfk_2` FOREIGN KEY (`idProveedor`) REFERENCES `proveedores` (`idProveedor`);

--
-- Filtros para la tabla `modelos`
--
ALTER TABLE `modelos`
  ADD CONSTRAINT `modelos_ibfk_1` FOREIGN KEY (`idMarca`) REFERENCES `marcas` (`idMarca`),
  ADD CONSTRAINT `modelos_ibfk_2` FOREIGN KEY (`idTipoHardware`) REFERENCES `tiposhardware` (`idTipoHardware`);

--
-- Filtros para la tabla `ubicaciones`
--
ALTER TABLE `ubicaciones`
  ADD CONSTRAINT `ubicaciones_ibfk_1` FOREIGN KEY (`idSede`) REFERENCES `sedes` (`idSede`);

--
-- Filtros para la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD CONSTRAINT `usuarios_ibfk_1` FOREIGN KEY (`idRol`) REFERENCES `roles` (`idRol`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
