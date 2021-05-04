FROM python:3-slim AS build-env
WORKDIR /app
COPY requirements.txt /app/requirements.txt
RUN apt-get update && apt-get install -y build-essential
RUN python -m venv venv
RUN venv/bin/python -m pip install --no-cache-dir -U pip setuptools wheel
RUN venv/bin/pip install --no-cache-dir -r requirements.txt

FROM build-env AS test-env
WORKDIR /app
RUN apt-get update && apt-get install -y git
COPY requirements-dev.txt /app/requirements-dev.txt
RUN venv/bin/pip install --no-cache-dir -r requirements-dev.txt
COPY msi_payments /app/msi_payments
COPY pyproject.toml /app/pyproject.toml
RUN venv/bin/mypy -p msi_payments
RUN venv/bin/black --check msi_payments

FROM python:3-slim
WORKDIR /app
COPY --from=build-env /app/venv /app/venv
COPY --from=test-env /app/msi_payments /app/msi_payments
COPY templates /app/templates
ENTRYPOINT ["venv/bin/uvicorn", "--port=5007", "--host=0.0.0.0", "msi_payments.app:app"]
