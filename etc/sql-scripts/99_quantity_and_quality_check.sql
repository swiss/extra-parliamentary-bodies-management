
-- Diese Datei gibt immer Select count(*) Scripts die Soll-Zahlen aus, welche in der Quell-Datenbank drin sind.
-- Mit dem 2. Select können dann die Soll-Zahlen mit den Ist-Zahlen von der Ziel-DB verglichen werden.


-- Kontroll-Scripts

-- Auf SQL Server ausführen
select count(*) from Gremium
-- 475

-- Auf PostGreSQL DB ausführen
select count(*) from committees 
-- 474 (1 hat kein Amt, siehe unten)

select count(*) from Gremium where EndDatum is not null or Histo is not null
-- 274
select count(*) from committees where end_date is not null and end_date < now()
-- 273

select count(*) from Gremium where EndDatum is null or Histo is null
-- 201
select count(*) from committees where end_date is null
-- 201


-- Aktive und History Mitglieder
select 
(select count(*) from Mitglied m where m.GremId = g.Id and m.Histo is not null and m.BisDate is not null ) HistMembers,
(select count(*) from Mitglied m where m.GremId = g.Id and m.Histo is  null and m.BisDate is  null) ActiveMembers,
* 
from Gremium g 
where g.Histo is not null and g.EndDatum is not null


-- unvollständig
select * from Gremium g
where g.Histo is not null
and g.EndDatum is not null
and (g.ZustVerwStelleId not in (select id from Verwaltungsstelle )
or g.ZustVerwStelleId is null
)
-- 1

select count(*) from Mitglied
-- 9458
select count(*) from memberships
-- 9458

select count(*) from Mitglied where (BisDate is not null AND BisDate < getdate()) -- or Histo is not null
-- 7801
select count(*) from memberships  where end_date is not null and end_date < now()
-- 7801

select count(*) from Mitglied where (BisDate is null or BisDate >= getdate()) -- AND Histo is  null
-- 1657
select count(*) from memberships  where end_date is null
-- 0
select count(*) from memberships  where end_date is not null and end_date > now()
-- 1657

select 1657 + 1801 --> 9458 OK

select PerId, count(*) from Mitglied group by PerId having count(*) > 1 order by count(*) desc

2684	17
2281	13
2341	13
1277	13
2290	11
131	    10
966	    9
2244	9
3209	9
5921	9
etc..

select person_id, count(*) from memberships group by person_id having count(*) > 1 order by count(*) desc
"04dff00b-cb20-4d71-a1b3-0070b315278a"	17
"da82a9fe-9a33-48a9-bded-9313809e29c3"	13
"a787908e-328d-4479-8a85-39a1d8d5fba3"	13
"c76af126-4a24-43f5-a07d-c2976ff812ad"	13
"cfc8e5f7-838c-49c6-aeb4-fc9b543bf9ab"	11
"819e4853-d610-4b63-ba4f-bf2dfa26f24f"	10
"6e75f4bd-adbe-4aa1-ae8f-b6adc03d4b8e"	9
"cb34c614-a67d-4031-b95e-b3b51d9f9519"	9
"03ba6398-6e2a-4561-8273-ac06df822a13"	9
"b7b58484-6aff-4747-a501-1dab0bf55374"	9
etc


select PerId, count(*) from Mitglied where (BisDate is not null AND BisDate > getdate()) group by PerId having count(*) > 1 order by count(*) desc
7860	7
5694	6
7243	5
7795	5


select person_id, count(*) from memberships where end_date is not null and end_date > now() group by person_id  having count(*) > 1 order by count(*) desc
"b798cfb7-ad2d-4150-970f-d7018bdaa4c4"	7
"5ab86436-8c3f-4128-a334-ce717dc03a57"	6
"dbb90243-896e-484c-83ab-ca1f9e9d2cee"	5
"ae42658f-2431-41a6-a5a7-1f8ef5ed76bf"	5


select count(*) from Person
-- 7643
select count(*) from persons 
-- 7643

select count(*) from Person where Histo is not null
-- 253

select count(*) from Person where Histo is null
-- 7390

--> Da Histo-Feld auf der Person ist wertlos

select count(*) from Adresse
-- 10896


-- Adressen, die mindestens eine Angabe haben und zu einer Person gehören
select 
* from Adresse 
Where PersId is not null
AND (
(Strasse is not null AND Strasse <> '') OR
(LänderCode is not null AND LänderCode <> '') OR
(Plz is not null AND Plz <> '') OR
(Ort is not null AND Ort <> '') OR
(Kanton is not null AND Kanton <> '') OR
(Mobile is not null AND Mobile <> '') OR
(Telefon is not null AND Telefon <> '') OR
(Email is not null AND Email <> '') OR
(Firma is not null AND Firma <> '') OR
(Postfach is not null AND Postfach <> '') OR
(PlzZusatz is not null AND PlzZusatz <> '') 
)
-- 8474 sinnvolle Datensätze

select count(*) from adresses




-- Vorbereitung Migration Interessenbindungen & Journal

SELECT *
  FROM [PAPGBK].[dbo].[InteressenbindungFunktion] i
  LEFT JOIN Translation t ON t.OwningObjectGuid = i.Guid 

SELECT *
  FROM [PAPGBK].[dbo].[InteressenbindungGremium] i
  LEFT JOIN Translation t ON t.OwningObjectGuid = i.Guid 

SELECT *
  FROM [PAPGBK].[dbo].[InteressenbindungRechtsform] i
  LEFT JOIN Translation t ON t.OwningObjectGuid = i.Guid 


SELECT *
  FROM [PAPGBK].[dbo].[InteressenbindungenFailedToMatch] i


SELECT *
  FROM [PAPGBK].[dbo].[Interessenbindung]


  
SELECT *
  FROM [PAPGBK].[dbo].[Journal] j


SELECT *
  FROM [PAPGBK].[dbo].[JournalCode] j
    LEFT JOIN Translation t ON t.OwningObjectGuid = j.Guid 
