import ollama


def generate_response(query: str) -> str:
    prompt = f"""
You are Gearbox AI, an assistant for vehicle services, diagnostics, and maintenance.

User question:
{query}

Rules:
- Be clear and practical
- Keep it under 80 words
- Give actionable advice
"""

    response = ollama.chat(
        model="llama3",  # or your model
        messages=[
            {"role": "user", "content": prompt}
        ],
        options={
            "temperature": 0.3,
            "num_predict": 120
        }
    )

    return response["message"]["content"]