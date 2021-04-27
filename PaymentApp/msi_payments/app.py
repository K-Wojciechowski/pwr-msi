import urllib.parse
import uuid

from typing import Optional

from fastapi import FastAPI, Form, HTTPException, Request
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
    await db.prepare_db()


@app.on_event("shutdown")
async def shutdown():
    await db.disconnect()


@app.get("/", response_class=HTMLResponse)
async def index(request: Request):
    return templates.TemplateResponse("index.html", {"request": request})


@app.get("/api/{payment_id}/")
async def get_payment(
    payment_id: uuid.UUID,
) -> models.PaymentInfoResponse:
    payment = await db.get_payment(payment_id)
    if payment is None:
        raise HTTPException(status_code=404)
    url = urllib.parse.urljoin(settings.server_address, f"/pay/{payment.id}/")
    res = models.PaymentInfoResponse(
        id=payment.id,
        url=url,
        payment=payment,
    )
    return res


@app.post("/api/new/")
async def new_payment(
    payment_request: models.PaymentRequest,
) -> models.PaymentInfoResponse:
    payment: models.Payment = models.Payment.from_request(payment_request)
    await db.insert_payment(payment)
    url = urllib.parse.urljoin(settings.server_address, f"/pay/{payment.id}/")
    res = models.PaymentInfoResponse(
        id=payment.id,
        url=url,
        payment=payment,
    )
    return res


@app.get("/pay/{payment_id}/", response_class=HTMLResponse)
async def pay_get(request: Request, payment_id: uuid.UUID, next: Optional[str] = None):
    payment = await db.get_payment(payment_id)
    if not payment or payment.status != models.PaymentStatus.REQUESTED:
        return templates.TemplateResponse("pay_error.html", {"request": request, "payment": payment})
    next_str = next if next else ""
    return templates.TemplateResponse("pay.html", {"request": request, "payment": payment, "next": next_str})


@app.post("/pay/{payment_id}/", response_class=HTMLResponse)
async def pay_post(
    request: Request,
    payment_id: uuid.UUID,
    new_status: models.PaymentStatus = Form(...),
    new_error: Optional[str] = Form(None),
    next: Optional[str] = Form(None),
):
    payment = await db.get_payment(payment_id)
    if not payment or payment.status != models.PaymentStatus.REQUESTED:
        return templates.TemplateResponse("pay_error.html", {"request": request, "payment": payment})

    payment.status = new_status
    payment.error = new_error
    await db.update_payment_status(payment)

    if next:
        return RedirectResponse(url=next, status_code=302)
    else:
        return templates.TemplateResponse("pay_success.html", {"request": request, "payment": payment})
