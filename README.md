# ProgrammList

Ein Programm zum erstellen einer Liste installierter Programme auf einem Rechner. Man kann das Programmm in einen Netzwerkordner legen um von mehreren Systemen die Information zu bekommen und diese Zentral zu Speichern.

Untersützt werden bisher Speichermethoden in MSSQL, Sqlite und MySql.

Für Sqlite wird eine Dantenbankdatei über eine app.conf date erstellt. Die app.conf Datei muss sich dazu im gleichen Ordner wie die exe befinden.
Für MSSQL und MySql werden bisher Standard Parameter für IP (Localhost) und Port verwendet.

In Späteren Versionen sollte dies auch über das Config file editierbar sein.

Zukunftsplan:
- Oberfläche zur Einrichtung der Konfigurationsdatei. Eventuell in eigenen Branches (WinForm vs WPF)
- Zusätzliche Datenbanken und eventuell auch ein CSV report.
- Je nach .net version, Portable single exe oder ein installer