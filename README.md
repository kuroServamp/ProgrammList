
# ProgrammList (EN)

A Program to create a list of programs installled on a system. You may add this program on a networkfolder to get more information form single clients that execute a script to invoke it and save the data to a centralised database.

Supported databases are MSSQL, Sqlite and MySql.

For Sqlite a database file will be created as long as the parameter for it is in the coresponding app.conf file. The app.conf file has to be in the same folder as the .exe file.
For MSSQL and MySql standard parameters are used currently such as IP (localhost) and Port.

In later versions this should be made configurable through the app.conf file.

Plan for the future:
- UI To configure the program through parameter, default will be cli auto run. there may be branches for winform vs wpf 
- Additional Database support and eventuall csv support. (
- Depending on .net version (see branches) a portable version or installer.

# ProgrammList (GER)

Ein Programm zum erstellen einer Liste installierter Programme auf einem Rechner. Man kann das Programmm in einen Netzwerkordner legen um von mehreren Systemen die Information zu bekommen und diese Zentral zu Speichern.

Untersützt werden bisher Speichermethoden in MSSQL, Sqlite und MySql.

Für Sqlite wird eine Dantenbankdatei über eine app.conf date erstellt. Die app.conf Datei muss sich dazu im gleichen Ordner wie die exe befinden.
Für MSSQL und MySql werden bisher Standard Parameter für IP (Localhost) und Port verwendet.

In Späteren Versionen sollte dies auch über das Config file editierbar sein.

Zukunftsplan:
- Oberfläche zur Einrichtung der Konfigurationsdatei. Eventuell in eigenen Branches (WinForm vs WPF)
- Zusätzliche Datenbanken und eventuell auch ein CSV report.
- Je nach .net version, Portable single exe oder ein installer
