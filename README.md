# This is a coding challenge exercise for ENSEK software engineer position.

The solution is built using .net core 3.1 lts. It has two projects
- MeterDataImport.Api ( this has the api controller, database migration as well as the validation code. Some of the depedencies are **csvhelper**, **fluent validation**. )
- MeterDataImport.Test ( this contains the test methods, it uses xunit and moq as its depedencies. )

## before runing the solution, please do the following ##
- adjust the connection string in appsettings.json file.
- run "update-database" command from the visual studio package manager console.

The soltution has swagger enabled. You should able to run the solution and test the endpoint from swagger ui.

## this solution assumes that you have seeded the initial account data.
