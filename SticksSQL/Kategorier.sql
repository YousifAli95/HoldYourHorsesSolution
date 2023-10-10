CREATE TABLE [dbo].Categories
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [Namn] NVARCHAR(50) NOT NULL unique
)
