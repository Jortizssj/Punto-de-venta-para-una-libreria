-- TALLER DE BASES DE DATOS - PROYECTO FINAL LIBRERIA
-- 1. CREACIÓN Y USO DE LA BASE DE DATOS
CREATE DATABASE IF NOT EXISTS VENTAS;
USE VENTAS;

-- 2. TABLA DE EMPLEADOS 
CREATE TABLE EMPLEADOS (
    ID_Empleado INT PRIMARY KEY AUTO_INCREMENT,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Cargo VARCHAR(50) NOT NULL,
    Activo BOOLEAN DEFAULT TRUE -- Para borrado lógico
);

-- 3. TABLA DE USUARIOS 
-- Las contraseñas se almacenan en HASH (encriptación no reversible) 
CREATE TABLE USUARIOS (
    ID_Usuario INT PRIMARY KEY AUTO_INCREMENT,
    NombreUsuario VARCHAR(50) NOT NULL UNIQUE,
    Contrasena_Hash VARCHAR(128) NOT NULL,
    Tipo_Usuario ENUM('Administrador', 'Vendedor') NOT NULL, -- 2 TIPOS DE USUARIOS
    ID_Empleado INT NULL,
    FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ID_Empleado) REFERENCES EMPLEADOS(ID_Empleado)
);

-- 4. TABLA DE PRODUCTOS
CREATE TABLE PRODUCTOS (
    CLAVE VARCHAR(20) PRIMARY KEY, -- Usamos ISBN como CLAVE
    NOMBRE VARCHAR(150) NOT NULL,
    DESCRIPCION TEXT,
    PRECIO DECIMAL(10, 2) NOT NULL CHECK (PRECIO >= 0),
    STOCK INT NOT NULL CHECK (STOCK >= 0)
);

-- 5. TABLA DE VENTAS
CREATE TABLE VENTAS (
    ID_Venta INT PRIMARY KEY AUTO_INCREMENT,
    Fecha_Hora DATETIME DEFAULT CURRENT_TIMESTAMP,
    Total DECIMAL(10, 2) NOT NULL,
    ID_Empleado INT,
    FOREIGN KEY (ID_Empleado) REFERENCES EMPLEADOS(ID_Empleado)
);

-- 6. TABLA DE DETALLE DE VENTA
CREATE TABLE DETALLE_VENTA (
    ID_Detalle INT PRIMARY KEY AUTO_INCREMENT,
    ID_Venta INT,
    CLAVE_Producto VARCHAR(20),
    Cantidad INT NOT NULL CHECK (Cantidad > 0),
    Precio_Unitario DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (ID_Venta) REFERENCES VENTAS(ID_Venta),
    FOREIGN KEY (CLAVE_Producto) REFERENCES PRODUCTOS(CLAVE)
);

-- 7. TABLA DE AUDITORÍA
CREATE TABLE AUDITORIA_PRODUCTOS (
    ID_Auditoria INT PRIMARY KEY AUTO_INCREMENT,
    Fecha_Hora DATETIME DEFAULT CURRENT_TIMESTAMP, -- Hora y fecha del cambio
    Tipo_Cambio VARCHAR(10) NOT NULL,      -- INSERT, UPDATE o DELETE
    Usuario_DB VARCHAR(50) NULL, -- Usuario de la base de datos
    CLAVE_Producto_Afectado VARCHAR(20) NOT NULL, -- CLAVE del producto
    Campo_Afectado VARCHAR(50) NULL, -- Nombre de la columna cambiada (en UPDATE)
    Valor_Anterior TEXT NULL, -- Valor antes del cambio
    Valor_Nuevo TEXT NULL     -- Nuevo valor
);

-- 8. PROCEDIMIENTOS ALMACENADOS
DELIMITER //

-- SP 1: Insertar Empleado
CREATE PROCEDURE sp_InsertarEmpleado(
    IN p_Nombre VARCHAR(100),
    IN p_Apellido VARCHAR(100),
    IN p_Cargo VARCHAR(50)
)
BEGIN
    INSERT INTO EMPLEADOS (Nombre, Apellido, Cargo)
    VALUES (p_Nombre, p_Apellido, p_Cargo);
END //

-- SP 2: Actualizar Empleado
CREATE PROCEDURE sp_ActualizarEmpleado(
    IN p_ID_Empleado INT,
    IN p_Nombre VARCHAR(100),
    IN p_Apellido VARCHAR(100),
    IN p_Cargo VARCHAR(50)
)
BEGIN
    UPDATE EMPLEADOS
    SET 
        Nombre = p_Nombre,
        Apellido = p_Apellido,
        Cargo = p_Cargo
    WHERE 
        ID_Empleado = p_ID_Empleado;
END //

-- SP 3: Eliminar Empleado (Borrado Lógico)
CREATE PROCEDURE sp_EliminarEmpleado(
    IN p_ID_Empleado INT)
BEGIN
    UPDATE EMPLEADOS
    SET Activo = FALSE 
    WHERE ID_Empleado = p_ID_Empleado;
END //

-- SP 4: Obtener Empleados (Para la cuadrícula)
CREATE PROCEDURE sp_ObtenerEmpleados()
BEGIN
    SELECT 
        ID_Empleado, 
        Nombre, 
        Apellido, 
        Cargo 
    FROM 
        EMPLEADOS 
    WHERE 
        Activo = TRUE 
    ORDER BY Apellido, Nombre;
END //

DELIMITER ;

-- 9. TRIGGER (MONITOREO DE PRODUCTOS)
-- TRIGGERS que audita cambios en PRODUCTOS (INSERT, UPDATE, DELETE)
DROP TRIGGER IF EXISTS tr_AuditoriaProductos_INSERT;
DROP TRIGGER IF EXISTS tr_AuditoriaProductos_UPDATE;
DROP TRIGGER IF EXISTS tr_AuditoriaProductos_DELETE;

DELIMITER //
-- TRIGGER 1: AFTER INSERT
CREATE TRIGGER tr_AuditoriaProductos_INSERT
AFTER INSERT ON PRODUCTOS
FOR EACH ROW
BEGIN
    DECLARE v_clave_producto VARCHAR(20) DEFAULT NEW.CLAVE;
    DECLARE v_usuario_db VARCHAR(50) DEFAULT USER();
    DECLARE v_detalle_despues TEXT;
    
    SET v_detalle_despues = CONCAT('Nombre: ', NEW.NOMBRE, ' | Precio: ', NEW.PRECIO, ' | Stock: ', NEW.STOCK);

    INSERT INTO AUDITORIA_PRODUCTOS (CLAVE_Producto_Afectado, Tipo_Cambio, Usuario_DB, Campo_Afectado, Valor_Nuevo)
    VALUES (v_clave_producto, 'INSERT', v_usuario_db, 'REGISTRO COMPLETO', v_detalle_despues);
END //

-- TRIGGER 2: AFTER UPDATE
CREATE TRIGGER tr_AuditoriaProductos_UPDATE
AFTER UPDATE ON PRODUCTOS
FOR EACH ROW
BEGIN
    DECLARE v_clave_producto VARCHAR(20) DEFAULT NEW.CLAVE;
    DECLARE v_usuario_db VARCHAR(50) DEFAULT USER();

    -- Auditar cambios en NOMBRE
    IF OLD.NOMBRE <> NEW.NOMBRE THEN
        INSERT INTO AUDITORIA_PRODUCTOS (CLAVE_Producto_Afectado, Tipo_Cambio, Usuario_DB, Campo_Afectado, Valor_Anterior, Valor_Nuevo)
        VALUES (v_clave_producto, 'UPDATE', v_usuario_db, 'NOMBRE', OLD.NOMBRE, NEW.NOMBRE);
    END IF;

    -- Auditar cambios en DESCRIPCION
    IF OLD.DESCRIPCION <> NEW.DESCRIPCION THEN
        INSERT INTO AUDITORIA_PRODUCTOS (CLAVE_Producto_Afectado, Tipo_Cambio, Usuario_DB, Campo_Afectado, Valor_Anterior, Valor_Nuevo)
        VALUES (v_clave_producto, 'UPDATE', v_usuario_db, 'DESCRIPCION', LEFT(OLD.DESCRIPCION, 255), LEFT(NEW.DESCRIPCION, 255));
    END IF;

    -- Auditar cambios en PRECIO
    IF OLD.PRECIO <> NEW.PRECIO THEN
        INSERT INTO AUDITORIA_PRODUCTOS (CLAVE_Producto_Afectado, Tipo_Cambio, Usuario_DB, Campo_Afectado, Valor_Anterior, Valor_Nuevo)
        VALUES (v_clave_producto, 'UPDATE', v_usuario_db, 'PRECIO', OLD.PRECIO, NEW.PRECIO);
    END IF;

    -- Auditar cambios en STOCK
    IF OLD.STOCK <> NEW.STOCK THEN
        INSERT INTO AUDITORIA_PRODUCTOS (CLAVE_Producto_Afectado, Tipo_Cambio, Usuario_DB, Campo_Afectado, Valor_Anterior, Valor_Nuevo)
        VALUES (v_clave_producto, 'UPDATE', v_usuario_db, 'STOCK', OLD.STOCK, NEW.STOCK);
    END IF;
END //

-- TRIGGER 3: AFTER DELETE
CREATE TRIGGER tr_AuditoriaProductos_DELETE
AFTER DELETE ON PRODUCTOS
FOR EACH ROW
BEGIN
    DECLARE v_clave_producto VARCHAR(20) DEFAULT OLD.CLAVE;
    DECLARE v_usuario_db VARCHAR(50) DEFAULT USER();
    DECLARE v_detalle_antes TEXT;

    SET v_detalle_antes = CONCAT('Nombre: ', OLD.NOMBRE, ' | Precio: ', OLD.PRECIO, ' | Stock: ', OLD.STOCK);

    INSERT INTO AUDITORIA_PRODUCTOS (CLAVE_Producto_Afectado, Tipo_Cambio, Usuario_DB, Campo_Afectado, Valor_Anterior)
    VALUES (v_clave_producto, 'DELETE', v_usuario_db, 'REGISTRO COMPLETO', v_detalle_antes);
END //

DELIMITER ;

-- 10. INSERCIÓN DE DATOS DE PRUEBA

-- Las contraseñas se hashean en la aplicación real. 
-- Aquí usamos SHA2(X, 256) para simular el hash no reversible.
SET @PassAdmin = SHA2('adminpass', 256);
SET @PassVendedor = SHA2('ventaspass', 256);

-- Datos de Empleados (uso de Stored Procedure)
CALL sp_InsertarEmpleado('Juan', 'Perez', 'Gerente'); -- ID 1
CALL sp_InsertarEmpleado('Maria', 'Gomez', 'Vendedor'); -- ID 2
CALL sp_InsertarEmpleado('Carlos', 'Lopez', 'Vendedor'); -- ID 3

-- Datos de Usuarios (2 TIPOS: Administrador y Vendedor)
INSERT INTO USUARIOS (NombreUsuario, Contrasena_Hash, Tipo_Usuario, ID_Empleado) 
VALUES 
('admin', @PassAdmin, 'Administrador', 1),
('vendedor1', @PassVendedor, 'Vendedor', 2),
('vendedor2', @PassVendedor, 'Vendedor', 3);

-- Datos de Productos (Libros) 
INSERT INTO PRODUCTOS (CLAVE, NOMBRE, DESCRIPCION, PRECIO, STOCK) 
VALUES
('978-0131103627', 'El Arte de la Programación', 'Clásico sobre algoritmos y estructuras de datos.', 650.00, 20),
('978-0321765723', 'Bases de Datos Relacionales', 'Manual completo para diseño de bases de datos.', 480.50, 15),
('978-1984801939', 'La Guía de Taller de BD', 'Ejercicios prácticos y casos de estudio.', 399.99, 30),
('978-0061120084', 'Cien Años de Soledad', 'Novela emblemática de Gabriel García Márquez.', 220.00, 50),
('978-8420471830', 'Don Quijote de la Mancha', 'Clásico de la literatura española.', 190.75, 45),
('978-0743273565', 'El Gran Gatsby', 'Obra cumbre de la literatura norteamericana.', 150.00, 25),
('978-0451524935', '1984', 'Distopía de George Orwell.', 185.00, 35),
('978-0547928227', 'El Señor de los Anillos: La Comunidad', 'Primer libro de la trilogía.', 550.00, 18),
('978-0007440058', 'Crimen y Castigo', 'Novela filosófica de Fiodor Dostoievski.', 295.50, 22),
('978-0062315007', 'El Principito', 'Clásico infantil y filosófico.', 95.00, 60),
('978-0385732551', 'Fundamentos de C#', 'Libro de programación orientado a objetos.', 720.00, 12);

-- Datos de Ventas de Prueba
INSERT INTO VENTAS (Total, ID_Empleado) VALUES (1500.00, 2);
SET @VentaID1 = LAST_INSERT_ID();
INSERT INTO DETALLE_VENTA (ID_Venta, CLAVE_Producto, Cantidad, Precio_Unitario) VALUES
(@VentaID1, '978-0131103627', 1, 650.00), 
(@VentaID1, '978-0061120084', 4, 212.50); 

INSERT INTO VENTAS (Total, ID_Empleado) VALUES (499.99, 3);
SET @VentaID2 = LAST_INSERT_ID();
INSERT INTO DETALLE_VENTA (ID_Venta, CLAVE_Producto, Cantidad, Precio_Unitario) VALUES
(@VentaID2, '978-1984801939', 1, 399.99), 
(@VentaID2, '978-0062315007', 1, 95.00);
