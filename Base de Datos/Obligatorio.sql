SET DATEFORMAT dmy

USE master
GO

IF EXISTS (select * from sys.databases where name = 'Obligatorio')
begin
	DROP DATABASE Obligatorio
end
GO

CREATE DATABASE Obligatorio ON
(
	NAME=Obligatorio,
	FILENAME='C:\BD\Obligatorio.mdf'
)
GO

USE Obligatorio
GO

--TABLAS

CREATE TABLE Empleados
(
	NombreUsuario varchar(10) PRIMARY KEY CHECK(LEN(LTRIM(RTRIM(NombreUsuario))) = 10),
	Contraseña varchar(7) NOT NULL CHECK(Contraseña LIKE '[a-zA-Z][a-zA-Z][a-zA-Z][a-zA-Z][0-9][0-9][0-9]' 
                                         OR Contraseña LIKE '[0-9][0-9][0-9][a-zA-Z][a-zA-Z][a-zA-Z][a-zA-Z]')
)
GO

CREATE TABLE Secciones
(
	CodigoSeccion varchar(5) PRIMARY KEY CHECK(LEN(LTRIM(RTRIM(CodigoSeccion))) = 5),
	Nombre varchar(20)NOT NULL,
	Activo bit Not Null Default (1)
)
GO

CREATE TABLE Periodistas
(
	Cedula varchar(8) PRIMARY KEY,
	Nombre varchar(20) NOT NULL,
	Email varchar (20) NOT NULL CHECK(Email LIKE '%[a-zA-Z0-9][@][a-zA-Z0-9]%[.][a-zA-Z0-9]%'),
	Activo bit Not Null Default (1)
)
GO

CREATE TABLE Noticias
(
	Codigo varchar(20) PRIMARY KEY,
	NombreUsuario varchar(10) FOREIGN KEY References Empleados(NombreUsuario) NOT NULL,
	CodigoSeccion varchar(5) FOREIGN KEY References Secciones(CodigoSeccion) NOT NULL,
    Titulo varchar(50) NOT NULL,
    Cuerpo varchar(8000) NOT NULL,
	Importancia int NOT NULL CHECK(Importancia >= 1 AND Importancia <= 5),
	-- si FechaPublicacion es menor a GETDATE(), DATEDIFF producirá un valor menor a 0
	FechaPublicacion date NOT NULL --CHECK(DATEDIFF(day,GETDATE(),FechaPublicacion) >= 0)
)
GO

CREATE TABLE Escriben
(
	Codigo varchar(20) FOREIGN KEY References Noticias(Codigo) NOT NULL,
	Cedula varchar(8) FOREIGN KEY References Periodistas(Cedula) NOT NULL,
	PRIMARY KEY(Codigo, Cedula)
)
GO

-- Procedimientos almacenados

-- ===============================================
CREATE PROCEDURE Logueo
--ALTER PROCEDURE Logueo
	@nombreUsuario VARCHAR(10),
	@contraseña VARCHAR(7)
AS
BEGIN
	SELECT * FROM Empleados WHERE NombreUsuario = @nombreUsuario AND Contraseña = @Contraseña
END
GO

-- ===============================================
CREATE PROCEDURE EliminarPeriodista
--ALTER PROCEDURE EliminarPeriodista
	@Cedula varchar(8),
	@ret int output
AS
BEGIN
    -- Caso periodista no existe
	IF (NOT EXISTS(SELECT * FROM Periodistas WHERE Cedula = @Cedula))
		SET @ret = -1 --No existe un periodista con la cedula ingresada
	
	DECLARE @Error INT
	
	-- Si el periodista publicó al menos una noticia, se realiza una baja lógica
	IF EXISTS(SELECT * FROM Escriben WHERE Cedula = @Cedula)
	BEGIN
		UPDATE Periodistas
		SET Activo = 0
		WHERE Cedula = @Cedula
		
		SET @Error = @@ERROR
	END
	ELSE
	BEGIN
	-- Si el periodista no publicó nada, se realiza una baja física
		DELETE Periodistas WHERE Cedula = @Cedula
		
		SET @Error = @@ERROR
	END
	
	IF(@Error = 0)
		SET @ret = 1
	ELSE
		SET @ret = -2 -- Hubo un error y no se pudo eliminar el periodista de la base de datos
END
GO

-- ===============================================
CREATE PROCEDURE EliminarSeccion
--ALTER PROC EliminarSeccion
	@CodigoSeccion varchar(5),
	@ret int output
AS
BEGIN 
	-- Verifica que la sección exista
	IF (NOT EXISTS (SELECT CodigoSeccion FROM Secciones WHERE CodigoSeccion = @CodigoSeccion))
		SET @ret = -1; -- La sección ingresada no existe
	
	DECLARE @Error INT
	
	-- Si la sección tiene noticias publicadas, se realiza una baja lógica
	IF (EXISTS (SELECT * FROM Noticias WHERE CodigoSeccion = @CodigoSeccion ))
	BEGIN
		UPDATE Secciones
		SET Activo = 0
		WHERE CodigoSeccion = @CodigoSeccion
		
		SET @Error = @@ERROR
	END
	ELSE 
	BEGIN
		-- Si la sección no tiene noticias publicadas, se realiza una baja física
		DELETE Secciones WHERE CodigoSeccion = @CodigoSeccion
		SET @Error = @@ERROR
	END
	
	IF(@Error != 0)
	BEGIN
		SET @ret = -2 -- No se pudo eliminar la sección de la base de datos.
	END
	
	SET @ret = 1
END
GO

-- ===============================================
CREATE PROCEDURE AltaPeriodista
--ALTER PROCEDURE AltaPeriodista
	@Cedula varchar(8),
	@Nombre varchar(20),
	@Email varchar(20)
AS
BEGIN
	--Caso periodista inactivo
	IF (EXISTS(SELECT * FROM Periodistas WHERE Cedula = @Cedula AND Activo = 0))	
	BEGIN
		UPDATE Periodistas
		SET Nombre = @Nombre, Email = @Email, Activo = 1
		WHERE Cedula = @Cedula
		
		RETURN 1
	END
	
	-- Caso periodista activo
	IF (EXISTS(SELECT * FROM Periodistas WHERE Cedula = @Cedula AND Activo = 1))
	BEGIN
		RETURN -1 --Ya existe un periodista con esa cedula en el sistema
	END
	
	-- Si no existe el periodista, se le da de alta	
	INSERT Periodistas(Cedula,Nombre,Email) VALUES (@Cedula,@Nombre,@Email)
	
	IF (@@ERROR = 0)
		RETURN 1
	ELSE
		RETURN -2 -- No se pudo dar de alta al periodista en la base de datos
END
GO

-- ===============================================
CREATE PROCEDURE ModificarPeriodista
--ALTER DROP PROCEDURE ModificarPeriodista
	@Cedula varchar(8),
	@Nombre varchar(20),
	@Email varchar(20)
AS
BEGIN
	--Caso si el periodista no está activo
	IF (NOT EXISTS(SELECT * FROM Periodistas WHERE Cedula = @Cedula AND Activo = 1))
		BEGIN
			RETURN -1 --No existe un periodista con la cedula ingresada
		END
	ELSE
	BEGIN
	--Caso si el periodista está activo
		UPDATE Periodistas
		SET Nombre = @Nombre, Email = @Email
		WHERE Cedula = @Cedula
			
		IF (@@ERROR = 0)
			RETURN 1
		ELSE
			RETURN -2 -- Hubo un error y no se pudo actualizar los datos del periodista
	END
END
GO

-- ===============================================
CREATE PROCEDURE AltaEmpleado
--ALTER PROCEDURE AltaEmpleado
  @nombreUsuario VARCHAR(10),
  @contraseña VARCHAR(7)
AS
BEGIN
	--Verifico que no exista un empleado con ese usuario.
	IF (EXISTS (SELECT NombreUsuario FROM Empleados WHERE NombreUsuario = @nombreUsuario))
	BEGIN
		RETURN -1; -- Ya existe un empleado con el nombre de usuario ingresado
	END
	
	INSERT INTO Empleados(NombreUsuario, Contraseña) 
	VALUES(@nombreUsuario, @contraseña)
	
	IF @@ERROR != 0
	BEGIN
		RETURN -2 -- No se pudo crear el empleado en la base de datos
	END
	
	RETURN 1
END
GO

-- ===============================================
CREATE PROCEDURE AltaSeccion
--ALTER PROCEDURE AltaSeccion
  @codigoSeccion VARCHAR(5),
  @nombre VARCHAR(20)
AS
BEGIN
	-- Verifica si la seccion existe y está activa.
	IF (EXISTS(SELECT * FROM Secciones WHERE CodigoSeccion = @codigoSeccion AND Activo = 1))
	BEGIN
		RETURN -1 --Ya existe una seccion con ese codigo en el sistema.
	END
	
	--Verifica si la seccion existe en la base de datos pero está inactiva.
	IF (EXISTS(SELECT * FROM Secciones WHERE CodigoSeccion = @codigoSeccion AND Activo = 0))	
	BEGIN
		UPDATE Secciones
		SET Nombre = @Nombre, Activo = 1
		WHERE CodigoSeccion = @codigoSeccion
		
		IF @@ERROR != 0
		BEGIN
			RETURN -2 -- No se pudo crear la sección en la base de datos
		END
		
		RETURN 1
	END
	
	-- Si no existe la seccion, se le da de alta.
	INSERT INTO Secciones(CodigoSeccion, Nombre) VALUES (@codigoSeccion, @nombre)
	
	IF @@ERROR != 0
	BEGIN
		RETURN -2 -- No se pudo crear la sección en la base de datos
	END
	
	RETURN 1
END
GO

-- ===============================================
CREATE PROCEDURE ModificarSeccion
  @codigoSeccion VARCHAR(5),
  @nuevoNombre VARCHAR(20)
AS
BEGIN
	-- Verifica si la seccion existe y está activa.
	IF (NOT EXISTS(SELECT * FROM Secciones WHERE CodigoSeccion = @codigoSeccion AND Activo = 1))
	BEGIN
		RETURN -1 --No existe una seccion con ese codigo
	END
	
	UPDATE Secciones 
	SET Nombre = @nuevoNombre 
	WHERE CodigoSeccion = @codigoSeccion
	
	IF @@ERROR != 0
	BEGIN
		RETURN -2 -- No se pudo actualizar la sección
	END
	
	RETURN 1
END
GO

--INSERTS

EXEC AltaEmpleado 'Empleado10', 'empl123'
EXEC AltaEmpleado 'Empleado20', '234empl'
EXEC AltaEmpleado 'Empleado30', 'empl345'
EXEC AltaEmpleado 'Empleado40', 'empl456'
GO

EXEC AltaSeccion 'polic','Policial'
EXEC AltaSeccion 'econo','Economia'
EXEC AltaSeccion 'depor','Deportes'
EXEC AltaSeccion 'cultu','Cultural'
EXEC AltaSeccion 'viaje','Viajes'
EXEC ModificarSeccion 'cultu','Cultura'
GO

EXEC AltaPeriodista '44259772', 'Periodista 1', 'period1@gmail.com'
EXEC AltaPeriodista '51237687', 'Periodista 2', 'period2@gmail.com'
EXEC AltaPeriodista '39867291', 'Periodista 3', 'period3@gmail.com'
EXEC AltaPeriodista '67839025', 'Periodista 4', 'period4@gmail.com'
EXEC AltaPeriodista '00000000', 'Periodista 5', 'period5@gmail.com'
EXEC ModificarPeriodista '67839025', 'Periodista 6', 'period6@gmail.com'
GO


SELECT * FROM Empleados
SELECT * FROM Secciones
SELECT * FROM Periodistas
SELECT * FROM Noticias
SELECT * FROM Escriben

ALTER TABLE Noticias WITH NOCHECK
	ADD CONSTRAINT VerificarAño
	-- si FechaPublicacion es menor a GETDATE()
	-- DATEDIFF producirá un valor menor a 0
	CHECK(DATEDIFF(day,GETDATE(),FechaPublicacion) >= 0) 
GO

--Creacion de usuario IIS para que el sitio pueda acceder a la BD-------------------------------------------
USE master
GO

CREATE LOGIN [IIS APPPOOL\DefaultAppPool] FROM WINDOWS 
GO

USE Obligatorio
GO

CREATE USER [IIS APPPOOL\DefaultAppPool] FOR LOGIN [IIS APPPOOL\DefaultAppPool]
GO

exec sys.sp_addrolemember 'db_owner', [IIS APPPOOL\DefaultAppPool]
go