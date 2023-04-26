# Introduction 
Library for setting and detecting maintenance time for a service.

[![Nuget](https://img.shields.io/nuget/v/FlutterEffect.MaintenanceTime)](https://www.nuget.org/packages/FlutterEffect.MaintenanceTime/)

# Getting Started
1.  Install FlutterEffect.MaintenanceTime nuget package
2.	Create MaintenanceTime section in your config file
3.	In Startup.cs call `services.AddSingleton(new MaintenanceTimeDetector(Configuration, "MaintenanceTime"));`
4.	Import MaintenanceTimeDetector instance in your service

--or--

2. Create MaintenanceTimeDetector as a list of MaintenanceTimeConfigEntry
3. You can create MaintenanceTimeConfigEntry by using any WeekdayTimeEntry constructor, or by specifying maintenance time in format "Weekday, Timespan":
    `new MaintenanceTimeConfigEntry("Tuesday, 6:00", "Wednesday,6:00")`

# Config section reference
```
  "MaintenanceTime": [ //array of time sections
    {
      "From": { //Beginning of maintenance
        "Weekday": "Friday", //Day of week (weekly). Skip this for daily maintenance.
        "Time": "17:00" //Time of day
      },
      "To": { //Ending of maintenance
        "Weekday": "Sunday",
        "Time": "19:00"
      }
    },
    {
      "From": {
        "Weekday": "Monday",
        "Time": "17:00"
      },
      "To": {
        "Weekday": "Monday",
        "Time": "19:00"
      },
      "Disabled": true //to disable the entry
    }
  ]
```
