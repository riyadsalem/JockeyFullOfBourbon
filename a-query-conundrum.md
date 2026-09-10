# Query: geschikte en beschikbare coaches zoeken

## Context

De applicatie gebruikt nu EF Core en SQLite om coaches en cursussen te bewaren. Een gebruiker wil voor een bestaande cursus alle coaches kunnen opvragen die op dit moment zowel **geschikt** als **beschikbaar** zijn.

Deze zoekopdracht is geen vervanging voor de domeinregels bij het werkelijk toewijzen van een coach. Een zoekresultaat is een momentopname; de toewijzing zelf moet nog steeds veilig kunnen weigeren wanneer de toestand ondertussen veranderd is.

## Opdracht

Ontwerp en implementeer een databasequery die geschikte en beschikbare coaches voor een gegeven cursus teruggeeft.

De query moet paginering ondersteunen en een stabiele, deterministische sortering gebruiken. Definieer een duidelijk input- en outputcontract, bijvoorbeeld met een cursus-id, paginanummer, paginagrootte en een compacte projectie van de gevonden coaches.

Je mag EF LINQ, geparameteriseerde SQL, een databasefunctie, een view of een combinatie daarvan gebruiken. Motiveer je keuze. Een afzonderlijke read-database of een tweede domeinmodel is niet vereist.

## Geschiktheid

Een coach is geschikt wanneer die **alle** vereiste skills van de cursus bezit.

Let ook op de lege verzameling: iedere coach voldoet aan een cursus zonder vereiste skills.

## Beschikbaarheid

Een coach is beschikbaar wanneer geen enkele reeds toegewezen cursus werkelijk samenvalt met de gegeven cursus.

Twee cursussen conflicteren alleen wanneer er minstens één concrete kalenderdatum bestaat waarvoor:

1. de datum binnen de periode van beide cursussen valt;
2. de weekdag overeenkomt met de weekdag van een timeslot van beide cursussen;
3. de uren van die timeslots overlappen.

Timeslots die elkaar enkel aan hun grens raken, overlappen niet. `09:00–10:00` en `10:00–11:00` vormen dus geen conflict.

## Belangrijke valkuil

Het is niet voldoende om alleen te controleren dat:

* de periodes elkaar overlappen;
* beide cursussen een timeslot op dezelfde weekdag hebben;
* en die uren overlappen.

Bijvoorbeeld:

```text
Cursus A: maandag 7 september t.e.m. vrijdag 11 september
Cursus B: dinsdag 8 september t.e.m. maandag 14 september
Timeslots: beide op maandag van 09:00 tot 11:00
```

De periodes overlappen en beide cursussen bevatten een maandag, maar niet dezelfde maandag. Hun gemeenschappelijke periode loopt van dinsdag 8 september tot en met vrijdag 11 september en bevat geen maandag. Deze cursussen conflicteren daarom niet.

Controleer dus of de relevante weekdag werkelijk voorkomt in de **doorsnede van de twee periodes**.

## Query- en paginatievereisten

* Geschiktheid en beschikbaarheid moeten volledig bepaald zijn vóór paginering wordt toegepast.
* Laad niet eerst alle coaches of alle mogelijke resultaten in het applicatiegeheugen om ze daar te filteren.
* Gebruik geen `AsEnumerable`, `ToList` of vergelijkbare materialisatie vóór de geschiktheids- en beschikbaarheidsfilters en de paginering voltooid zijn.
* Sla niet voor iedere cursusdag een afzonderlijke occurrence op enkel om deze zoekopdracht mogelijk te maken.
* Vermijd een N+1-querypatroon.
* Gebruik een stabiele sortering met een unieke tie-breaker, zodat dezelfde toestand voorspelbare pagina's oplevert.
* SQL die zelf wordt geschreven moet geparameteriseerd zijn.

De query mag een compacte read-projectie teruggeven; ze hoeft geen volledige `Coach`-aggregates te materialiseren.

## Minimaal te testen scenario's

Voorzie integratietesten tegen SQLite voor minstens:

* een coach met alle vereiste skills;
* een coach bij wie één van meerdere vereiste skills ontbreekt;
* een cursus zonder vereiste skills;
* een coach zonder toegewezen cursussen;
* overlappende periodes met een echt timeslotconflict op dezelfde kalenderdatum;
* overlappende periodes en timeslots op dezelfde weekdag, maar zonder die weekdag in de gemeenschappelijke periode;
* een gemeenschappelijke periode van exact één dag waarop de timeslots conflicteren;
* timeslots die elkaar alleen aan de grens raken;
* meerdere geschikte coaches met dezelfde naam over meerdere pagina's;
* een lege pagina en de laatste gedeeltelijk gevulde pagina.

Bewijs in minstens één test of via vastgelegde query-output dat filtering, sortering en paginering door SQLite worden uitgevoerd en niet na materialisatie in applicatiecode.

