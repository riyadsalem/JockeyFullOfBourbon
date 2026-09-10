# Domain Testing

Je krijgt een bestaande codebase en de bijhorende [requirements](requirements.md).

Je hebt deze code niet geschreven.

Ga er niet vanuit dat de implementatie correct is.

## Opdracht

Onderzoek een samenhangend deel van de applicatie.

Gebruik de requirements om het verwachte gedrag te bepalen en geautomatiseerde testen om het werkelijke gedrag te onderzoeken.

Wanneer je een defect vindt:

* maak het reproduceerbaar met een test
* documenteer het in `Defects.md`
* herstel het defect
* behoud de test als regressietest

Je hoeft niet de volledige applicatie te onderzoeken en je hoeft niet alle defecten te vinden.

## Testorganisatie

Organiseer testen volgens samenhangende domeinconcepten en gedrag.

Een lezer moet de testen voor een requirement gemakkelijk kunnen terugvinden zonder door ongerelateerde testklassen te zoeken.

Testnamen beschrijven de situatie die getest wordt en het verwachte resultaat.

De teststructuur moet succesvol gedrag, validatieregels, grenzen, state transitions en interacties tussen objecten duidelijk zichtbaar maken.

Vermijd grote verzamelklassen en een structuur die alleen gebaseerd is op implementatiedetails.

De testsuite moet leesbaar zijn als een gestructureerde beschrijving van het domein.

## Duplicatie en testondersteuning

Herhaal toevallige setup code niet overal.

Haal gedeelde constructie en setup uit wanneer dat de testen duidelijker maakt en de abstrahering een betekenisvolle rol krijgt.

Gebruik parameterized tests wanneer meerdere inputs dezelfde regel uitdrukken.

Gebruik builders, fixtures, helpers en gedeelde assertions wanneer ze ruis verwijderen.

Verberg geen informatie die nodig is om een test te begrijpen.

Vermijd diepe helperstructuren, verborgen mutable state en abstraheringen waarvoor je door meerdere bestanden moet springen om een scenario te begrijpen.

Minder lijnen code is op zichzelf geen reden voor een abstractie.

## Defect reporting

Documenteer voor elk gevonden verschil:

* welke requirement geschonden wordt
* welk gedrag de implementatie vertoont
* welk gedrag verwacht werd
* welke test of testen het probleem aantonen
* of het defect opgelost werd
* welke productiecode gewijzigd werd

Beschrijf het oorspronkelijke probleem onafhankelijk van de uiteindelijke oplossing.

Meerdere falende testen kunnen hetzelfde onderliggende defect aantonen. Beschouw die dan als één defect.

## Extra Tools
- [Coverage](coverage.md)
- [Stryker](mutation-testing.md)

## Verwachtingen

Aan het einde moet je kunnen aantonen:

* welk deel van de applicatie je onderzocht hebt
* welk gedrag je met testen bevestigd hebt
* welke defecten je gevonden en hersteld hebt
* welke delen je niet onderzocht hebt

De requirements blijven de bron voor het bedoelde gedrag.