# Meal Planner Chatbot in .NET demo
C# version of the Meal Planner Chatbot demo shown in the CodeMash presentation.

It uses the .NET version of LangChain to integrate with the OpenAI API and uses Postgres as the Vector Store to do RAG (retrieval augmented generation).

There are some differences from the Python version. 
1. There's no 'CSV Loader' in the LangChain .NET. Instead, I created a loader that is specific to loading the recipes CSV file. I did not try to generalize this to load any CSV file.
2. This doesn't use the memory feature (aka chat history).

Ref: https://github.com/ai-ml-workshops/meal-planner-chatbot

# How to run
Prereqs: 
1. Visual Studio 2022 and .NET 8
2. OpenAI API Key
3. Install Postgres w/ the PGVector extension. (I'd suggest running it in a Docker container).

Refer to https://github.com/ai-ml-workshops/SystemPrereqs/blob/main/2025-CodeMash.md for setting those things up.

Steps:
1. Clone this repo
2. Stick your OpenAI API key in an environment variable named OPENAI_API_KEY
3. Stick your Postgres connection string in an environment variable named PGVECTOR_CONNECTION_STRING

Example: "Host=localhost:5440;Username=postgres;Password=yourpassword;Database=meal_planner"

4. Build. This will load the two dependencies - LangChain and CsvHelper
5. Run
6. Ask: "3 meals with chicken". If everything is working, it will output the recipes.
