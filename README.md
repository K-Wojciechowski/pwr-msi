MSI
===

Production environment
----------------------

<https://msi.krzysztofwojciechowski.pl/>

Log into server: user = msi, password = nSHwuL2RpWhAsJRk

Log into app: user = admin, password = nSHwuL2RpWhAsJRk

Mailhog is available at <https://msi.krzysztofwojciechowski.pl/mailhog>.

Local development setup
-----------------------

Run app in development: use Rider (or VS), or `dotnet run`.

Run PostgreSQL, Redis, Mailhog in Docker: `docker-compose -f docker-compose.infra.yml up`

Database migration: Install `dotnet ef` first by running `dotnet tool install
--global dotnet-ef`. Then you can use `dotnet ef database update` to migrate the
database.

Verification e-mails will be sent to Mailhog, which is available at <http://localhost:8025/>.
