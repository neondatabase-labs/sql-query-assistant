-- Create a table to store vector embeddings
CREATE EXTENSION IF NOT EXISTS vector;

CREATE TABLE vector_data (
    id SERIAL PRIMARY KEY,
    description TEXT NOT NULL,
    embedding VECTOR(1536)
);

-- Create a customers Table
CREATE TABLE customers (
    id SERIAL PRIMARY KEY,
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    email TEXT NOT NULL UNIQUE,
    phone_number TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);