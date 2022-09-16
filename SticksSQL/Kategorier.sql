CREATE TABLE [dbo].[Kategorier]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [Namn] NVARCHAR(50) NOT NULL unique
)
