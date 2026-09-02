-- Este script faz o povoamento das tabelas necessárias para a aplicação.

INSERT INTO aspnetroles (Id, Name, NormalizedName, ConcurrencyStamp) 
VALUES 
(UUID(), 'Administrador', 'ADMINISTRADOR', UUID()),
(UUID(), 'Cuidador', 'CUIDADOR', UUID()),
(UUID(), 'Paciente', 'PACIENTE', UUID());