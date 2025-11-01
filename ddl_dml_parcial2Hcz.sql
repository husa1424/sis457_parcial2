CREATE DATABASE Parcial2Hcz;
USE master
GO
CREATE LOGIN usrparcial2 WITH PASSWORD = '12345678',
	CHECK_POLICY = ON,
	CHECK_EXPIRATION = OFF,
	DEFAULT_DATABASE = Parcial2Hcz
GO
USE Parcial2Hcz
GO
CREATE USER usrparcial2 FOR LOGIN usrparcial2
GO
ALTER ROLE db_owner ADD MEMBER usrparcial2
GO

-- TABLA Canal
CREATE TABLE Canal (
  id INT IDENTITY(1,1) PRIMARY KEY,
  nombre VARCHAR(50) NOT NULL,
  frecuencia VARCHAR(20) NOT NULL,
  estado SMALLINT NOT NULL DEFAULT 1
);

-- TABLA Programa
CREATE TABLE Programa (
  id INT IDENTITY(1,1) PRIMARY KEY,
  idCanal INT NOT NULL,
  titulo VARCHAR(100) NOT NULL,
  descripcion VARCHAR(250) NOT NULL,
  duracion INT NOT NULL,
  productor VARCHAR(100) NOT NULL,
  fechaEstreno DATE NOT NULL,
  estado SMALLINT NOT NULL DEFAULT 1,
  CONSTRAINT fk_Programa_Canal FOREIGN KEY (idCanal) REFERENCES Canal(id)
);



ALTER TABLE Canal ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Canal ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Canal ADD estadoRegistro SMALLINT NOT NULL DEFAULT 1; -- -1: Eliminado, 0: Inactivo, 1: Activo


ALTER TABLE Programa ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Programa ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Programa ADD estadoRegistro SMALLINT NOT NULL DEFAULT 1; -- -1: Eliminado, 0: Inactivo, 1: Activo

select * from Canal
select * from Programa

GO
INSERT INTO Canal (nombre, frecuencia, estado) VALUES
('Canal 7', '90.5 FM', 1),
('Canal 11', '102.3 FM', 1),
('Canal 4', '87.9 FM', 1),
('Mega TV', '98.1 FM', 1),
('TV Mundo', '105.7 FM', 1);
 
 INSERT INTO Programa (idCanal, titulo, descripcion, duracion, productor, fechaEstreno, estado) VALUES
(1, 'Noticias Mañaneras', 'Informativo matutino', 60, 'Carlos Fuentes', '2024-01-05', 1),
(2, 'Cine Nocturno', 'Películas clásicas', 120, 'Ana Morales', '2023-11-20', 1),
(3, 'Música en Vivo', 'Presentaciones musicales en vivo', 90, 'Luis Herrera', '2024-02-10', 1),
(4, 'Debate Abierto', 'Debates sobre temas actuales', 45, 'María López', '2023-12-15', 1),
(5, 'Cocina Fácil', 'Recetas rápidas y sencillas', 30, 'Jorge Ramos', '2024-03-01', 1);

DROP PROC IF EXISTS paProgramaListar; --NO SE EJECUTA , ESTO ES PARA BORRAR
GO
CREATE PROC paProgramaListar @parametro VARCHAR(50)
AS
  SELECT pr.id,
         pr.idCanal,
         c.nombre AS canal,
         pr.titulo,
         pr.descripcion,
         pr.duracion,
         pr.productor,
         pr.fechaEstreno,
         pr.estado,
         pr.usuarioRegistro,
         pr.fechaRegistro
        





  FROM Programa pr
  INNER JOIN Canal c ON c.id = pr.idCanal
  WHERE pr.estado > -1
    AND (pr.titulo + pr.descripcion + c.nombre) LIKE '%' + REPLACE(@parametro,' ','%') + '%'
  ORDER BY pr.estado DESC, pr.titulo ASC;
GO

-- Ejemplo de ejecución
EXEC paProgramaListar 'noticias canal'; --ES UN BUSCADOR 
EXEC paProgramaListar '';


USE Parcial2Hcz;
GO

IF OBJECT_ID('dbo.TipoPrograma', 'U') IS NOT NULL
  DROP TABLE dbo.TipoPrograma;
GO

CREATE TABLE TipoPrograma (
  id INT IDENTITY(1,1) PRIMARY KEY,
  nombre VARCHAR(50) NOT NULL UNIQUE,
  descripcion VARCHAR(250) NULL,
  estado SMALLINT NOT NULL DEFAULT 1, -- -1 Elim, 0 Inactivo, 1 Activo
  usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME(),
  fechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
);
GO

INSERT INTO TipoPrograma (nombre, descripcion) VALUES
('noticiero', 'Programas de noticias e informativos'),
('serie', 'Series de TV por capítulos'),
('pelicula', 'Películas'),
('deportes', 'Transmisiones deportivas'),
('animado', 'Programas/series animadas');
GO

ALTER TABLE Programa ADD tipoId INT NULL;
GO

select * from Programa


IF OBJECT_ID('dbo.paProgramaListar', 'P') IS NOT NULL
  DROP PROC dbo.paProgramaListar;
GO

CREATE PROC paProgramaListar @parametro VARCHAR(50)
AS
  SELECT pr.id,
         pr.idCanal,
         c.nombre AS canal,
         pr.titulo,
         pr.descripcion,
         pr.duracion,
         pr.productor,
         pr.fechaEstreno,
         tp.nombre AS tipo, 
         pr.estado,
         pr.usuarioRegistro,
         pr.fechaRegistro
  FROM Programa pr
  INNER JOIN Canal c ON c.id = pr.idCanal
  INNER JOIN TipoPrograma tp ON tp.id = pr.tipoId
  WHERE pr.estado > -1
    AND (pr.titulo + pr.descripcion + c.nombre + tp.nombre) LIKE '%' + REPLACE(@parametro,' ','%') + '%'
  ORDER BY pr.estado DESC, pr.titulo ASC;
GO


select * from Canal
select * from Programa