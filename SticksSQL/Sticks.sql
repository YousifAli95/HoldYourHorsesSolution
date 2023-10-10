CREATE TABLE [dbo].[Sticks]
(
	[Artikelnr] INT NOT NULL PRIMARY KEY identity, 
    [Pris] MONEY NOT NULL, 
    [Hästkrafter] INT NOT NULL, 
    [Trädensitet] INT NOT NULL, 
    [Artikelnamn] NVARCHAR(50) NOT NULL unique, 
    [MaterialId] INT NOT NULL REFERENCES Material(Id), 
    [KategoriId] INT NOT NULL REFERENCES Categories(Id), 
    [Beskrivning] NVARCHAR(1000) NOT NULL, 
    [TillverkningslandId] INT NOT NULL REFERENCES OriginCountry(Id), 
    [AbsBroms] BIT NOT NULL
)
