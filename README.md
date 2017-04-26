# Rutetider

Simple web app that shows departure time for a given metro stations in Oslo. The data source is the [Ruter API](https://reisapi.ruter.no/help).

## Instructions

The app show departure time for a single station and direction only. The relevant parameters are set in the `<appSettings>`-section in `Web-config`:

- `StopId`: The metro station ID
- `Direction`