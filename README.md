# CD Collection - Entity Framework

This test is a bit freer than you are used to. We want you to build an API according to the specification below, and supply tests for your own code. We will grade the API on how well it works according to the specification and your code quality.

## Background - what the heck is a CD?

(This is just information about why I selected this exercise)

Back in the days we didn't have mp3-files. We stored our music on plastic discs called CDs. It was very important to keep track on which CDs you had, their genres and which artist played on which CDs etc.

Since your music library was a physical pile of discs it was pretty hard to know what music you had, after about 30 CDs. Our dream was to have some kind of application where we could search for this etc.

This exercise is built as an homage of that dream

## The data model

- A CD has a name, an artist name, a description and a purchased date
- A Genre has a name
- Each CD have exactly one Genre
- Each Genre can have many many CDs

## The exercise

Build a Web API that:

- stores it data in an SQL server (use the supplied Docker image, as during the week)
  - create migrations for your model so that we can run `dotnet ef database update` and get the database in the correct state
  - you don't need to seed the database with data

- Has the following features:
  - Post to create a new CD (POST `/api/CDs/`)
  - List all CDs in the database (GET `/api/CDs/`)
    - Add a parameter to filter by genre
    - This parameter can be empty which should list all CDs
  - Get one CD and all it's related data (GET `/api/CDs/{id}`)
  - Add Artist to a CD (PUT `/api/CDs/{id}/artist`, send artist in the body of the request)
  - Add Genre to a CD (PUT `/api/CDs/{id}/genre`, send genre name in the body of the request)

- Write tests that verifies that your code works on a suitable level (unit or integration tests) using the techniques we learned during the week.
- You can use the Swagger or CURL to verify your application end-to-end. Swagger will create CURL commands for you you to create these.
  - You can also use [Postman](https://www.postman.com/), that is installed on your computers, or [Insomnia](https://insomnia.rest/) if you rather use an UI

## Evaluation

Evaluation will be done by:

- running your tests and see that they pass
- verifying the basic functionality of the API
- looking through the code and making sure that it is easy to understand and well written

## Creating the solution

Create a new folder called `CDCollection` and create a solution in that folder:

```bash
dotnet new sln -n efCdCollection
dotnet new webapi -n efCdCollection.Api
dotnet new xunit -n efCdCollection.Tests
dotnet add efCdCollection.Tests reference efCdCollection.Api
dotnet sln add **/*.csproj
dotnet build
```

From here you need to add the correct packages and use generators etc. But that is part of the test.

### Solution technical requirements

In order for this to be automated we need:
- that you have used the supplied `docker-compose.yml` file 
  - (if you are using an M1 computer, switch line 5 in the yml file from `image: mcr.microsoft.com/mssql/server:2019-latest` to `image: mcr.microsoft.com/azure-sql-edge`)
- that the database is named `CdCollection`
- that your API runs on `http://localhost:3000/api/CDs/`

## Handing in the test

Upload all your code, including the test-project in a folder called `efCdCollection`.

Please run a `dotnet clean` in your folder before handing it in, to minimize the amount of files that we need to download.

# FAQ

Are we allow to use generator to scaffold our code?

> Yes, by all means. Use the tools we have talked about during the week, as you see fit

Should we also submit our test files?

> Yes - everything in the root folder of the project, including the .sln-file.
