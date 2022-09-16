CREATE TABLE [dbo].[Ordrar]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [Förnamn] NVARCHAR(50) NOT NULL, 
    [Efternamn] NVARCHAR(50) NOT NULL, 
    [Epost] NVARCHAR(50) NOT NULL, 
    [Stad] NVARCHAR(50) NOT NULL, 
    [Postnummer] INT NOT NULL, 
    [Adress] NVARCHAR(50) NOT NULL, 
    [Land] NVARCHAR(50) NOT NULL, 
    [User] NVARCHAR(450) NULL REFERENCES AspNetUsers(Id)
)