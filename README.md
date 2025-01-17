# Meal Planner Chatbot in .NET demo
C# version of the Meal Planner Chatbot demo shown in the CodeMash presentation.

It uses:
1. LangChain for .NET
2. OpenAI API
3. Postgres as a Vector Database

There are some differences from the Python version. 
1. There's no 'CSV Loader' in the LangChain .NET. Instead, I created a loader that is specific to loading the recipes CSV file. I did not try to generalize this to load any CSV file.
2. This doesn't use memory (aka chat history).

Ref: https://github.com/ai-ml-workshops/meal-planner-chatbot

# How to run
Prereqs: 
1. Visual Studio 2022 and .NET 8
2. OpenAI API Key
3. Postgres w/ the PGVector extension.

Refer to https://github.com/ai-ml-workshops/SystemPrereqs/blob/main/2025-CodeMash.md for setting those things up.


Steps:
1. Stick your OpenAI API key in an environment variable named OPENAI_API_KEY
2. Stick your Postgres connection string in an environment variable named PGVECTOR_CONNECTION_STRING

Note: If you get a format error, use a connection string like this
"Host=localhost:5440;Username=postgres;Password=yourpassword;Database=meal_planner"

3. Build. This will load the two dependencies - LangChain and CsvHelper
4. Run
5. Ask: "3 meals with chicken". If everything is working, it will output the recipes.
