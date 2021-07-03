MSI
===

A university project for *Mobilne Systemy Informatyczne* — an app to order food
and get it delivered.

Technologies
------------

* Main backend: C# + ASP.NET Core + REST
* Frontend: Angular + Bootstrap 4
* MSIPay app (simple payment simulator): Python + FastAPI + Jinja2
* Database: PostgreSQL

Screenshots
-----------

![Start screen](https://raw.githubusercontent.com/K-Wojciechowski/pwr-msi/master/screenshots/msi-start-screen.png)

![Menu (order creation)](https://raw.githubusercontent.com/K-Wojciechowski/pwr-msi/master/screenshots/msi-menu-ordering.png)

![Order details](https://raw.githubusercontent.com/K-Wojciechowski/pwr-msi/master/screenshots/msi-order-details.png)

Local development setup
-----------------------

Run app in development: use Rider, Visual Studio, or VS Code, or `dotnet run`.

Run PostgreSQL, Redis, Mailhog, Minio, MSIPay in Docker: `docker-compose -f docker-compose.infra.yml up`

Database migration: Install `dotnet ef` first by running `dotnet tool install
--global dotnet-ef`. Then you can use `dotnet ef database update` to migrate the
database.

Verification e-mails will be sent to Mailhog, which is available at <http://localhost:8025/>.
