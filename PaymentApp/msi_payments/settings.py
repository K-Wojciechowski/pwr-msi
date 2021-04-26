from pydantic import BaseSettings


class Settings(BaseSettings):
    db_connection_string: str
    server_address: str

    class Config:
        env_prefix = "msipay_"
        env_file = "../.env"


settings = Settings()
