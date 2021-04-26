import asyncpg
import asyncpg.prepared_stmt
import uuid
import typing
from .models import Payment
from msi_payments.settings import settings

GET_SQL = """
SELECT id, amount, payer, payee, description, external_id, is_return, status, error
FROM msipay
WHERE id = $1
"""

INSERT_SQL = """
INSERT INTO
    msipay(id, amount, payer, payee, description, external_id, is_return, status, error)
VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9)
"""

UPDATE_SQL = """
UPDATE msipay
SET status = $2, error = $3
WHERE id = $1
"""

CREATE_DB_SQL = """
CREATE TABLE IF NOT EXISTS msipay (
    id UUID NOT NULL PRIMARY KEY,
    amount DECIMAL(10, 3) NOT NULL,
    payer TEXT NOT NULL,
    payee TEXT NOT NULL,
    description TEXT NOT NULL,
    external_id VARCHAR(255) NULL,
    is_return BOOLEAN NOT NULL,
    status VARCHAR(16) NOT NULL,
    error VARCHAR(255) NOT NULL
)
"""


class Database:
    conn: asyncpg.Connection
    _get_stmt: asyncpg.prepared_stmt.PreparedStatement
    _insert_stmt: asyncpg.prepared_stmt.PreparedStatement
    _update_stmt: asyncpg.prepared_stmt.PreparedStatement

    async def connect(self):
        self.conn = await asyncpg.connect(settings.db_connection_string)

    async def prepare_db(self):
        await self.conn.execute(CREATE_DB_SQL)
        self._get_stmt = await self.conn.prepare(GET_SQL)
        self._insert_stmt = await self.conn.prepare(INSERT_SQL)
        self._update_stmt = await self.conn.prepare(UPDATE_SQL)

    async def disconnect(self):
        await self.conn.close()

    async def get_payment(self, payment_id: uuid.UUID) -> typing.Optional[Payment]:
        db_payment = await self._get_stmt.fetchrow(payment_id)
        if db_payment:
            return Payment.from_orm(db_payment)
        else:
            return None

    async def insert_payment(self, payment: Payment):
        await self._insert_stmt.execute(
            payment.id,
            payment.amount,
            payment.payer,
            payment.payee,
            payment.description,
            payment.external_id,
            payment.is_return,
            payment.status,
            payment.error,
        )

    async def update_payment_status(self, payment: Payment):
        await self._update_stmt.execute(payment.id, payment.status, payment.error)
