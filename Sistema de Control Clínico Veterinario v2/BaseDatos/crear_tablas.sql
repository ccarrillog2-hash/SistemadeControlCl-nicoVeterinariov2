CREATE TABLE IF NOT EXISTS Personas (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Tipo TEXT NOT NULL,               -- 'Cliente', 'Veterinario', 'Recepcionista'
    Nombre TEXT NOT NULL,
    Dpi TEXT NOT NULL UNIQUE,
    Telefono TEXT,
    Email TEXT,
    Direccion TEXT,
    Usuario TEXT,                     
    Contrasena TEXT,                  
    Nit TEXT,                       
    CodigoEmpleado TEXT,              
    Turno TEXT,                       
    CodigoVet TEXT,                   
    Especialidad TEXT,               
    Horario TEXT                      
);

CREATE TABLE IF NOT EXISTS Mascotas (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL,
    Especie TEXT,
    Raza TEXT,
    Sexo TEXT,
    FechaNacimiento TEXT,
    Peso REAL,
    PropietarioId INTEGER NOT NULL,
    FOREIGN KEY (PropietarioId) REFERENCES Personas(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Citas (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Fecha TEXT NOT NULL,
    Hora TEXT NOT NULL,
    Motivo TEXT,
    Estado TEXT NOT NULL DEFAULT 'Programada',
    ClienteId INTEGER NOT NULL,
    MascotaId INTEGER NOT NULL,
    VeterinarioId INTEGER NOT NULL,
    FOREIGN KEY (ClienteId) REFERENCES Personas(Id),
    FOREIGN KEY (MascotaId) REFERENCES Mascotas(Id),
    FOREIGN KEY (VeterinarioId) REFERENCES Personas(Id)
);

CREATE TABLE IF NOT EXISTS Consultas (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Fecha TEXT NOT NULL,
    Motivo TEXT,
    Diagnostico TEXT,
    Gravedad TEXT,
    Tratamiento TEXT,
    DuracionDias INTEGER,
    Medicamentos TEXT,
    MascotaId INTEGER NOT NULL,
    CitaId INTEGER,
    FOREIGN KEY (MascotaId) REFERENCES Mascotas(Id),
    FOREIGN KEY (CitaId) REFERENCES Citas(Id)
);

CREATE TABLE IF NOT EXISTS Facturas (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Fecha TEXT NOT NULL,
    Subtotal REAL NOT NULL,
    Impuestos REAL NOT NULL,
    Total REAL NOT NULL,
    Estado TEXT NOT NULL DEFAULT 'Pendiente',
    ClienteId INTEGER NOT NULL,
    FOREIGN KEY (ClienteId) REFERENCES Personas(Id)
);

CREATE TABLE IF NOT EXISTS Pagos (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Monto REAL NOT NULL,
    MetodoPago TEXT,
    Fecha TEXT NOT NULL,
    Estado TEXT NOT NULL DEFAULT 'Pendiente',
    FacturaId INTEGER NOT NULL,
    FOREIGN KEY (FacturaId) REFERENCES Facturas(Id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS IX_Citas_Fecha ON Citas(Fecha);
CREATE INDEX IF NOT EXISTS IX_Mascotas_Propietario ON Mascotas(PropietarioId);
CREATE INDEX IF NOT EXISTS IX_Consultas_Mascota ON Consultas(MascotaId);