import asyncio
import httpx
import logging
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
callback_queue: asyncio.Queue = asyncio.Queue()
callback_task: asyncio.Task
templates = Jinja2Templates(directory="templates")
logging.basicConfig(format="%(asctime)-15s %(name)s %(levelname)-8s: %(message)s", level=logging.DEBUG)
callback_logger = logging.getLogger("msipay_callback")
callback_logger.setLevel(logging.DEBUG)


async def callback_worker():
    callback_logger.debug("Callback worker started.")
    async with httpx.AsyncClient() as client:
        while True:
            payment = await callback_queue.get()
            if payment is None:
                callback_logger.debug("Callback worker done.")
                callback_queue.task_done()
                break
            callback_logger.info(f"Sending callback for payment {payment.id}.")

            callback = models.PaymentCallback(
                id=payment.id, external=payment.external_id, status=payment.status, error=payment.error
            )
            try:
                await client.post(
                    settings.callback_url, content=callback.json(), headers={"Content-Type": "application/json"}
                )
                callback_logger.info(f"Callback for payment {payment.id} sent.")
            except Exception as e:
                callback_logger.exception(f"Callback for payment {payment.id} failed.")

            callback_queue.task_done()


@app.on_event("startup")
async def startup():
    await db.connect()
    await db.prepare_db()
    callback_task = asyncio.create_task(callback_worker())


@app.on_event("shutdown")
async def shutdown():
    await db.disconnect()
    try:
        await asyncio.wait_for(shutdown_callback_queue(), 5)
    except asyncio.TimeoutError:
        callback_logger.exception("Timeout when shutting down.")


async def shutdown_callback_queue():
    await callback_queue.put(None)
    await callback_queue.join()
    await callback_task


@app.get("/", response_class=HTMLResponse)
async def index(request: Request):
    return templates.TemplateResponse("index.html", {"request": request})


@app.get("/api/payment/{payment_id}/")
async def get_payment(
    payment_id: uuid.UUID,
) -> models.PaymentInfoResponse:
    payment = await db.get_payment(payment_id)
    if payment is None:
        raise HTTPException(status_code=404)
    url: Optional[str]
    if payment.can_pay:
        url = urllib.parse.urljoin(settings.server_address, f"/pay/{payment.id}/")
    else:
        url = None
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
    if not payment or not payment.can_pay:
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
    if not payment or not payment.can_pay:
        return templates.TemplateResponse("pay_error.html", {"request": request, "payment": payment})

    payment.status = new_status
    payment.error = new_error
    await db.update_payment_status(payment)

    await callback_queue.put(payment)

    if next:
        return RedirectResponse(url=next, status_code=302)
    else:
        return templates.TemplateResponse("pay_success.html", {"request": request, "payment": payment})
