# Introduction 
Library for setting and detecting maintenance time for a service.

# Getting Started
1.	Create MaintenanceTime section in your config file
2.	In Startup.cs call `services.AddSingleton(new MaintenanceTimeDetector(Configuration, "MaintenanceTime"));`
3.	Import MaintenanceTimeDetector instance in your service
--or--
1. Create MaintenanceTimeDetector as a list of MaintenanceTimeConfigEntry
2. You can create MaintenanceTimeConfigEntry by using any WeekdayTimeEntry constructor, or by specifying maintenance time in format "Weekday, Timespan":
    `new MaintenanceTimeConfigEntry("Tuesday, 6:00", "Wednesday,6:00")`

# Config section reference
```
  "MaintenanceTime": [ //array of time sections, please don't overlap
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
