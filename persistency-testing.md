# Persistence met EF Core en SQLite

## Context

De oorspronkelijke opdracht werkt uitsluitend met domeinobjecten in het geheugen. In deze vervolgopdracht voeg je persistence toe zonder de bestaande domeinregels af te zwakken.

Gebruik **Entity Framework Core** met de relationele **SQLite-provider**. Gebruik dus niet de EF Core InMemory-provider.

## Opdracht

Zorg ervoor dat `Coach` en `Course`, inclusief hun volledige toestand en onderlinge relaties, in een SQLite-database bewaard en opnieuw geladen kunnen worden.

Minstens de volgende gegevens moeten persistent zijn:

* de identiteit van coaches en cursussen;
* naam en e-mailadres van een coach;
* de skills van een coach;
* naam, periode en bevestigingsstatus van een cursus;
* de vereiste skills van een cursus;
* de timeslots van een cursus;
* de toewijzing van een coach aan een cursus.

Na het opnieuw laden moeten de domeinobjecten hetzelfde waarneembare gedrag en dezelfde toestand hebben als vóór het bewaren.

## Ontwerpverwachtingen

* De bestaande `Id`-gebaseerde identiteit en equality van entities blijft behouden.
* Value objects blijven verantwoordelijk voor hun eigen betekenis en validatie.
* Maak domeinstate niet publiek wijzigbaar enkel om EF Core tevreden te stellen.
* Configureer mappings expliciet waar conventies de bedoeling van het domein niet correct kunnen afleiden.
* Modelleer de relatie tussen coach en toegewezen cursussen als één coherente relatie. Vermijd twee losstaande representaties die uit sync kunnen raken.
* Kies bewust hoe value objects en collecties relationeel worden voorgesteld. Documenteer keuzes die gevolgen hebben voor constraints, querying of toekomstige wijzigingen.
* Voeg databaseconstraints en unieke constraints toe waar ze een blijvende domein- of datainvariant ondersteunen.
* Voeg een EF Core-migratie toe waarmee een lege SQLite-database kan worden opgebouwd.

Productiecode mag aangepast worden wanneer dat nodig is voor persistence, maar zulke wijzigingen mogen geen bestaand domeingedrag omzeilen. Motiveer betekenisvolle aanpassingen aan het domeinmodel.

## Te ondersteunen scenario's

Toon minstens aan dat je:

1. een coach en cursus kunt bewaren en opnieuw laden;
2. skills en timeslots correct kunt round-trippen;
3. een bevestigde cursus met toegewezen coach kunt bewaren;
4. die cursus en coach opnieuw kunt laden met een consistente relatie in beide richtingen;
5. een geladen entity kunt wijzigen via zijn domeinmethodes en de wijziging opnieuw kunt bewaren;
6. meerdere entities met dezelfde beschrijvende eigenschappen maar verschillende `Id`s afzonderlijk kunt bewaren en laden.

## Testen

Schrijf integratietesten tegen de echte SQLite-provider. Een SQLite in-memory database is toegestaan wanneer de connectie gedurende de test open blijft en EF daadwerkelijk SQL uitvoert.

Test niet alleen dat `SaveChanges` geen exception gooit. Controleer de toestand na een nieuwe `DbContext` te gebruiken, zodat de assertions niet per ongeluk slagen dankzij EF change tracking.

Behoud daarnaast de bestaande domeintesten. Ze moeten zonder inhoudelijke wijzigingen blijven slagen.


