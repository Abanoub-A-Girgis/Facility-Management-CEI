# Facility Management Software - Maintenance Module

This Project is submitted as our final project for the course of the Civil Engineering Informatics track of the Information technology Institute in Egypt (Smart Village branch).

The project is made to showcase our capabilities as a team and to improve our knowledge of software development.

Technologies used:

> asp.net<br>
> HTML, CSS, Javascript<br>
> jQuery<br>
> jQuery UI<br>
> xBim<br>
> Chart.js<br>
> Datatable<br>
> IFCTools<br>


Template used:

> Skote MVC <br>

### Setting up

1. Add a new EF migration 

```sh
   add-migration MyMigration -context Facility_Management_CEI.IdentityDb.ApplicationDBContext
   ```
   
2. Update the database using the newly created EF migration 

```sh
   update-database MyMigration -context Facility_Management_CEI.IdentityDb.ApplicationDBContext
   ```