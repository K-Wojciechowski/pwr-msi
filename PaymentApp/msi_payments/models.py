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
    payer: str
    payee: str
    description: str
    external_id: Optional[str]
    is_return: bool
    status: PaymentStatus
    error: Optional[str]

    class Config:
        orm_mode = True


class PaymentRequest(BaseModel):
    amount: Decimal
    payer: str
    payee: str
    description: str
    external_id: Optional[str]
    is_to_user: bool


class PaymentAcceptedResponse(BaseModel):
    id: uuid.UUID
    url: str
    payment: Payment


class PaymentCallback(BaseModel):
    id: uuid.UUID
    external_id: Optional[str]
    status: PaymentStatus
    error: Optional[str]
