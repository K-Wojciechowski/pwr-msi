import uuid

from decimal import Decimal
from enum import Enum
from pydantic import BaseModel
from typing import Optional


class PaymentStatus(Enum):
    REQUESTED = "REQUESTED"
    COMPLETED = "COMPLETED"
    FAILED = "FAILED"
    CANCELLED = "CANCELLED"


class Payment(BaseModel):
    id: uuid.UUID
    amount: Decimal
    currency: str
    payer: str
    payee: str
    description: str
    external_id: Optional[str]
    is_return: bool
    status: PaymentStatus
    error: Optional[str]

    class Config:
        orm_mode = True

    @classmethod
    def from_request(cls, payment_request: "PaymentRequest") -> "Payment":
        return cls(
            id=uuid.uuid4(),
            amount=payment_request.amount,
            currency=payment_request.currency,
            payer=payment_request.payer,
            payee=payment_request.payee,
            description=payment_request.description,
            external_id=payment_request.external_id,
            is_return=payment_request.is_return,
            status=PaymentStatus.REQUESTED,
            error=None,
        )

    @property
    def amount_str(self) -> str:
        if self.currency == "PLN":
            return f"{self.amount:0.2d}\xa0zł"
        else:
            return f"{self.amount:0.2d}\xa0{self.currency}"


class PaymentRequest(BaseModel):
    amount: Decimal
    currency: str
    payer: str
    payee: str
    description: str
    external_id: Optional[str]
    is_return: bool


class PaymentAcceptedResponse(BaseModel):
    id: uuid.UUID
    url: str
    payment: Payment


class PaymentCallback(BaseModel):
    id: uuid.UUID
    external_id: Optional[str]
    status: PaymentStatus
    error: Optional[str]
