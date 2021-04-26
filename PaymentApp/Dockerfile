FROM python:3-slim AS build-env
WORKDIR /app

COPY requirements.txt ./
RUN apt-get update && apt-get install -y build-essential
RUN python -m venv venv
RUN venv/bin/pip install --no-cache-dir -r requirements.txt

FROM python:3-slim
WORKDIR /app
COPY --from=build-env /app/venv venv
COPY msi_payments templates /app/
ENTRYPOINT ["venv/bin/uvicorn", "--port=5007" "msi_payments.app:app"]
