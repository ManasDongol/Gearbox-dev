from fastapi import FastAPI
from pydantic import BaseModel
import ollama

app = FastAPI()

class QueryRequest(BaseModel):
    query: str


@app.get("/")
def root():
    return {"message": "Gearbox AI is running"}


@app.post("/ask")
def ask_ai(request: QueryRequest):

    prompt = f"""
You are Gearbox AI, an assistant for vehicle services, diagnostics, and maintenance.

User question:
{request.query}

Rules:
- Be clear and practical
- Keep it under 80 words
- Give actionable advice
"""

    response = ollama.chat(
        model="llama3",
        messages=[
            {"role": "user", "content": prompt}
        ],
        options={
            "temperature": 0.3,
            "num_predict": 120
        }
    )

    return {
        "answer": response["message"]["content"]
    }