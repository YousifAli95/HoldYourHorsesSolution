CREATE TABLE [dbo].[Tillverkningsländer]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [Namn] NVARCHAR(50) NOT NULL unique
)
