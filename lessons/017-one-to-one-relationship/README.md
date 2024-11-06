In today's lesson we'll look at one-to-one relationships in Entity Framework.  A one-to-one relationship is when each entity instance from one table is associated with only one entity instance in another table. This means that both tables share a unique link between their records, often through a primary key in one table that also acts as a foreign key in the related table. This type of relationship is used when you want to model exclusive relationships, such as a WeatherForecast having a unique alert message, where each Weatherforecast has exactly one Alert, and each Alert belongs to exactly one WeatherForecast.



`dotnet ef migrations add AddWeatherAlertTable`

In the coding exercise ...

## Main Points
- A one-to-one relationship is when each entity instance from one table is associated with only one entity instance in another table.
- Both tables share a unique link between their records, often through a primary key in one table that also acts as a foreign key in the related table.

## Suggested Coding Exercise
- Create a one-to-one relationship between a person and a passport.

## Building toward CSTA Standards:
- 

## Resources
- 
