CREATE TABLE [dbo].[Favourites]
(
	[Id] INT identity NOT NULL PRIMARY KEY,
	[User] NVARCHAR(450) NOT NULL REFERENCES AspNetUsers(Id),
	[Artikelnr] INT NOT NULL REFERENCES Sticks(Artikelnr) 


)
