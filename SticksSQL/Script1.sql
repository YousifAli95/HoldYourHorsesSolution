
--Tömmer tabellen
--truncate table Material

--Sätter in värden i tabellerna
insert into Material(Namn)
values
('Furu'), ('Ek'), ('Mahogony'), ('Gran')
go

insert into Kategorier(Namn)
values
('Sport'), ('Fritid'), ('Barn')
go

insert into Tillverkningsländer(Namn)
values
('Sverige'), ('Norge'), ('Danmark'), ('Finland')
go

insert into Sticks (Pris, Hästkrafter, Trädensitet, Artikelnamn, MaterialId, KategoriId, Beskrivning, TillverkningslandId, AbsBroms)
values
(1337, 250, 230,'Inzanity', 2, 1, 'Käpphäst med mjuk hårrem i polyester, med garnman som går att fläta. Mun som kan öppnas och stängas med kardborre, så att bettet ligger bra på plats.', 1, 1),
(1200, 180, 260,'Cloudberry Castle Budget', 1, 2, 'En rolig käpphäst med mjuk päls med lång man och ett fint träns.',2, 0),
(1100, 350, 260,'Pegasus Sea Lake', 3, 3, 'En lyxig käpphäst att trava runt trädgården och hoppa över hinder med.',4, 1),
(1800, 370, 200, 'Italian Stallion', 4, 1, 'Med käpphästen Italian Stallion är du snabbare än Rocky.', 2, 1),
(1500, 300, 350, 'Kingdra the Scaler', 1, 3, 'En brun käpphäst med brunt träns och grimma av mockainspirerat tyg. Simmar dessutom snabbt i vatten.', 4, 0),
(1423, 300, 350, 'The Cimarron Spirit', 4, 3, 'Med mustangen Spirit är du redo att ge dig ut i vildmarken.', 4, 0), --samma stats som Kingdra med flit
(1630, 400, 200, 'Secretariat', 3, 1, 'Med världsrekordsinnehavaren Secretariat har du chans att besegra alla motstånd.', 3, 1),
(900, 150, 200, 'Skyscraper Horse', 1,1, 'Scy Scraper Horse är den längsta hästen på marknaden. Längre än Empire State Byggnaden i New York.', 1,1),
(499, 150, 180, 'Stomme', 4, 2, 'Gör din egen käpphäst av denna fina stomme med käpp av trä och hästhuvud.', 2, 0),
(500, 175, 190, 'T-Rex', 1, 3, 'Den här magiska käpphästen älskar full fart och äventyr, precis som ditt barn!', 3, 0),
(600, 160, 200, 'Dino', 2, 3, 'En söt käpphäst föreställande en dinosaurie.', 1, 1),
(800, 200, 210, 'Star', 2, 2, 'Bygg upp en hinderbana och tävla med dina vänner och se vem som hoppar högst och vem som kommer först i mål!', 1, 0),
(900, 210, 230, 'Lilla Gubben', 4, 3, 'Alltid redo för en ridtur - med Lilla Gubben kan barnet susa fram på nya äventyr!', 1, 0),
(950, 300, 270, 'Unicorn Sunshine', 3, 2, 'Söt käpphäst i vitt med guldigt horn och tillhörande träns.', 3, 1),
(700, 200, 210, 'Palomino', 1, 1, 'Palormino är en sporthäst snabbare än blixten.', 1, 1),
(650, 180, 190, 'Wild West', 2, 1, 'Wild West har vit mule och röd scarf. Genom att trycka på hästens ena öra får du den att gnägga.', 2, 0),
(800, 250, 260, 'Pricken', 3, 1, 'Pricken har gång på gång satt nya banrekord och är därmed ett stort namn inom galoppsporten.', 4, 1),
(1000, 240, 250, 'Butterfly Flip', 3, 1, 'Med Butterfly Flip, även kallad Flippan, kommer du flyga över alla hinder.', 1, 1),
(800, 150, 170, 'Giddy Up', 1, 3, 'Den vilda käpphästen älskar full fart och äventyr! Ta hästen i tyglarna och klicka på knappen för att höra hovarna klappra mot marken, med så realistiska ljudeffekter att du nästan glömmer att det bara är en lek.', 3, 0),
(700, 160, 190, 'Playfun', 2, 1, 'Hästen har en regnbågsfärgad man och ett glittrigt horn i pannan. Käppen har flera färger och är beströdd med glänsande stjärnor.', 2, 0),
(3000, 200, 205, 'Hermes', 3, 2, 'Hermes är en käpphäst med stil och klass.', 3, 1)
go

--visa tabellerna
select * from Sticks
select * from Material
select * from Kategorier
select * from Tillverkningsländer

select 
	Artikelnr,
	Pris,
	Hästkrafter,
	Trädensitet,
	Artikelnamn,
	--material.id as MaterialId,
	material.Namn as MaterialNamn,
	--Kategorier.id as KategoriId,
	kategorier.namn as KategoriNamn,
	Tillverkningsländer.namn as TillverkningslandNamn
	--Tillverkningsländer.Id as TillverkningslandId
from Sticks
left join Material on sticks.MaterialId=material.Id
left join Kategorier on sticks.KategoriId=Kategorier.Id
left join Tillverkningsländer on sticks.TillverkningslandId = Tillverkningsländer.Id
go

-- om man vill radera alla poster i Sticks
delete from Sticks 
--om man vill radera alla poster i Material
delete from Material