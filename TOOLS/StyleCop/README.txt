In de Tools install directory staat de installer voor de StyleCop. Ik heb de directory waar de installer de bestanden neer zet naar de project map gekopieerd en refereerd hierna vanuit de solution zodat
het ook blijft werken op een PC waar de enterprise library niet staat. Uiteraard moeten hier de dll's dan wel neer worden gezet. Bij mij zet de installer ze in :
C:\Program Files (x86)\StyleCop 4.7
en de hele inhoud heb ik hier dus naar toe gekopieerd.

LET OP ik heb hier nog het bestand :
StyleCop.Current.Targets
aan toegevoegd zodat de stylecop versie en het project iets losser van elkaar staan en niet meteen aangepast worden bij het installeren van een nieuwe stylecop versie.