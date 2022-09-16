CREATE TABLE [dbo].[Material]
(
	[Id] INT NOT NULL PRIMARY KEY identity unique, 
    [Namn] NVARCHAR(50) NOT NULL unique
)
