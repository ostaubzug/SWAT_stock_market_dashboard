# Stock Market Dashboard WEBLAB HS24 Projekt
# Kontext
Das Stock Market Dashboard stellt verschiedenste Aktien mit live updates zur Verfügung. Dies sollte dazu dienen das Stock traders möglichst schnell Informationen über den Markt bekommen können.

Als Software-Anbieter möchte ich das Stock Market Dashboard als SaaS anbieten. Das Dasboard besteht aus folgenden elementaren Teilen:
- User können sich registrieren und anmelden.
- Top Aktien werden als Dashboard angezeigt mit live updates
- Man kann nach Aktien suchen.

Optionaler Teil:
- User können ihr Dashboard modifizieren so wie sie es wollen.

# Anforderungen
- **User Story 1**: Registrieren / Anmelden auf der Webseite (Prio 'Must').
- **Akzeptanzkriterien**:
  - Mittels korrekten User Informationen: Name(Muss Eingabe), Vorname(Muss Eingabe), E-Mail(Muss Eingabe + muss korrektes Format haben) und validem Passwort (genug lang + Sonderzeichen) sich registrieren könenn.
    - Es sollte eine Validierung client seitig wie auch server seitig passieren.
  - Die gleiche E-Mail kann nicht zwei mal registriert werden.
  - Nach dem registrieren kann man sich direkt via Login anmelden.

- **User Story 2**: Das Dashboard zeigt Aktien als Graphen an(Prio 'Could').
- **Akzeptanzkriterien**:
  - Aktien werden als Graphen dargestellt.

- **User Story 3**: Es können Aktien zum Dashboard hinzugefügt oder entfernt werden (Prio 'Must').
- **Akzeptanzkriterien**:
  - Es können via Suche weitere Aktien ins Dashboard hinzugefügt werden.
  - Aktien die man nicht will können entfernt werden vom Dashboard.

- **User Story 4**: Live Updates auf den Akiten (Prio 'Should).
- **Akzeptanzkriterien**:
  - Die Aktien sollen live Updates ihres Preises ohne neu laden der Seite anzeigen.

- **User Story 5**: User Preference (Prio 'Should')
- **Akzeptanzkriterien**:
  - Mittels favorite button (stern z.B) können User aktien favorisieren.
  - Favorisierte Aktien werden immer zuerst geladen und sind auch zuoberst.

- **User Story 6**:
    

