from fastapi import FastAPI, HTTPException, Request
from fastapi.responses import HTMLResponse, RedirectResponse
from fastapi.templating import Jinja2Templates

from msi_payments import models
from msi_payments.db import Database
from msi_payments.settings import settings

app = FastAPI()
db = Database()
templates = Jinja2Templates(directory="templates")


@app.on_event("startup")
async def startup():
    await db.connect()
    await db.create_db()


@app.on_event("shutdown")
async def shutdown():
    await db.disconnect()


@app.get("/", response_class=HTMLResponse)
async def index(request: Request):
    return templates.TemplateResponse("index.html", {"request": request})


@app.get("/api/{payment_id}/")
async def new_payment(
    payment_id: uuid.UUID,
) -> models.Payment:
    payment = await db.get_payment(payment)
    if payment is None:
        raise HTTPException(status_code=404)
    return payment



@app.post("/api/new/")
async def new_payment(
    payment_request: models.PaymentRequest,
) -> models.PaymentAcceptedResponse:
    payment: models.Payment = models.Payment.from_request(payment_request)
    await db.insert_payment(payment)
    url = urllib.parse.urljoin(settings.server_address, f"/pay/{payment.id}/")
    res = models.PaymentAcceptedResponse(
        id=payment.id,
        url=url,
        payment=payment,
    )
    return res


@app.get("/pay/{payment_id}/", response_class=HTMLResponse)
async def pay_get(payment_id: uuid.UUID, next: str):
    # TODO return url
    payment = await db.get_payment(payment)
    if not payment or payment.status != models.PaymentStatus.REQUESTED:
        return templates.TemplateResponse("pay_error.html", {"request":
            request, "payment": payment})
    return templates.TemplateResponse("pay.html", {"request": request,
        "payment": payment, "next": next})

@app.post("/pay/{payment_id}/", response_class=HTMLResponse)
async def pay_get(payment_id: uuid.UUID, new_status: models.PaymentStatus,
        new_error: Optional[str], next: str):
    payment = await db.get_payment(payment)
    if not payment or payment.status != models.PaymentStatus.REQUESTED:
        return templates.TemplateResponse("pay_error.html", {"request":
            request, "payment": payment})

    payment.status = new_status
    payment.error = new_error
    await db.update_payment_status(payment)

    return RedirectResponse(url = next, status_code=302)
